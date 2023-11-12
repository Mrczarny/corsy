using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection.PortableExecutable;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace corsy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            var envVars = Environment.GetEnvironmentVariables();
            var proxyVarNames = new[] {"ProxyAddress", "ProxyUser", "ProxyUserPassword"};

            if (proxyVarNames.Any(x => String.IsNullOrEmpty(envVars[x]?.ToString())) 
                && proxyVarNames.Any(x =>  String.IsNullOrEmpty(builder.Configuration[x])))
            {
                throw new Exception("Add proxy configuration!");
            }

            builder.Services.AddHttpClient("proxyHttp").AddHeaderPropagation().ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    Proxy = new WebProxy(Environment.GetEnvironmentVariable("ProxyAddress") ??
                                         builder.Configuration["ProxyAddress"])
                    {
                        Credentials = new NetworkCredential(
                            Environment.GetEnvironmentVariable("ProxyUser") ?? builder.Configuration["ProxyUser"],
                            Environment.GetEnvironmentVariable("ProxyUserPassword") ??
                            builder.Configuration["ProxyUserPassword"])
                    },
                    AutomaticDecompression = DecompressionMethods.All
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseHeaderPropagation();

            app.UseAuthorization();

            var omittedHeaders = builder.Configuration.GetSection("omitHeaders").Get<string[]>() ?? new string[] { };
            
            app.MapGet("/corsy/{*uri}", async Task<IResult> (HttpContext httpContext, IHttpClientFactory httpClientFactory, string? uri) 
                => await CorsyHandler(httpContext, httpClientFactory, omittedHeaders, uri));

            app.MapPost("/corsy/{method:alpha}/{*uri}", async Task<IResult> (HttpContext httpContext, IHttpClientFactory httpClientFactory, string? uri, string method,[FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] IDictionary<string, string> specialHeaders) 
                => await CorsyHandler(httpContext, httpClientFactory, omittedHeaders, uri,method, specialHeaders));

            app.Run();

        }

        public static async Task<IResult> CorsyHandler(HttpContext httpContext, IHttpClientFactory httpClientFactory, IEnumerable<string> omittedHeaders, string? uri,string method = "get", IDictionary<string, string>? specialHeaders = null)
        {
            var allOmittedHeaders = omittedHeaders.ToList();

            if (String.IsNullOrEmpty(uri))
            {
                return Results.BadRequest("Pass url as a parameter!");
            }

            if (specialHeaders is null)
            {
                //return Results.BadRequest("Pass headers you want to add!");
                specialHeaders = new Dictionary<string, string>();
            }


            //lazy but works
            try
            {
                var d = new HttpMethod(method);
            }
            catch (Exception e)
            {
                return Results.BadRequest("Bad request method!");
            }


            if (method == "get")
            {
                 allOmittedHeaders.AddRange(new[] { "Content-Type", "Content-Length"});
            }

            var client = httpClientFactory.CreateClient("proxyHttp");
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (Uri.TryCreate(uri, UriKind.Absolute, out _))
            {
                try
                {

                    var req = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(uri + httpContext.Request.QueryString),
                        Method = new HttpMethod(method),
                    };

                    foreach (var specialHeader in specialHeaders)
                    {
                        req.Headers.Add(specialHeader.Key, value: specialHeader.Value);
                    }



                    foreach (var header in httpContext.Request.Headers.Where(x => x.Value != StringValues.Empty))
                    {
                        if (!allOmittedHeaders.Contains(header.Key))
                        {
                            req.Headers.Add(header.Key, values: header.Value);
                        }

                    }

                    var response = await client.SendAsync(req);


                    // var response = await client.GetAsync(uri);


                    if (response.IsSuccessStatusCode)
                    {
                        return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
                    }

                    return Results.Problem(statusCode: (int)response.StatusCode, detail: response.ReasonPhrase);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                    //throw;
                    return Results.Problem();
                }

            }

            return Results.BadRequest("Bad url!");
        }

    }
}