# LoanCalculator
A simple ASP.NET Core web application for calculating housing loan costs. This project includes a backend API developed in C# and a frontend implemented as a Single Page Application (SPA). The application demonstrates loan cost calculations, extensibility for different loan types, and includes sample data handling using Entity Framework.

## Packages
In /api:
* dotnet restore

In /web:
* npm install

in /LoanCalculator.Tests:
* dotnet restore

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

## How to run unit test

* Run: dotnet test in /LoanCalculator.Tests
