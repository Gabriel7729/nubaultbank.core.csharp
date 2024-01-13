using NuBaultBank.Core.Enums;
using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.TransferAggregate;
public class Transfer : EntityBase, IAggregateRoot
{
  public Transfer()
  {
    Reference = CreateTransferReference();
  }
  public string Reference { get; set; } = string.Empty;
  public Guid SourceAccountId { get; set; }
  public Guid DestinationAccountId { get; set; }
  public decimal Amount { get; set; }
  public string Concept { get; set; } = string.Empty;
  public DateTime TransferDate { get; set; } = DateTime.UtcNow;
  public TransactionStatus Status { get; set; }
  public Guid UserId { get; set; }
  private string CreateTransferReference()
  {
    string dateTimePart = DateTime.Now.ToString("yyyyMMddHHmmss");
    string guidPart = Guid.NewGuid().ToString("N");
    return $"TR-{dateTimePart}-{guidPart}";
  }
  public void UpdateTransference()
  {
    CreateTransferReference();
    Status = TransactionStatus.Completed;
  }
}
