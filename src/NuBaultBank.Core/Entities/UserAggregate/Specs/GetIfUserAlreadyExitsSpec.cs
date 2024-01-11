using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.UserAggregate.Specs;
public class GetIfUserAlreadyExitsSpec : Specification<User>, ISingleResultSpecification
{
  public GetIfUserAlreadyExitsSpec(string idNumber, string email)
  {
    Query.Where(user => user.Email == email || user.IdNumber == idNumber);
  }
}
