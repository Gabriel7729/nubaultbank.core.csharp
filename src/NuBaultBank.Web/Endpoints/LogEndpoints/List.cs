using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.LogAggregate;
using NuBaultBank.Core.Entities.LogAggregate.Specifications;
using NuBaultBank.Core.Models;
using NuBaultBank.Infrastructure.Dto.LogDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.LogEndpoints;

[Authorize]
public class List : EndpointBaseAsync
  .WithRequest<FilterLogQuery>
  .WithActionResult<PaginationResult<List<LogResponseDto>>>
{
  private readonly IRepository<Log> _logRepository;
  private readonly IMapper _mapper;
  public List(IRepository<Log> logRepository, IMapper mapper)
  {
    _logRepository = logRepository;
    _mapper = mapper;
  }
  [HttpGet("/api/Log/Paginated")]
  [SwaggerOperation(
         Summary = "List paginated Logs by branch",
         Description = "List paginated Logs by branch",
         OperationId = "ListPaginatedByBranch.Log",
         Tags = new[] { "LogEndpoints" })
 ]
  public override async Task<ActionResult<PaginationResult<List<LogResponseDto>>>> HandleAsync([FromQuery] FilterLogQuery query, CancellationToken cancellationToken = default)
  {
    try
    {
      GetAllLogsSpec spec = new(query);
      List<Log> logs = await _logRepository.ListAsync(spec, cancellationToken);
      List<LogResponseDto> logsResponseDto = _mapper.Map<List<LogResponseDto>>(logs);

      var paginatedInstance = new Paginate<Log, List<LogResponseDto>>(_logRepository);
      var paginatedList = await paginatedInstance.GetResponse(
        query.PageNumber!.Value,
        query.PageSize!.Value,
        logsResponseDto,
        new GetAllLogsSpec(query));

      var result = Result<PaginationResult<List<LogResponseDto>>>.Success(paginatedList);

      return Ok(result);
    }
    catch (Exception)
    {
      return BadRequest(Result<PaginationResult<List<LogResponseDto>>>.Error(new string[] { "An error has occurred getting the logs paginated" }));
    }
  }
}
