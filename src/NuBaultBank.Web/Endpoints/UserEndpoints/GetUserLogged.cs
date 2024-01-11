using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Core.Models;
using NuBaultBank.Infrastructure.Dto.UserDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.UserEndpoints;

[Authorize]
public class GetUserLogged : EndpointBaseAsync
  .WithoutRequest
  .WithActionResult<UserResponseDto>
{
  private readonly IAuthService _authService;
  private readonly IRepository<User> _userRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public GetUserLogged(
    IAuthService authService,
    IRepository<User> userRepository,
    ILogService logService,
    IMapper mapper)
  {
    _authService = authService;
    _userRepository = userRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpGet("/api/User/Logged")]
  [SwaggerOperation(
          Summary = "Get actual user logged",
          Description = "Get actual user logged",
          OperationId = "GetUser.Logged",
          Tags = new[] { "UserEndpoints" })
  ]
  public override async Task<ActionResult<UserResponseDto>> HandleAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
      TokenClaim claims = _authService.DecryptToken(token);

      User? user = await _userRepository.GetByIdAsync(claims.UserId, cancellationToken);
      if (user is null)
        return NotFound(Result<UserResponseDto>.Error(new string[] { $"The user was not found" }));

      await _logService.CreateLog(HttpContext, "Se han obtenido los datos del usuario logeado correctamente", ActionStatus.Success, cancellationToken: cancellationToken);

      UserResponseDto userResponseDto = _mapper.Map<UserResponseDto>(user);
      var result = Result<UserResponseDto>.Success(userResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error obteniendo los datos del usuario logeado", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<UserResponseDto>.Error(new string[] { "An error has occurred getting the user logged" }));
    }
  }
}

