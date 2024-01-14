using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.ProductAggregate.Specs;
public class GetAccountDetailsSpec : Specification<Account>, ISingleResultSpecification
{
  public GetAccountDetailsSpec(Guid accountId)
  {
    Query.Where(account => account.Id == accountId)
      .Include(x => x.Transfers);
  }
}

