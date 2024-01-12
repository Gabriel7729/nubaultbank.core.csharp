using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Events;
using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.UserAggregate;
public class User : EntityBase, IAggregateRoot
{
  public string Name { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string IdNumber { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public bool IsActive { get; set; } = false;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public ICollection<Account> Accounts { get; set; } = new List<Account>();
  public ICollection<Loan> Loans { get; set; } = new List<Loan>();

  public void CallEventUserCreatedAddCheckingAccount()
  {
    RegisterDomainEvent(new UserCreatedAddCheckingAccount(this));
  }
}
