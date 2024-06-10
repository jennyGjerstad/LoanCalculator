using LoanCalculator.Data;
using LoanCalculator.Services;

namespace LoanCalculator.Factories;

public class LoanCalculatorFactory(Context context)
{
    public ILoanCalculatorService GetLoanCalculatorService(int paymentSchemeId)
    => paymentSchemeId switch
    {
        1 => new SeriesLoanCalculatorService(context),
        2 => new AnnuityLoanCalculatorService(context),
        _ => throw new Exception($"Unsupported payment scheme: {paymentSchemeId}"),
    };
}