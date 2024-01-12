using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Specs;
using NuBaultBank.Core.Enums;
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

  public ListPaginated(
    IRepository<User> userRepository,
    IMapper mapper)
  {
    _userRepository = userRepository;
    _mapper = mapper;
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

      var result = Result<PaginationResult<List<UserResponseDto>>>.Success(paginatedList);
      return Ok(result);
    }
    catch (Exception)
    {
      return BadRequest(Result<PaginationResult<List<UserResponseDto>>>.Error(new string[] { "An error has occurred getting all users" }));
    }
  }
}
