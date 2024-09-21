# StealTheCatsProject

## Overview
StealTheCatsProject is an ASP.NET Core Web API application that interacts with the Cats as a Service API to retrieve cat images and store them in a database. The project includes a database schema for cats, tags, and their relationships, along with unit tests and Swagger API documentation.

## Prerequisites

Before you start, ensure you have the following installed on your local machine:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Git](https://git-scm.com/)
- [Visual Studio Code](https://code.visualstudio.com/) or any other preferred IDE
- [Postman](https://www.postman.com/downloads/) or [curl](https://curl.se/) (for testing APIs)

## Clone the Repository

Clone the repository from GitHub to your local machine.

git clone https://github.com/gkoulieri/StealTheCatsProject.git

cd StealTheCatsProject


## Setting Up the Database
1. Configure SQL Server:

Ensure SQL Server is installed and running.
Update the appsettings.json file in the StealTheCatsAPI project to point to your SQL Server instance. You may need to adjust the ConnectionStrings section based on your local SQL Server setup.

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CatDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}



## Building and Running the Application

1.Restore Dependencies: Navigate to the StealTheCatsAPI directory and restore all dependencies using the .NET CLI.

cd StealTheCatsAPI
dotnet restore

2.Build the Project: Build the project to ensure everything compiles correctly.

dotnet build

3.Run the Application: Run the ASP.NET Core Web API application.

dotnet run

The API will be available at https://localhost:7063 

4.Access Swagger Documentation: Once the API is running, you can access the Swagger documentation at:

https://localhost:7063/swagger

This will display all available endpoints and allow you to test them directly.

## Running Unit Tests
Navigate to the StealTheCatsAPITests directory and run the unit tests using the .NET CLI.

cd StealTheCatsAPITests
dotnet test

## Conclusion
This README provides all the necessary steps to set up and run the StealTheCatsProject locally. Feel free to reach out if you have any questions or need further clarifications.

