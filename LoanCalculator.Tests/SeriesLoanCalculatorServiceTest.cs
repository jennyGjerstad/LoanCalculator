using LoanCalculator.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using LoanCalculator.Models;
using LoanCalculator.Services;
using LoanCalculator.Configurations;
using Microsoft.Extensions.Hosting;

namespace LoanCalculator.Tests;

public class SeriesLoanCalculatorServiceTest
{
    private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private DbContextOptions<Context> _dbContextOptions;
    private Context _context;
    private IConfiguration _configuration;
    private SeriesLoanCalculatorService _service;
    private LoanCalculationRequest _request;
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
        _service = new SeriesLoanCalculatorService(_context);

        SeedDatabase();

        _request = new LoanCalculationRequest
        {
            LoanAmount = 12000,
            PaybackTime = 1,
            LoanTypeId = 1,
            PaymentSchemeId = 1
        };
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
            new Loan { Type = "Housing Loan", InterestRate = 12, PaymentSchemes = [paymentScheme] }
        );

        _context.SaveChanges();
    }

    [Test]
    public async Task GetPaymentPlanAsync_ShouldReturnPaymentPlan()
    {
        // Act
        var result = await _service.GetPaymentPlanAsync(_request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<PaymentPlan>());

    }

    [Test]
    public async Task GetPaymentPlanAsync_ShouldReturnCorrectPaymentPlan()
    {
        // Arrange
        int expectedInterestAmount = 120;
        int expectedPrincipalAmount = 1000;
        int expectedTotalAmount = 1120;
        int expectedOutstanding = 11000;

        // Act
        var result = await _service.GetPaymentPlanAsync(_request);

        // Assert
        var paymentPlan = result;
        Assert.That(result, Has.Count.EqualTo(12));
        var paymentPlanList = paymentPlan.ToList();
        paymentPlanList.ForEach(payment => Assert.That((int)payment.PrincipalAmount, Is.EqualTo(expectedPrincipalAmount)));

        for (int i = 0; i < 12; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That((int)paymentPlanList[i].InterestAmount, Is.EqualTo(expectedInterestAmount));
                Assert.That((int)paymentPlanList[i].TotalAmount, Is.EqualTo(expectedTotalAmount));
                Assert.That((int)paymentPlanList[i].OutstandingAmount, Is.EqualTo(expectedOutstanding));
            });
            expectedInterestAmount -= 10;
            expectedTotalAmount -= 10;
            expectedOutstanding -= expectedPrincipalAmount;
        }
        Assert.That((int)paymentPlanList[11].OutstandingAmount, Is.EqualTo(0));
    }
}