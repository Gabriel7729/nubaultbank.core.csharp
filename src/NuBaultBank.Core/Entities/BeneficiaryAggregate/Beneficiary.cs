using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.BeneficiaryAggregate;
public class Beneficiary : EntityBase, IAggregateRoot
{
  public Guid UserId { get; set; }
  public Guid AccountId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty; 
  public string Email { get; set; } = string.Empty; 
  public string AccountNumber { get; set; } = string.Empty; 
  public string AccountType { get; set; } = string.Empty;
  public string BankName { get; set; } = string.Empty; 

}
