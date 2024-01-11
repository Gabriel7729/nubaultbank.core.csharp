using NuBaultBank.Core.Models;

namespace NuBaultBank.Core.Interfaces;
public interface IAuthService
{
  Task<UserLoginResponseDto> LoginAsync(string idNumberOrEmail, string password, CancellationToken cancellationToken = default);
  Task<string> RefreshTokenAsync(string token, CancellationToken cancellationToken = default);
  TokenClaim DecryptToken(string token);
}
