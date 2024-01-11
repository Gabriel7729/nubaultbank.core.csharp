using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.AuthEndpoints;

[Authorize]
public class DecryptToken : EndpointBaseAsync
  .WithoutRequest
  .WithActionResult<TokenClaim>
{
  private readonly IAuthService _authService;
  private readonly ILogService _logService;

  public DecryptToken(
    IAuthService authService,
    ILogService logService)
  {
    _authService = authService;
    _logService = logService;
  }

  [HttpPost("/api/Auth/Decrypt/Token")]
  [SwaggerOperation(
          Summary = "Decrypt a token",
          Description = "Decrypt a token",
          OperationId = "Decrypt.Token",
          Tags = new[] { "AuthEndpoints" })
  ]
  public override async Task<ActionResult<TokenClaim>> HandleAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

      TokenClaim claims = _authService.DecryptToken(token);
      var result = Result<TokenClaim>.Success(claims);

      await _logService.CreateLog(HttpContext, "Token descifrado exitosamente", ActionStatus.Success, cancellationToken: cancellationToken);

      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Error al descifrar el token", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<TokenClaim>.Error(new string[] { ex.Message }));
    }
  }
}

