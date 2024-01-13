using NuBaultBank.Core.Enums;

namespace NuBaultBank.Infrastructure.Dto.TransferDtos;
public class TransferDto
{
  public Guid SourceAccountId { get; set; }
  public Guid DestinationAccountId { get; set; }
  public decimal Amount { get; set; }
  public string Concept { get; set; } = string.Empty;
  public Guid UserId { get; set; }
}

public class TransferResponseDto : BaseResponseDto
{
  public Guid SourceAccountId { get; set; }
  public Guid DestinationAccountId { get; set; }
  public decimal Amount { get; set; }
  public string Concept { get; set; } = string.Empty;
  public DateTime TransferDate { get; set; } = DateTime.UtcNow;
  public TransactionStatus Status { get; set; }

  public Guid UserId { get; set; }
}
