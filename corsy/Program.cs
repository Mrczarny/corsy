using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
                    Proxy = new WebProxy()
                    {
                        Address = new Uri(Environment.GetEnvironmentVariable("ProxyAddress") ??
                                          builder.Configuration["ProxyAddress"]),
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


            app.Map("/corsy/{*uri}", async Task<IResult> (HttpContext httpContext, IHttpClientFactory httpClientFactory, string? uri) =>
            {
                if (String.IsNullOrEmpty(uri))
                {
                    return Results.BadRequest("Pass url as a parameter!");
                }

                var client = httpClientFactory.CreateClient("proxyHttp");
                var omittedHeaders = builder.Configuration.GetSection("omitHeaders").Get<string[]>() ?? new string[] {};
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                if (Uri.TryCreate(uri, UriKind.Absolute, out _))
                {
                    try
                    {

                        var req = new HttpRequestMessage()
                        {
                            RequestUri = new Uri(uri+ httpContext.Request.QueryString),
                            Method = new HttpMethod(httpContext.Request.Method),
                        };
                        foreach (var header in httpContext.Request.Headers.Where(x => x.Value != StringValues.Empty))
                        {
                            if (header.Key != "Host" && !omittedHeaders.Contains(header.Key))
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

                        return Results.Problem(statusCode: (int) response.StatusCode, detail: response.ReasonPhrase);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}",e.Message);
                        //throw;
                        return Results.Problem();
                    }

                }

                return Results.BadRequest("Bad url!");

            });

            app.Run();
        }
    }
}