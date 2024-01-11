namespace NuBaultBank.Core.Models;
public class TokenClaim
{
  public Guid UserId { get; set; }
  public string UserName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string IdNumber { get; set; } = string.Empty;
}
