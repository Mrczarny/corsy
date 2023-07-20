<!--
*** Readme created using https://github.com/othneildrew/Best-README-Template 
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]




<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

Corsy is minimal api that proxy requests and 
adds Access-Control-Allow-Origin header to response

### Built With

* ASP.NET Core 7.0

<!-- GETTING STARTED -->
## Getting Started

To make a request you first need to add proxy address,
proxy user and his password in `appsettings.json` or as an environment variables.



```
 /corsy/${uri}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `uri` | `string` | **Required**. Url of site you want to request |


<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/Mrczarny/corsy/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Jan Adamski - adamski.jj@gmail.com

Project Link: [https://github.com/Mrczarny/corsy](https://github.com/Mrczarny/corsy)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Img Shields](https://shields.io)
* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Pages](https://pages.github.com)
* [Animate.css](https://daneden.github.io/animate.css)
* [Loaders.css](https://connoratherton.com/loaders)
* [Slick Carousel](https://kenwheeler.github.io/slick)
* [Sticky Kit](http://leafo.net/sticky-kit)
* [JVectorMap](http://jvectormap.com)
* [Font Awesome](https://fontawesome.com)





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/Mrczarny/corsy?style=for-the-badge
[contributors-url]: https://github.com/Mrczarny/corsy/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Mrczarny/corsy?style=for-the-badge
[forks-url]: https://github.com/Mrczarny/corsy/network/members
[stars-shield]: https://img.shields.io/github/stars/Mrczarny/corsy?style=for-the-badge
[stars-url]: https://github.com/Mrczarny/corsy/stargazers
[issues-shield]: https://img.shields.io/github/issues/Mrczarny/corsy?style=for-the-badge
[issues-url]: https://github.com/Mrczarny/corsy/issues
[license-shield]: https://img.shields.io/github/license/Mrczarny/corsy?style=for-the-badge
[license-url]: https://github.com/Mrczarny/corsy/blob/master/LICENSE.txt
