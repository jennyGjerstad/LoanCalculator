using LoanCalculator.Data;
using LoanCalculator.Factories;
using LoanCalculator.MappingExtensions;
using LoanCalculator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanCalculator.Controllers;

[Route("loans")]
public class LoanController(Context context, LoanCalculatorFactory loanCalculatorFactory) : ControllerBase
{
    private readonly LoanCalculatorFactory _loanCalculatorFactory = loanCalculatorFactory;
    private readonly Context _context = context;

    [HttpGet]
    public async Task<IActionResult> GetLoans()
    {
        var loans = await _context.LoanTypes
            .ToListAsync();
        
        return Ok(loans.Select(loan => loan.ToLoanDTO()));
    }

    [HttpGet("{loanId}")]
    public async Task<IActionResult> GetLoan(int loanId)
    {
        var loan = await _context.LoanTypes
                   .Where( l => l.Id == loanId )
                   .FirstOrDefaultAsync();
        if (loan == null ) return NotFound($"Could not find any loan type with id: {loanId}");
        return Ok(loan.ToLoanDTO());
    }

    [HttpGet("{loanId}/paymentschemes")]
    public async Task<IActionResult> GetLoanSchemes(int loanId)
    {
        var loan = await _context.LoanTypes.Include(l => l.PaymentSchemes)
                   .Where( l => l.Id == loanId )
                   .FirstOrDefaultAsync();
        if (loan == null ) return NotFound($"Could not find any loan type with id: {loanId}");

        var schemes = loan.PaymentSchemes.Select(sc => sc.ToPaymentSchemeDTO());

        return Ok(schemes);
    }
    
    [HttpPost("table")]
    public async Task<IActionResult> GetLoanTable([FromBody] LoanCalculationRequest request)
    {
        PaymentPlan paymentPlan;
        try
        {
            var loan = await _context.LoanTypes.FirstOrDefaultAsync(loantype => loantype.Id == request.LoanTypeId)
                ?? throw new Exception($"Could not find loan type {request.LoanTypeId}");

            var loanService = _loanCalculatorFactory.GetLoanCalculatorService(request.PaymentSchemeId);
            paymentPlan = await loanService.GetPaymentPlanAsync(request);
            return Ok(paymentPlan);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            return BadRequest(ex);
        }
    }
}