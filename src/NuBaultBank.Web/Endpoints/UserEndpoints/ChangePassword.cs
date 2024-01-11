using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.UserEndpoints;

[Authorize]
public class ChangePassword : EndpointBaseAsync
  .WithRequest<ChangePasswordRequest>
  .WithActionResult<GeneralResponse>
{
  private readonly IRepository<User> _userRepository;
  private readonly ILogService _logService;

  public ChangePassword(
    IRepository<User> userRepository,
    ILogService logService)
  {
    _userRepository = userRepository;
    _logService = logService;
  }

  [HttpPatch("/api/User/{userId:Guid}/Password/Change")]
  [SwaggerOperation(
          Summary = "Change User Password",
          Description = "Change User Password",
          OperationId = "ChangePassword.User",
          Tags = new[] { "UserEndpoints" })
  ]
  public override async Task<ActionResult<GeneralResponse>> HandleAsync([FromRoute] ChangePasswordRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
      if (user is null)
        return NotFound(Result<GeneralResponse>.Error("The user was not found"));

      bool isUserPasswordValid = BCrypt.Net.BCrypt.Verify(request.Body.OldPassword, user.Password);
      if (!isUserPasswordValid)
        return BadRequest(Result<GeneralResponse>.Error("The password is not valid"));

      user.Password = BCrypt.Net.BCrypt.HashPassword(request.Body.NewPassword);
      await _userRepository.UpdateAsync(user, cancellationToken);

      await _logService.CreateLog(HttpContext, "La contraseña ha sido actualizada correctamente", ActionStatus.Success, cancellationToken: cancellationToken);

      var result = Result<GeneralResponse>.Success(new GeneralResponse(true, "The password was changed successfully"));
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error cambiando la contraseña del usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<GeneralResponse>.Error(new string[] { "An error has occurred changing the user password", ex.Message }));
    }
  }
}

