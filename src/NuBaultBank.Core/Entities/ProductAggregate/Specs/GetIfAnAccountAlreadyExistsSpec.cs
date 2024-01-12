using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.ProductAggregate.Specs;
public class GetIfAnAccountAlreadyExistsSpec : Specification<Account>, ISingleResultSpecification
{
  public GetIfAnAccountAlreadyExistsSpec(string accountNumber)
  {
    Query.Where(account => account.AccountNumber == accountNumber);
  }
}

