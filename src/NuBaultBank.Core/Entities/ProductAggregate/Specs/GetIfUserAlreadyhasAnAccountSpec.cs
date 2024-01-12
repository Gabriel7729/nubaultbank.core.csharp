using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.ProductAggregate.Specs;
public class GetIfUserAlreadyhasAnAccountSpec : Specification<Account>, ISingleResultSpecification
{
  public GetIfUserAlreadyhasAnAccountSpec(Guid userId)
  {
    Query.Where(account => account.UserId == userId);
  }
}
