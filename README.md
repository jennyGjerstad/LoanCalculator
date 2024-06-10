# LoanCalculator
A simple ASP.NET Core web application for calculating housing loan costs. This project includes a backend API developed in C# and a frontend implemented as a Single Page Application (SPA). The application demonstrates loan cost calculations, extensibility for different loan types, and includes sample data handling using Entity Framework.

## Versions
* api: net8.0
* web: angular 18

## Packages
In /api:
* dotnet restore

In /web:
* npm install

## Configuring API

* Go to appsettings.json or appsetting.Development.json

  "API": {
    "ApiUrl": "http://hosturl:port"
  },
  "DbConfiguration": {
    "DbType": "SQLITE | SQLSERVER",
    "ConnectionString": "connectionstring"
  }

EXAMPLE:

  "API": {
    "ApiUrl": "http://localhost:5240"
  },
  "DbConfiguration": {
    "DbType": "SQLITE",
    "ConnectionString": "Datasource=C:/temp/loancalculator.db"
  }

## Configuring Web Client

* Go to src/assets/appsettings.json

    "apiUrl": "http://apiurl:port"

    EXAMPLE:

    "apiUrl": "http://localhost:5240"

## How to run API
Go to /api/LoanCalculator and run:
* dotnet run

## How to run WEB
Go to /web and run:
* npm start

## How to run unit test
Go to /api/LoanCalculator.Tests and run:
* dotnet test
