namespace LoanCalculator.Models;

public class PaymentScheme
{
    public static string TableName => "PaymentSchemes";
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual ICollection<Loan> Loans { get; set; } = [];
}

public class PaymentSchemeDTO
{
    public int Id { get; set; }
    public string DisplayValue { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}