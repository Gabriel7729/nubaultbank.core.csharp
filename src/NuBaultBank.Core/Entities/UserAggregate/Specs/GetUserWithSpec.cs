using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.UserAggregate.Specs;
public class GetUserWithSpec : Specification<User>, ISingleResultSpecification<User>
{
  public GetUserWithSpec(Guid userId)
  {
    Query
      .Where(user => user.Id == userId)
      .Include(user => user.Beneficiaries)
      .Include(user => user.Accounts)
      .Include(user => user.Loans);
  }
}
