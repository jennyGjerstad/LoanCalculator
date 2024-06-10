using LoanCalculator.Models;

namespace LoanCalculator.Services;

public interface ILoanCalculatorService
{
    public Task<PaymentPlan> GetPaymentPlanAsync(LoanCalculationRequest request);
}