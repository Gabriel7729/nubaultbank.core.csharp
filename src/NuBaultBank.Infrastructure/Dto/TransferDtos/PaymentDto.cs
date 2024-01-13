using NuBaultBank.Core.Enums;

namespace NuBaultBank.Infrastructure.Dto.TransferDtos;
public class PaymentDto
{
}

public class PaymentResponseDto : BaseResponseDto
{
  public Guid LoanId { get; set; }
  public decimal Amount { get; set; }
  public string Concept { get; set; } = string.Empty;
  public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
  public TransactionStatus Status { get; set; }

  public Guid UserId { get; set; }
}
