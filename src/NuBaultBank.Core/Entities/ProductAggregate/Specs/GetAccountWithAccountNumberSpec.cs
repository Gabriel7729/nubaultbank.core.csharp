using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.ProductAggregate.Specs;
public class GetAccountWithAccountNumberSpec : Specification<Account>, ISingleResultSpecification<Account>
{
  public GetAccountWithAccountNumberSpec(string accountNumber)
  {
    Query.Where(account => account.AccountNumber == accountNumber);
  }
}
