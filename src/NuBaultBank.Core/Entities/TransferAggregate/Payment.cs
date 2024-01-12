using NuBaultBank.SharedKernel.Interfaces;
using NuBaultBank.SharedKernel;

namespace NuBaultBank.Core.Entities.TransferAggregate;
public class Payment : EntityBase, IAggregateRoot
{
  public Guid LoanId { get; set; }
  public decimal Amount { get; set; }
  public string Concept { get; set; } = string.Empty;
  public DateTime PaymentDate { get; set; }
}
