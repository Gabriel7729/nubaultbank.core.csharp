using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Specs;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.UserDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.AuthEndpoints;
public class SignUp : EndpointBaseAsync
.WithRequest<UserDto>
.WithActionResult<UserResponseDto>
{
  private readonly IRepository<User> _userRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;
  public SignUp(
  IRepository<User> userRepository,
  ILogService logService,
    IMapper mapper)
  {
    _userRepository = userRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpPost("/api/User")]
  [SwaggerOperation(
          Summary = "Sign Up a new user",
          Description = "Sign Up a new user",
          OperationId = "SignUp.User",
          Tags = new[] { "AuthEndpoints" })
  ]
  public override async Task<ActionResult<UserResponseDto>> HandleAsync([FromBody] UserDto request, CancellationToken cancellationToken = default)
  {
    try
    {
      GetIfUserAlreadyExitsSpec spec = new(request.IdNumber, request.Email);
      User? userExists = await _userRepository.GetBySpecAsync(spec, cancellationToken);
      if (userExists is not null)
        return BadRequest(Result<UserResponseDto>.Error("La cédula o el correo no se encuentran disponibles"));

      User user = _mapper.Map<User>(request);
      user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

      User? userResponse = await _userRepository.AddAsync(user, cancellationToken);
      await _logService.CreateLog($"{userResponse.Name} {userResponse.LastName}", "Usuario ha sido creado satisfactoriamente", ActionStatus.Success, userResponse.Id, cancellationToken: cancellationToken);

      UserResponseDto userResponseDto = _mapper.Map<UserResponseDto>(userResponse);
      var result = Result<UserResponseDto>.Success(userResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog($"{request.Name} {request.LastName}", "Error al crear la cuenta del usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<UserResponseDto>.Error(new string[] { "Ha ocurrido un error creando la cuenta del usuario", ex.Message }));
    }
  }
}
