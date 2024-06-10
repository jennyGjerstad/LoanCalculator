using LoanCalculator.Models;

namespace LoanCalculator.MappingExtensions;

public static class PaymentschemeMappingExtensions
{
    public static PaymentSchemeDTO ToPaymentSchemeDTO(this PaymentScheme scheme)
    {
        return new()
        {
            Id = scheme.Id,
            DisplayValue = scheme.Name,
            Description = scheme.Description,
        };
    }
}