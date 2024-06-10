using Microsoft.EntityFrameworkCore;
using LoanCalculator.Models;
using LoanCalculator.Configurations;

namespace LoanCalculator.Data;

public class Context : DbContext
{
    private readonly DbOptions _dbOptions;
    private readonly IWebHostEnvironment _environment;
    public Context(DbContextOptions<Context> options,
                   IWebHostEnvironment environment,
                   DbOptions dbOptions) : base(options)
    {
        _environment = environment;
        _dbOptions = dbOptions;

        if (!_environment.IsProduction())
        {
            EnsureDatabaseExist();
        }
    }

    public DbSet<Loan> LoanTypes { get; set; }
    public DbSet<PaymentScheme> PaymentSchemes { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loan>(loantype => {
            loantype.ToTable("LoanTypes");
            loantype.HasMany(l => l.PaymentSchemes).WithMany(p => p.Loans);
            loantype.HasKey(l => l.Id);
        });

        modelBuilder.Entity<PaymentScheme>(scheme => {
            scheme.ToTable("PaymentSchemes");
        });
    }

    public async void EnsureDatabaseExist()
    {
        try
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
        } catch (Exception ex)
        {
            Console.WriteLine($"Error when ensuring database exists: {ex.Message}");
        }
        if (_dbOptions.SeedDatabase)
        {
            try
            {
                var paymentScheme = new PaymentScheme() {
                    Id = 1,
                    Name = "Series Loan",
                    Description = "A series loan is a payment scheme where the customer pays back an equal principal amount with each installment, along with recurring interest charges calculated each month."
                };

                await LoanTypes.AddRangeAsync(
                    new Loan { Type = "Housing Loan", InterestRate = 3.5, PaymentSchemes = [paymentScheme] },
                    new Loan { Type = "Car Loan", InterestRate = 3.5 }
                );

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when ensuring database exists: {ex.Message}");
            }
        }
    }
}