using NuBaultBank.SharedKernel.Interfaces;
using NuBaultBank.SharedKernel;
using NuBaultBank.Core.Enums;

namespace NuBaultBank.Core.Entities.ProductAggregate;
public class Loan : EntityBase, IAggregateRoot
{
  public double LoanAmount { get; private set; }
  public double AnnualInterestRate { get; private set; }
  public int LoanDurationMonths { get; private set; }
  public double MonthlyPayment { get; private set; }
  public LoanStatus Status { get; private set; }
  public Guid UserId { get; set; }

  public void AddLoanData(double loanAmount, int loanDurationMonths)
  {
    LoanAmount = loanAmount;
    LoanDurationMonths = loanDurationMonths;
  }
  public void StartLoan(double annualInterestRate) 
  {
    AnnualInterestRate = annualInterestRate;
    MonthlyPayment = CalculateMonthlyPayment();
    Status = LoanStatus.Active;
  }
  private double CalculateMonthlyPayment()
  {
    double monthlyInterestRate = AnnualInterestRate / 12 / 100;
    return LoanAmount * monthlyInterestRate / (1 - Math.Pow(1 + monthlyInterestRate, -LoanDurationMonths));
  }
}
