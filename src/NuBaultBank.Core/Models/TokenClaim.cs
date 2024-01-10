namespace NuBaultBank.Core.Models;
public class TokenClaim
{
  public Guid BranchId { get; set; }
  public Guid UserId { get; set; }
  public string UserName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public bool IsGeneralAdmin { get; set; }
}
