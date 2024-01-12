using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.ProductAggregate.Specs;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Services;
public class ProductService : IProductService
{
  private readonly IRepository<Account> _accountRepository;
  public ProductService(IRepository<Account> accountRepository)
  {
    _accountRepository = accountRepository;
  }

  public async Task<bool> IsAnUniqueAccountAsync(string accountNumber)
  {
    GetIfAnAccountAlreadyExistsSpec spec = new GetIfAnAccountAlreadyExistsSpec(accountNumber);
    Account? account = await _accountRepository.GetBySpecAsync(spec);
    return account is null;
  }
}
