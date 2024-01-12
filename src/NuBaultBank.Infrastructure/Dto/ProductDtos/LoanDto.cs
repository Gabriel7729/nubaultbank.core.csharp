using NuBaultBank.Core.Enums;
using NuBaultBank.Infrastructure.Dto.TransferDtos;

namespace NuBaultBank.Infrastructure.Dto.ProductDtos;
public class LoanDto
{
  public double LoanAmount { get; set; }
  public int LoanDurationMonths { get; set; }
  public Guid UserId { get; set; }
}

public class ApproveLoanRequestDto
{
  public Guid LoanId { get; set; }
  public double AnnualInterestRate { get; set; }
}

public class LoanResponseDto : BaseResponseDto
{
  public double LoanAmount { get;  set; }
  public double AnnualInterestRate { get;  set; }
  public int LoanDurationMonths { get;  set; }
  public double MonthlyPayment { get;  set; }
  public LoanStatus Status { get;  set; }
  public Guid UserId { get; set; }

  public ICollection<PaymentResponseDto> Payments { get; set; } = new List<PaymentResponseDto>();
}
