namespace NuBaultBank.Web.Endpoints.ProductEndpoints;

public class DepositAccountRequest
{
  public Guid AccountId { get; set; }
  public decimal Amount { get; set; }
}
