using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.AuthEndpoints;

[AllowAnonymous]
public class Login : EndpointBaseAsync
  .WithRequest<LoginRequest>
  .WithActionResult<LoginResponse>
{
  private readonly IAuthService _authService;
  private readonly ILogService _logService;

  public Login(
    IAuthService authService,
    ILogService logService)
  {
    _authService = authService;
    _logService = logService;
  }
  [HttpPost("/api/Auth/Login")]
  [SwaggerOperation(
  Summary = "Login a user",
  Description = "Login a user",
  OperationId = "Login.User",
          Tags = new[] { "AuthEndpoints" })
  ]
  public override async Task<ActionResult<LoginResponse>> HandleAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
  {
    UserLoginResponseDto? responeAuthDto = null;

    try
    {
      responeAuthDto = await _authService.LoginAsync(request.IdNumberOrEmail, request.Password, cancellationToken);
      LoginResponse response = new(responeAuthDto.Token);

      await _logService.CreateLog($"{responeAuthDto.Name} {responeAuthDto.LastName}", "Usuario ha iniciado sesión correctamente", ActionStatus.Success, cancellationToken: cancellationToken);
      return Ok(Result<LoginResponse>.Success(response));
    }
    catch (Exception ex)
    {
      string userName = responeAuthDto != null ? (responeAuthDto.Name + responeAuthDto.LastName) : "Unknown User";
      await _logService.CreateLog(userName, "Ha ocurrido un error en el inicio de sesión del usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);

      return BadRequest(Result<LoginResponse>.Error(new string[] { ex.Message }));
    }
  }
}

