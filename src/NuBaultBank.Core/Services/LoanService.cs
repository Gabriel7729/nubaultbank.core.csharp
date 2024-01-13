using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Services;
internal class LoanService : ILoanService
{
  private readonly IRepository<User> _repository;
  public LoanService(IRepository<User> repository)
  {
    _repository = repository;
  }
  public async Task PayLoan(User user, Account account, Loan loan)
  {
    try
    {
      account.Withdraw((decimal)loan.MonthlyPayment);
      loan.MakePayment();
      await _repository.UpdateAsync(user);
    }
    catch (ArgumentException ex)
    {
      // Funds not above 0
      throw new NotImplementedException();
    }
    catch(InvalidOperationException ex)
    {
      // Not enough funds
      throw new NotImplementedException();
    }
  }
}
