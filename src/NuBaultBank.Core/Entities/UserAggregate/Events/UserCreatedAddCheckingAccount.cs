using MediatR;
using NuBaultBank.SharedKernel;

namespace NuBaultBank.Core.Entities.UserAggregate.Events;
public class UserCreatedAddCheckingAccount : DomainEventBase, IRequest<User>
{
  public User User { get; set; }
  public UserCreatedAddCheckingAccount(User user)
  {
    User = user;
  }
}
