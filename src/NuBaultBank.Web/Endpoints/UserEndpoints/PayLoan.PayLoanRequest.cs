namespace NuBaultBank.Web.Endpoints.UserEndpoints;

public class PayLoanRequest
{
  public Guid UserId { get; set; }
  public Guid AccountId { get; set; }
  public Guid LoanId { get; set; }
}
