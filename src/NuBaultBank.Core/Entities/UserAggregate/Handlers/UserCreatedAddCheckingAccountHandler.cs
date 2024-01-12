using MediatR;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Events;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.UserAggregate.Handlers;
public class UserCreatedAddCheckingAccountHandler : INotificationHandler<UserCreatedAddCheckingAccount>
{
  private readonly IRepository<Account> _accountRepository;
  private readonly ILogService _logService;
  private readonly IProductService _productService;

  public UserCreatedAddCheckingAccountHandler(
    IRepository<Account> accountRepository,
    ILogService logService,
    IProductService productService)
  {
    _logService = logService;
    _accountRepository = accountRepository;
    _productService = productService;
  }

  public async Task Handle(UserCreatedAddCheckingAccount domainEvent, CancellationToken cancellationToken)
  {
    Account account = new()
    {
      UserId = domainEvent.User.Id,
      AccountNumber = await Account.GenerateAccountNumberAsync(_productService)
    };
    await _accountRepository.AddAsync(account, cancellationToken);
    await _logService.CreateLog($"{domainEvent.User.Name} {domainEvent.User.LastName}", "La cuenta ha sido agregada ", ActionStatus.Success, domainEvent.User.Id, cancellationToken: cancellationToken);
  }
}
