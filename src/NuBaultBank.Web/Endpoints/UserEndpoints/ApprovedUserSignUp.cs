using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.ProductAggregate.Specs;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.UserDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.UserEndpoints;

public class ApprovedUserSignUp : EndpointBaseAsync
.WithRequest<Guid>
.WithActionResult<GeneralResponse>
{
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<Account> _accountRepository;
  private readonly ILogService _logService;
  public ApprovedUserSignUp(
  IRepository<User> userRepository,
  IRepository<Account> accountRepository,
  ILogService logService)
  {
    _userRepository = userRepository;
    _accountRepository = accountRepository;
    _logService = logService;
  }

  [HttpPatch("/api/User/{userId:Guid}/Approve")]
  [SwaggerOperation(
          Summary = "Approve User Account",
          Description = "Approve User Account",
          OperationId = "ApproveAccount.User",
          Tags = new[] { "UserEndpoints" })
  ]
  public override async Task<ActionResult<GeneralResponse>> HandleAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
  {
    try
    {
      User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
      if (user is null)
        return BadRequest(Result<GeneralResponse>.Error("El usuario no fue encontrado"));

      if(user.IsActive)
        return BadRequest(Result<GeneralResponse>.Error("El usuario ya se encuentra activo"));

      GetIfUserAlreadyhasAnAccountSpec spec = new(userId);
      Account? accountUser = await _accountRepository.GetBySpecAsync(spec, cancellationToken);
      if(accountUser is null)
        user.CallEventUserCreatedAddCheckingAccount();

      user.IsActive = true;

      await _userRepository.UpdateAsync(user, cancellationToken);
      await _logService.CreateLog(HttpContext, "Usuario ha sido habilitando con exito", ActionStatus.Success, user.Id, cancellationToken: cancellationToken);

      var result = Result<GeneralResponse>.Success(new GeneralResponse(true, "Usuario ha sido habilitado con exito"));
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error al activar el usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<UserResponseDto>.Error(new string[] { "Ha ocurrido un error al activar el usuario", ex.Message }));
    }
  }
}
