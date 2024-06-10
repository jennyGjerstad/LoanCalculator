using LoanCalculator.Utils;
using LoanCalculator.Data;
using LoanCalculator.Models;
using Microsoft.EntityFrameworkCore;


namespace LoanCalculator.Services;


public class SeriesLoanCalculatorService(Context context) : ILoanCalculatorService
{
    private Context _context = context;
    public async Task<PaymentPlan> GetPaymentPlanAsync(LoanCalculationRequest request)
    {
        var scheme = await _context.PaymentSchemes.FirstOrDefaultAsync(sc => sc.Id == request.PaymentSchemeId) ??
            throw new Exception($"Paymentscheme not configured: {nameof(SeriesLoanCalculatorService)}");

        var loan = await _context.LoanTypes.FirstOrDefaultAsync(loan => loan.Id == request.LoanTypeId) ??
            throw new Exception($"Could not find loan with id: {request.LoanTypeId}");


        var paymentPlan = new PaymentPlan();
        var interestRate = loan.InterestRate;
        var installments = request.PaybackTime * 12;
        var principalAmount = request.LoanAmount / (double)installments;
        var outstandingAmount = (double)request.LoanAmount;
        for (int i = 0; i < installments; i++)
        {
            Month month = (Month)(i % 12);
            var interestAmount = outstandingAmount * (interestRate / 12) / 100;
            var totalAmount = principalAmount + interestAmount;
            outstandingAmount = outstandingAmount + interestAmount - totalAmount;
            if (outstandingAmount < 0)
            {
                totalAmount +=  outstandingAmount;
                principalAmount +=  outstandingAmount;
                outstandingAmount = 0;
            }
            // If we have reached the amount of installments, but there are still
            // outstanding payment left we add it to the principal amount
            if (i == installments - 1 && outstandingAmount != 0) {
                principalAmount += outstandingAmount;
                outstandingAmount = 0;
            }
            var payment = new Payment
            {
                Month = month,
                PrincipalAmount = principalAmount,
                InterestAmount = interestAmount,
                TotalAmount = totalAmount,
                OutstandingAmount = outstandingAmount
            };
            paymentPlan.Add(payment);
        }
        return paymentPlan;
    }
}