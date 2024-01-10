using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Specs;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Core.Models;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Infrastructure.Services;
public class AuthService : IAuthService
{
  private readonly IRepository<User> _userRepository;
  private readonly IConfiguration _config;

  public AuthService(
    IRepository<User> userRepository,
    IConfiguration config)
  {
    _userRepository = userRepository;
    _config = config;
  }

  public async Task<UserLoginResponseDto> LoginAsync(string idNumberOrEmail, string password, CancellationToken cancellationToken = default)
  {
    GetUserByIdNumberOrEmailSpec spec = new(idNumberOrEmail);
    var user = await _userRepository.GetBySpecAsync(spec, cancellationToken)
      ?? throw new Exception($"The credentials you entered are invalid");

    if (!ValidPassword(user.Password, password))
      throw new Exception("The credentials you've entered are invalid");

    if (!user.IsActive)
      throw new Exception("Your account has not been active");

    return new UserLoginResponseDto()
    {
      Name = user.Name,
      LastName = user.LastName,
      Token = GenerateToken(user)
    };
  }
  public async Task<string> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
  {
    var tokenDeserialized = DeserializeToken(token);

    if (tokenDeserialized.ValidTo < DateTime.UtcNow)
      throw new AuthenticationException("The refresh token has expired.");

    string email = tokenDeserialized.Claims.Where(x => x.Type == "email").FirstOrDefault()!.Value
      ?? throw new Exception($"The email was not found in the token");

    GetUserByIdNumberOrEmailSpec spec = new(email);
    User? user = await _userRepository.GetBySpecAsync(spec, cancellationToken)
      ?? throw new Exception($"The user was not found");

    return GenerateToken(user);
  }
  public TokenClaim DecryptToken(string token)
  {
    IEnumerable<Claim> claims = DeserializeToken(token).Claims;

    TokenClaim tokenClaim = new();

    foreach (var claim in claims)
    {
      switch (claim.Type)
      {
        case "userId":
          tokenClaim.UserId = Guid.Parse(claim.Value);
          break;
        case "email":
          tokenClaim.Email = claim.Value;
          break;
        case "role":
          tokenClaim.Role = claim.Value;
          break;
        case "userName":
          tokenClaim.UserName = claim.Value;
          break;
        default:
          break;
      }
    }

    return tokenClaim;
  }

  private static JwtSecurityToken DeserializeToken(string token)
  {
    var manejadorJwtToken = new JwtSecurityTokenHandler();

    if (manejadorJwtToken.ReadToken(token) is not JwtSecurityToken tokenDeserializado)
      throw new Exception("The token is not valid");

    return tokenDeserializado;
  }
  private string GenerateToken(User user)
  {
    var secretKey = Encoding.ASCII.GetBytes(_config["JWT_SECRET"]);
    var expiresMinutes = int.Parse(_config["JWT_EXPIRES_MINUTES"]);

    var claims = new ClaimsIdentity(new[] {
                new Claim("userName", user.Name + " " + user.LastName),
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("idNumber", user.IdNumber),
    });
    var signInCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = claims,
      Expires = DateTime.Now.AddMinutes(expiresMinutes),
      NotBefore = DateTime.Now,
      SigningCredentials = signInCredentials,
      Issuer = _config["JWT_ISSUER"],
      Audience = _config["JWT_AUDIENCE"]
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }
  private static bool ValidPassword(string encryptedPassword, string password)
  {
    return BCrypt.Net.BCrypt.Verify(password, encryptedPassword);
  }
}
