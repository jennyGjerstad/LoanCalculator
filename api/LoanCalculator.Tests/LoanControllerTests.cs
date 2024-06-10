using LoanCalculator.Controllers;
using LoanCalculator.Data;
using LoanCalculator.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using LoanCalculator.Models;
using Microsoft.AspNetCore.Mvc;
using LoanCalculator.Configurations;

namespace LoanCalculator.Tests;

public class LoanControllerTests
{
        private LoanCalculatorFactory _loanCalculatorFactory;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private LoanController _loanController;
        private DbContextOptions<Context> _dbContextOptions;
        private Context _context;
        private IConfiguration _configuration;
        private DbOptions _dbOptions;
    
    [SetUp]
    public void Setup()
    {
        _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        _mockWebHostEnvironment.Setup(env => env.EnvironmentName).Returns("Staging");

        // Load test configuration from appsettings.Test.json
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .Build();

        _dbOptions = _configuration.GetSection("DbConfiguration")?.Get<DbOptions>() ??
                    throw new Exception("Could not load DbConfiguration");

        _dbContextOptions = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _context = new Context(_dbContextOptions, _mockWebHostEnvironment.Object, _dbOptions);

        _loanCalculatorFactory = new LoanCalculatorFactory(_context);
        _loanController = new LoanController(_context, _loanCalculatorFactory);

        SeedDatabase();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedDatabase()
    {
        var paymentScheme = new PaymentScheme()
        {
            Id = 1,
            Name = "Series Loan",
            Description = "A series loan is a payment scheme where the customer pays back an equal principal amount with each installment, along with recurring interest charges calculated each month."
        };

        _context.LoanTypes.AddRange(
            new Loan { Type = "Housing Loan", InterestRate = 3.5, PaymentSchemes = [paymentScheme] },
            new Loan { Type = "Car Loan", InterestRate = 1 }
        );

        _context.SaveChanges();
    }

    [Test]
    public async Task GetLoans_ShouldReturnOkResult_WithLoanDTOs()
    {
        // Act
        var result = await _loanController.GetLoans();
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var loans = okResult.Value as IEnumerable<LoanDTO>;
        Assert.That(loans, Is.Not.Null);
        Assert.That(loans.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetLoanById_ShouldReturnOkResult_AsLoanDTO()
    {
        // Act - loanId = 1
        var result_1 = await _loanController.GetLoan(1);
        Assert.That(result_1, Is.InstanceOf<OkObjectResult>());
        var okResult_1 = result_1 as OkObjectResult;
        Assert.That(okResult_1, Is.Not.Null);
        var loan_1 = okResult_1.Value as LoanDTO;
        Assert.That(loan_1, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(loan_1.Id, Is.EqualTo(1));
            Assert.That(loan_1.DisplayValue, Is.EqualTo("Housing Loan"));
            Assert.That(loan_1.Value, Is.EqualTo(3.5));
        });

        // Act - loanId = 2
        var result_2 = await _loanController.GetLoan(2);
        Assert.That(result_2, Is.InstanceOf<OkObjectResult>());
        var okResult_2 = result_2 as OkObjectResult;
        Assert.That(okResult_2, Is.Not.Null);
        var loan_2 = okResult_2.Value as LoanDTO;
        Assert.That(loan_2, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(loan_2.Id, Is.EqualTo(2));
            Assert.That(loan_2.DisplayValue, Is.EqualTo("Car Loan"));
            Assert.That(loan_2.Value, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GetPaymentSchemesByLoanId_ShouldReturnOkResult_WithPaymentSchemeDTO()
    {
        // Act
        var result = await _loanController.GetLoanSchemes(1);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var schemes = okResult.Value as IEnumerable<PaymentSchemeDTO>;
        Assert.That(schemes, Is.Not.Null);
        Assert.That(schemes.Count, Is.EqualTo(1));
        var scheme = schemes.First();
        Assert.Multiple(() =>
        {
            Assert.That(scheme.Id, Is.EqualTo(1));
            Assert.That(scheme.DisplayValue, Is.EqualTo("Series Loan"));
        });
    }

    [Test]
    public async Task GetLoanTableFromCalculationRequest_ShouldReturnOkResult_WithPaymentPlan()
    {
        // Arrange
        var request = new LoanCalculationRequest {
            LoanAmount = 1000,
            PaybackTime = 1,
            LoanTypeId = 1,
            PaymentSchemeId = 1
        };

        // Act
        var result = await _loanController.GetLoanTable(request);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var loanTable = okResult.Value as PaymentPlan;
        Assert.That(loanTable, Is.Not.Null);
        Assert.That(loanTable.Count, Is.EqualTo(12));
    }
}