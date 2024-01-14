using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using NuBaultBank.Core.Entities.UserAggregate;

namespace NuBaultBank.Core.Entities.BeneficiaryAggregate.Specs;
public class GetIfUserAlreadyHasABeneficiarySpec : Specification<User>
{
  public class GetIfUserAlreadyhasABeneficiarySpec : Specification<User>, ISingleResultSpecification
  {
    public GetIfUserAlreadyhasABeneficiarySpec(Guid userId, string email)
    {
      Query
          .Where(user => user.Id == userId)
          .Include(user => user.Beneficiaries)
          .Where(user => user.Beneficiaries.Any(beneficiary => beneficiary.Email == email));
    }
  }
}
