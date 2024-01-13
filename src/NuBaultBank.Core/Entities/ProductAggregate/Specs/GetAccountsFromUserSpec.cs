using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.ProductAggregate.Specs;
public class GetAccountsFromUserSpec : Specification<Account>
{
  public GetAccountsFromUserSpec(Guid userId)
  {
    Query.Where(account => account.UserId == userId);
  }
}

