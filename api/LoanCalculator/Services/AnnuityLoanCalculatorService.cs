using LoanCalculator.Data;
using LoanCalculator.Models;

namespace LoanCalculator.Services;

public class AnnuityLoanCalculatorService(Context context) : ILoanCalculatorService
{
    public Task<PaymentPlan> GetPaymentPlanAsync(LoanCalculationRequest request)
    {
        throw new NotImplementedException();
    }
}