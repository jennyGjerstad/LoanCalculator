namespace LoanCalculator.Models;

public class Loan
{
    public static string TableName => "LoanTypes";
    public int Id { get; set; }
    public required string Type { get; set; }
    public required double InterestRate { get; set; }
    public virtual ICollection<PaymentScheme> PaymentSchemes { get; set; } = [];
}

public class LoanDTO
{
    public int Id { get; set; }
    public string DisplayValue { get; set; } = string.Empty;
    public double Value { get; set; }
}