using LoanCalculator.Data;
using LoanCalculator.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using LoanCalculator.Tests.Utils;
using LoanCalculator.Models;
using LoanCalculator.Services;

namespace LoanCalculator.Tests;

public class LoanCalculatorFactoryTests
{
    private LoanCalculatorFactory _loanCalculatorFactory;
    private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private DbContextOptions<Context> _dbContextOptions;
    private Context _context;
    private IConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
        _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        _mockWebHostEnvironment.Setup(env => env.EnvironmentName).Returns(CustomEnvironments.Test);

        // Load test configuration from appsettings.Test.json
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .Build();

        _dbContextOptions = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _context = new Context(_dbContextOptions, _mockWebHostEnvironment.Object, _configuration);

        _loanCalculatorFactory = new LoanCalculatorFactory(_context);

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
        var paymentScheme_1 = new PaymentScheme()
        {
            Id = 1,
            Name = "Series Loan",
            Description = "A series loan is a payment scheme where the customer pays back an equal principal amount with each installment, along with recurring interest charges calculated each month."
        };
        var paymentScheme_2 = new PaymentScheme()
        {
            Id = 2,
            Name = "Annuity Loan",
            Description = "An annuity loan is a payment scheme where the customer pays back an equal total amount with each installment."
        };
        _context.PaymentSchemes.AddRange(
            paymentScheme_1, paymentScheme_2
        );
;

        _context.LoanTypes.AddRange(
            new Loan { Type = "Housing Loan", InterestRate = 3.5, PaymentSchemes = [paymentScheme_1] },
            new Loan { Type = "Housing Loan", InterestRate = 3.5, PaymentSchemes = [paymentScheme_2] }
        );

        _context.SaveChanges();
    }

    [Test]
    public void GetLoanCalculatorService_ReturnsCorrectLoanCalculator()
    {
        // Act
        var seriesLoan = _loanCalculatorFactory.GetLoanCalculatorService(1);
        var annuityLoan = _loanCalculatorFactory.GetLoanCalculatorService(2);

        // Assert
        Assert.Multiple(() =>
        {

            Assert.That(seriesLoan, Is.InstanceOf<SeriesLoanCalculatorService>());
            Assert.That(annuityLoan, Is.InstanceOf<AnnuityLoanCalculatorService>());
        });
    }
}