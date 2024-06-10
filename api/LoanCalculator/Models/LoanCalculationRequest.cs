namespace LoanCalculator.Models;

public class LoanCalculationRequest
{
    public required int LoanAmount { get; set; }
    public required int PaybackTime { get; set; }
    public required int LoanTypeId { get; set; }
    public required int PaymentSchemeId { get; set; }
}