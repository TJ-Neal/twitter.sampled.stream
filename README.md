<!-- omit from toc -->
# Twitter Sampled Stream Project
<br/>

Sample code for interacting with, tracking, and analysizing data from the [Twitter Sampled Stream](https://developer.twitter.com/en/docs/twitter-api/tweets/volume-streams/quick-start/sampled-stream) developed as a proof of concept for a coding challenge.
<br/><br/>

## Table of Contents

- [**1. Purpose**](#1-purpose)
- [**2. Practices**](#2-practices)
- [**3. Concessions**](#3-concessions)
- [**4. Primary Technologies and Packages**](#4-primary-technologies-and-packages)
- [**5. Prerequisites**](#5-prerequisites)
  - [**5.1 Web API, Twitter Stream Reader, Kafka, and Kafka Stream Reader**](#51-web-api-twitter-stream-reader-kafka-and-kafka-stream-reader)
  - [**5.2 React Front-end UI**](#52-react-front-end-ui)
- [**6. Structure**](#6-structure)
- [**7. User Secrets and `SSL` Certs**](#7-user-secrets-and-ssl-certs)
  - [**Twitter API keys**](#twitter-api-keys)
  - [**Self-signed (dev) `SSL` Certs**](#self-signed-dev-ssl-certs)
- [**8. Getting Started**](#8-getting-started)
  - [**Docker**](#docker)
  - [**Health Checks**](#health-checks)
  - [**Direct API Access**](#direct-api-access)
  - [**Swagger UI**](#swagger-ui)
  - [**Sample React Application**](#sample-react-application)
- [**9. Clean-up**](#9-clean-up)
- [**10. Conclusion**](#10-conclusion)

<br/>

## **1. <u>Purpose</u>**
<br/>

The purpose of this project began as a way for me to demonstrate my grasp of modern software development practices, architecture, and principles. However, as I began to develop the project, I found myself wanting to explore other ways of implementing a solution. Because of that, it has grown into a comparison of 3 different tiers of API processing.

- A simple in-memory repository that has no duribility, but is performant and asynchronous.
- A more complex hybrid in-memory/file repository that introduces duribility without giving up performance or asynchronous aspects.
- A message broker based append log solution allows for pub/sub behavior, duribility, replayability, and asynchronous processing.
<br/><br/>

## **2. <u>Practices</u>**
<br/>

The following practices for modern software development were used as much as possible:
<br/>

- SOLID Principles
- Clean Code Architecture
- Microservice Architecture
- API Gateway
- Containers and orchestration
- .NET Minimal API
- Repository Pattern
- Event-Driven Architecture
- Structured Logging
- Unit Testing
<br/><br/>

## **3. <u>Concessions</u>**
<br/>
There are a number of concessions that I made due to the nature of the project and the scope that I decided to undertake to demonstrate the technologies.
<br/><br/>

1. Unit Tests - Though I did include some unit tests to demonstrate my grasp of the subject and use of `Moq`, `Xunit`, and `Dependency Injection` in unit testing, they are by no means intended to be exhaustive, are not indicitive of `Test Driven Development` (or a lack of appreciation for the practice) and are admittedly quite anemic. My appologies.
2. Security - I would never normally include any sort of secrets in a repository; however, I did make one exception in this case in order to test and demonstrate `SSL` certificates in `Docker` containers. These are only `dev-certs` and the `GUID` used as a password conveys no risk.
3. DRY - I love `DRY` code. I live by `DRY` code. I literally create nuget packages for my teams to enforce `DRY` code. This project is not `DRY` code. You will see many pieces that are nearly identical. This is on purpose. I wanted to treat the three distinct API implementations as separate entities that would not all normally co-exist. Therefore, only common business models, library wrappers, and injected interfaces were used. Abstraction of things like the Program similarities or configuration similarities were left in place as to not give an appearance of interoperability or cohesion.
<br/><br/>

## **4. <u>Primary Technologies and Packages</u>**
<br/>

- [Windows 11](https://www.microsoft.com/en-us/windows?wa=wsignin1.0) - Host/Development OS
- [WSL 2](https://learn.microsoft.com/en-us/windows/wsl/install) - Windows Subsystem for Lixus, used to host Linux containers on Windows OS
- [.NET Core 7](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0#recommended-learning-path) - Application framework
- [C# 11](https://learn.microsoft.com/en-us/dotnet/csharp/) - Primary programming language
- [Twitter Volume Stream - Sampled Stream](https://developer.twitter.com/en/docs/twitter-api/tweets/volume-streams/quick-start/sampled-stream) - Data source API
  - [LinqToTwitter](https://github.com/JoeMayo/LinqToTwitter) - Data source client
- [Docker](https://www.docker.com) - Container hosting platform
  - [Docker Compose](https://docs.docker.com/compose/) - Container orchestration utility
- [Kafka](https://kafka.apache.org/) - Message bus platform
  - [.NET Kafka Client](https://github.com/confluentinc/confluent-kafka-dotnet/) - Message bus platform client
- [FASTER](https://github.com/microsoft/FASTER) - Hybrid Key/Value Log
- [MediatR](https://github.com/jbogard/MediatR) - Event-Driven pub/sub mediator
- [React](https://reactjs.org) - Front-end Web UI framework
  - [Create React App](https://create-react-app.dev) - Application bootstrap tool
  - [Typescript](https://www.typescriptlang.org) - Primary UI programming language
  - [MUI](https://mui.com) - Material Design UI component library
- [Serilog](https://serilog.net/) - .NET Logging implementations
- [Swagger/OpenAPI](https://swagger.io/) - API explorer/UI tool
- [XUnit](https://github.com/xunit/visualstudio.xunit) - Unit tesing framework for .NET
- [Moq](https://github.com/moq/moq4) - Unit Test object mocking utility
<br/><br/>

## **5. <u>Prerequisites</u>**
<br/>

### **5.1 Web API, Twitter Stream Reader, Kafka, and Kafka Stream Reader**
<br/>

The preferred way to run the APIs and Readers is to launch them in their Docker containers. This requires that you have `Visual Studio 2022` (or any `.NET 7` and `Docker` compatible IDE), and assuming you are working on a Windows OS, `Docker Desktop` installed and running with `WSL 2` (comes with `Docker Desktop`) but may need to be updated. Free versions are avaiable for each of these.

- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
<br/><br/>

### **5.2 React Front-end UI**
<br/>

In order to run the React Front-end, Node and npm need to be installed. Both are freely available. I also highly recommend using Node Version Manager for this process, as it makes the process super simple.

- [Node Version Manager](https://github.com/nvm-sh/nvm)
  - [For Windows](https://github.com/coreybutler/nvm-windows)
- [NodeJS](https://nodejs.org/en/) (For reference only, should be installed through NVM)
<br/><br/>

## **6. <u>Structure</u>**
<br/>
Here is a very basic diagram of the three implementations:
<br/><br/>

<div style="max-width: 50%">

![Basic Desgin Diagram][basic_design_diagram]

</div>
<br/><br/>

## **7. <u>User Secrets and `SSL` Certs</u>**
<br/>

### **Twitter API keys**
<br/>

To access the Twitter Volume Stream you will need to have valid API keys placed into the user secrets of the `StreamReader` project.

Open `Manage User Secrets` for the project from the context menu:
<br/><br/>

<span style="padding-left: 3em">![][user_secrets_image]</span>
<br/><br/>

And use the following keys to edit the values to match your Twitter access tokens:
<br/><br/>

```json
{
    "InMemoryCredentialStore": {
        "ConsumerKey": "<PRIVATE_TWITTER_CONSUMER_KEY>",
        "ConsumerSecret": "<PRIVATE_TWITTER_CONSUMER_SECRET>"
    }
}
```

<br/><br/>

### **Self-signed (dev) `SSL` Certs**
<br/>

The `Docker` build process does include a `dev-cert SSL` certificate for each `API` to use for testing; however, this is for testing purposes only and would be replaced in production configurations.

The `dotnet` commandline tool provides a tool for creating self-signed certificates for developers to use on their local machines. I have included this functionality in the `Docker` file process so that the certificates are included in the images that are built. ___**This is not secure and uses a password that is plain text in all the docker files.**___ This is only for development and will not function on external systems. Additionally, because the certification is generated within the `Docker` image context, the certificate cannot be automatically trusted on the host machine, though it can be trusted manually or per browser session. 
<br/><br/>

```yaml
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=f9ea0a74-d7b3-49ae-b18b-25630bfbea10
.
.
.
RUN dotnet dev-certs https -ep /app/https/aspnetapp.pfx -p f9ea0a74-d7b3-49ae-b18b-25630bfbea10
```
<br/>

Alternatively, the `Docker` images could be made to point to a user's existing certificates, but I wanted the process to be as automated as possible with limited impact on the host machines.

When working with the application, you can ignore the `SSL` and use `HTTP` since I have not enforced `HTTPS` purposely. Alternatively, you can accept the certificate in your browser or add it to your trusted certificate store.
<br/><br/>

## **8. <u>Getting Started</u>**
<br/>

I have included all the necessary files and configurations for operating the application locally using `Docker Desktop` and `WSL` via the `Docker Compose` project. This excludes the sample React User Interface, which must be built and started manually. Instructions are included in the UI folder. ___**The yml and Dockerfiles are not configured for production - there is no real security implemented.**___ 
<br/><br/>

To get started, clone this repository to a location on your local machine.
<br/><br/>

### **Docker**
<br/>

The simplist way (i.e. "The Happy Path") to run the sample is to use the `Docker Compose` tool. This can be accessed either from the `Powershell` terminal or by using the `Developer Powershell` terminal built into Visual Studio.

From the terminal and within the `src` folder, use the following two commands to build and launch the `Docker` containers (they can be combined, but for ease of demonstration I have seperated them here).
<br/><br/>

```powershell
docker compose --profile all build --parallel --no-cache
docker compose --profile all up --detach
```
<br/>

You should see something similar to the following if your environment is properly set up with the latest `Docker Desktop`, `WSL 2`, `Visual Studio 2022`, and `.NET 7 SDK` as described in [**Primary Technologies and Packages**](#3-primary-technologies-and-packages). If you encounter an error accessing your local file system from within the containers, I found this [article](https://appuals.com/an-error-occurred-mounting-one-of-your-file-systems/) extremely helpful for addressing `WSL` as the cause.
<br/><br/><br/>

![][docker_desktop_image]
<br/><br/>

Assuming that everything is up and running now, you should see traffic between the containers as the `StreamReader` process pulls Tweets from the Twitter Volume Stream and sends them to each of the producers (`Kafka`, `Faster`, and `Simple` are enabled by default). This can be seen via the running logs within each respective container. Additionally, I have provided a number of ways to interact with the APIs to gain insights into their functioning.
<br/><br/>

### **Health Checks**

Each `API` has a very basic health check thread implemented that will respond with "Healthy" if it is responsive. They can be reached using the following URLs. (each `SSL` version requires accepting the `dev-cert` in your browser)

Simple API: http://localhost:4000/health, https://localhost:4001/health<br/>
Faster API: http://localhost:4100/health, https://localhost:4101/health<br/>
Kafka Tweet API: http://localhost:4200/health, https://localhost:4201/health
<br/><br/>

### **Direct API Access**

Each API is also accessible via the direct links to each operation. For instance, the count of Tweets can be see in the Simple API via http://localhost:4000/api/tweets/count
<br/><br/>

### **Swagger UI**

Most importantly, each API has Swagger UI enabled when running in `Docker`, giving an interface to interact with the API endpoints. Swagger UI can be reached at the following locations:

Simple API: http://localhost:4000/swagger<br/>
Faster API: http://localhost:4100/swagger<br/>
Kafka Tweet API: http://localhost:4200/swagger
<br/><br/>

### **Sample React Application**

Finally, there is a sample React JS UI application included to view all three APIs together, showing the total number of Tweets processed by each and the top 10 Hashtags captured. See the `README` included in the UI folder for instructrion on building and running the application.
<br/><br/>

![][react_sample_page]
<br/><br/>

## **9. <u>Clean-up</u>**
<br/>

When the solution is run within `Docker Desktop` the persisted data and logs are store in the users temporary storage in a folder called `Containers`. On Windows, this is in the %TEMP% directory. When run outside of `Docker Desktop`, some files will be written to the `c:/Temp` directory. Simply delete these folders when you no longer need them or wish to reset the data.
<br/><br/>

## **10. <u>Conclusion</u>**
<br/>

**TL/DR;** I find that despite the added complexity, `Docker` and `Kafka` seems to, unsurprisingly, provide the most benifits.

Going into the project, the plan was to demonstrate the use of `Docker` and `Kafka` to show the throught processes of having a scalable, performant, and well supported platform for deliverying high-volume applications. As I progressed into the project, I also decided that I wanted to also 1. demonstrate a simple in-memory solution that I believe was the minimum required to solve the problem statement and meet requires and 2. a hybrid solution that would add some persistence and scalability, but not rely on `Kafka` or `Docker`. This repository is the outcome of that decision. Having accomplished all three goals, I have satisfied myself, spent far more time than intended, "made a career" out of a simple programming exercise, and had a lot of fun. I would not do change it.

I believe that this project exercises all three solution quite well and is capable of processing millions of Tweets with minimal resources. I was actually extremely surprised at the overall performance of all three approaches. Obviously, the simple in-memory repository is simply a proof-of-concept and has no advantages (unexpectedly) over the other solutions. I had thought that a simple `ConcurrentDictionary` would outperform the hybrid solution until a certain memory threshold was reached, but I was not able to reach that threshhold to see that actually happen. I also expected the hybrid solution to suffer when performing counts and reads due to the nature of the solution; however, I only saw negligible latency when first loading counts or hashtags from a log that had been restored after a shutdown, that was quite large, and only on the very first access. There after, the read caching made it perform extremely quickly.. Due to that, I used it for the basis of the final `Kafka` solution and persist the data that is read from the consumer in a version of that repository. My original plan was to use the in-memory solution since the `Kafka` log could be re-played on each start-up, but the `FasterKV` solution provided a better answer in my opinion.

All of that being said, I think the ultimate solution is the full `Kafka` pipeline. It shows that it is very scalable, durable, and quite performant. I could fleshing that system out and making it into a real application that could handle magnitudes more data that the 1% sample from the volume stream. Truly not surprising; however, I am pleasantly surprised by the hybrid log and could see using that for caching mechinism for sure.

[user_secrets_image]: content/visual_studio_project_user_secrets.png
[docker_desktop_image]: content/Docker_Desktop_Running_All_Containers.png
[react_sample_page]: content/React_Sample_Page.png
[basic_design_diagram]: content/Basic_Design_Diagram.svg