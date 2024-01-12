using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.ProductAggregate.Specs;
public class GetLoansFromUserSpec : Specification<Loan>
{
  public GetLoansFromUserSpec(Guid userId)
  {
    Query.Where(loan => loan.UserId == userId);
  }
}
