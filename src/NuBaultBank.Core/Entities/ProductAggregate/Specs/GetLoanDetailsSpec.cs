using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.ProductAggregate.Specs;
public class GetLoanDetailsSpec : Specification<Loan>, ISingleResultSpecification
{
  public GetLoanDetailsSpec(Guid loanId)
  {
    Query.Where(loan => loan.Id == loanId)
      .Include(loan => loan.Payments);
  }
}

