using LoanCalculator.Models;

namespace LoanCalculator.MappingExtensions;
public static class LoanMappingExtentions
{
    public static LoanDTO ToLoanDTO(this Loan loan)
    {
        return new() {
            Id = loan.Id,
            DisplayValue = loan.Type,
            Value = loan.InterestRate
        };
    }
}