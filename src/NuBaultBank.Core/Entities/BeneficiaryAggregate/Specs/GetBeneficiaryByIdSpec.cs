using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using NuBaultBank.Core.Entities.UserAggregate;

namespace NuBaultBank.Core.Entities.BeneficiaryAggregate.Specs;
public class GetBeneficiaryByIdSpec : Specification<Beneficiary>
{
  public GetBeneficiaryByIdSpec(Guid beneficiaryId)
  {
    Query.Where(beneficiary => beneficiary.Id == beneficiaryId);
  }


}
