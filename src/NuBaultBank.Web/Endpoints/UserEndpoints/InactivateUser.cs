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
public class InactivateUser : EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult<GeneralResponse>
{
  private readonly IRepository<User> _userRepository;
  private readonly ILogService _logService;

  public InactivateUser(
      IRepository<User> userRepository,
      ILogService logService)
  {
    _userRepository = userRepository;
    _logService = logService;
  }

  [HttpPatch("/api/User/{userId:Guid}/Inactivate")]
  [SwaggerOperation(
          Summary = "Inactivate User Account",
          Description = "Inactivates a User Account",
          OperationId = "InactivateAccount.User",
          Tags = new[] { "UserEndpoints" })
  ]
  public override async Task<ActionResult<GeneralResponse>> HandleAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
  {
    try
    {
      User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
      if (user is null)
        return BadRequest(Result<GeneralResponse>.Error("El usuario no fue encontrado"));

      if (!user.IsActive)
        return BadRequest(Result<GeneralResponse>.Error("El usuario ya se encuentra inactivo"));

      user.IsActive = false;

      await _userRepository.UpdateAsync(user, cancellationToken);
      await _logService.CreateLog(HttpContext, "Usuario ha sido inhabilitado con exito", ActionStatus.Success, user.Id, cancellationToken: cancellationToken);

      var result = Result<GeneralResponse>.Success(new GeneralResponse(true, "Usuario ha sido inhabilitado con exito"));
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error al inactivar el usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<GeneralResponse>.Error(new string[] { "Ha ocurrido un error al inactivar el usuario", ex.Message }));
    }
  }
}
