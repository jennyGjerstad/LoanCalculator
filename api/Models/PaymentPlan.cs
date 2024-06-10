using LoanCalculator.Utils;

namespace LoanCalculator.Models;

public class Payment
{
    public Month Month { get; set; }
    public double TotalAmount { get; set;}
    public double InterestAmount { get; set; }
    public double PrincipalAmount { get; set; }
    public double OutstandingAmount { get; set; }
}

public class PaymentPlan : List<Payment>
{ }