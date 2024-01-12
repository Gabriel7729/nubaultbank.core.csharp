using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.TransferAggregate;
public class Transfer : EntityBase, IAggregateRoot
{
  public Guid SourceAccountId { get; set; }
  public Guid DestinationAccountId { get; set; }
  public decimal Amount { get; set; }
  public string Concept { get; set; } = string.Empty;
  public DateTime TransferDate { get; set; } = DateTime.UtcNow;
}
