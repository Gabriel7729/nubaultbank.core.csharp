using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Specs;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.UserDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.UserEndpoints;

[Authorize]
public class ListPaginated : EndpointBaseAsync
  .WithRequest<RequestPagination>
  .WithActionResult<PaginationResult<List<UserResponseDto>>>
{
  private readonly IRepository<User> _userRepository;
  private readonly IMapper _mapper;
  private readonly ILogService _logService;

  public ListPaginated(
      IRepository<User> userRepository,
      IMapper mapper,
      ILogService logService)
  {
    _userRepository = userRepository;
    _mapper = mapper;
    _logService = logService;
  }

  [HttpGet("/api/User/Paginated")]
  [SwaggerOperation(
          Summary = "List all users paginated",
          Description = "List all users paginated",
          OperationId = "ListPaginated.User",
          Tags = new[] { "UserEndpoints" })
  ]
  public override async Task<ActionResult<PaginationResult<List<UserResponseDto>>>> HandleAsync([FromQuery] RequestPagination request, CancellationToken cancellationToken = default)
  {
    try
    {
      GetPaginateUsersSpec spec = new(request.PageNumber, request.PageSize);
      var users = await _userRepository.ListAsync(spec, cancellationToken);
      var userResponseDto = _mapper.Map<List<UserResponseDto>>(users);

      var paginatedInstance = new Paginate<User, List<UserResponseDto>>(_userRepository);
      var paginatedList = await paginatedInstance.GetResponse(
        request.PageNumber,
        request.PageSize,
        userResponseDto,
        spec);

      await _logService.CreateLog(HttpContext, "Listado de usuarios paginados obtenidos correctamente", ActionStatus.Success, cancellationToken: cancellationToken);  // Add this line

      var result = Result<PaginationResult<List<UserResponseDto>>>.Success(paginatedList);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error obteniendo el listado de usuarios paginados", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<PaginationResult<List<UserResponseDto>>>.Error(new string[] { "Ha ocurrido un error obteniendo el listado de usuarios paginados" }));
    }
  }
}
