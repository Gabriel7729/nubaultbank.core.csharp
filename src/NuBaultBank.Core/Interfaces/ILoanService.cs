using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.UserAggregate;

namespace NuBaultBank.Core.Interfaces;
public interface ILoanService
{
  public Task PayLoan(User user, Account account, Loan loan);
}
