namespace NuBaultBank.Web.Endpoints.AuthEndpoints;

public class LoginRequest
{
  public string IdNumberOrEmail { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
