using MediatR;
using NuBaultBank.Core.Entities.UserAggregate.Events;
using NuBaultBank.Core.Interfaces;

namespace NuBaultBank.Core.Entities.UserAggregate.Handlers;
public class UserCreatedAddCheckingAccountHandler : INotificationHandler<UserCreatedAddCheckingAccount>
{
  private readonly ILogService _logService;

  public UserCreatedAddCheckingAccountHandler(ILogService logService)
  {
    _logService = logService;
  }

  public async Task Handle(UserCreatedAddCheckingAccount domainEvent, CancellationToken cancellationToken)
  {
    // TODO: Add checking account to user
  }
}
