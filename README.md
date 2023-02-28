<p align="center">
  <img alt="GitHub language count" src="https://img.shields.io/github/languages/count/GabrielBueno200/GeekShopping-Microservices">

  <img alt="GitHub repo size" src="https://img.shields.io/github/repo-size/GabrielBueno200/GeekShopping-Microservices">
  
  <a href="https://github.com/GabrielBueno200/GeekShopping-Microservices/commits/main">
    <img alt="GitHub last commit" src="https://img.shields.io/github/last-commit/GabrielBueno200/GeekShopping-Microservices">
  </a>
  
   <img alt="GitHub" src="https://img.shields.io/github/license/GabrielBueno200/GeekShopping-Microservices">
</p>

<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/GabrielBueno200/GeekShopping-Microservices">
    <img src="https://github.com/GabrielBueno200/GeekShopping-Microservices/blob/main/GeekShopping.Web/wwwroot/images/geek_shopping.png?raw=true" alt="Logo" width="550">
  </a>
</p>

<p align="center">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white"/>
  <img alt="C#" src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white"/>
  <img alt="Docker" src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white"/>
</p>


<p align="center">
  <img alt="Html" src="https://img.shields.io/badge/HTML-239120?style=for-the-badge&logo=html5&logoColor=white"/>
  <img alt="CSS" src="https://img.shields.io/badge/CSS-239120?&style=for-the-badge&logo=css3&logoColor=white"/>
  <img alt="Boostrap" src="https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white"/>
</p>


<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#-about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#-how-to-run">How To Run</a>
    </li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## 💻 About The Project
The project is an example of ecommerce application developed using .NET Core based on a simplified microservices architecture and Docker containers.

### Architecture
![image](https://user-images.githubusercontent.com/56837996/221745738-aa5483ca-2226-40f2-b360-8fe7929c2e6d.png)

<!-- HOW TO RUN -->
## 🚀 How To Run

### 1. Running with docker-compose

```bash

# Clone the repository
$ git clone https://github.com/GabrielBueno200/GeekShopping-Microservices.git

# Access the project folder in your terminal / cmd
$ cd GeekShopping-Microservices

# Run docker-compose commands below
docker-compose -f docker-compose.yml -f docker-compose-override.yml build
docker-compose -f docker-compose.yml -f docker-compose-override.yml up
```

After these steps, open your browser and access ```http://localhost:5002```

### 2. Running with docker-compose
You can run the .NET projects locally setting the environment variable ```ASPNETCORE_ENVIRONMENT``` to "Local".

