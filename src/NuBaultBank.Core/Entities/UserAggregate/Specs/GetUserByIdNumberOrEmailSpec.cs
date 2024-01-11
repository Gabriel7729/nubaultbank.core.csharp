using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.UserAggregate.Specs;
public class GetUserByIdNumberOrEmailSpec : Specification<User>, ISingleResultSpecification
{
  public GetUserByIdNumberOrEmailSpec(string idNumberOrEmail)
  {
    Query.Where(user => user.IdNumber == idNumberOrEmail || user.Email == idNumberOrEmail);
  }
}
