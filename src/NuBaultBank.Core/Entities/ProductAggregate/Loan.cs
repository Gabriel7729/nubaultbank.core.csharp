using NuBaultBank.SharedKernel.Interfaces;
using NuBaultBank.SharedKernel;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Entities.TransferAggregate;

namespace NuBaultBank.Core.Entities.ProductAggregate;
public class Loan : EntityBase, IAggregateRoot
{
  public double LoanAmount { get; private set; }
  public double AnnualInterestRate { get; private set; }
  public double LoanAmountWithInterest
  {
    get
    {
      // Based on a simple interest
      double interest = LoanAmount * AnnualInterestRate * (LoanDurationMonths / 12.0);
      return LoanAmount + interest;
    }
  }

  public int LoanDurationMonths { get; private set; }
  public double MonthlyPayment { get; private set; }
  public LoanStatus  Status { get; private set; }
  public Guid UserId { get; set; }

  public ICollection<Payment> Payments { get; set; } = new List<Payment>();

  public void AddLoanData(double loanAmount, int loanDurationMonths)
  {
    LoanAmount = loanAmount;
    LoanDurationMonths = loanDurationMonths;
  }
  public void MakePayment()
  {
    // Deduct the payment amount from the loan
    LoanAmount -= MonthlyPayment;
    // Add the payment to the payment history
    Payments.Add(new Payment { 
      Amount = (decimal)MonthlyPayment, 
      PaymentDate = DateTime.Now,
      UserId = UserId,
    });
    // Update the status of the loan if it's fully paid
    if (LoanAmount <= 0)
    {
      Status = LoanStatus.PaidOff;
    }
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
