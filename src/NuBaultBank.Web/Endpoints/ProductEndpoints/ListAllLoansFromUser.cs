using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.ProductAggregate.Specs;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.ProductDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace NuBaultBank.Web.Endpoints.ProductEndpoints;

[Authorize]
public class ListAllLoansFromUser : EndpointBaseAsync
  .WithRequest<Guid>
  .WithActionResult<List<AccountResponseDto>>
{
  private readonly IRepository<Loan> _loanRepository;
  private readonly IMapper _mapper;
  private readonly ILogService _logService;

  public ListAllLoansFromUser(
      IRepository<Loan> loanRepository,
      IMapper mapper,
      ILogService logService)
  {
    _loanRepository = loanRepository;
    _mapper = mapper;
    _logService = logService;
  }

  [HttpGet("/api/Loan/User/{userId:Guid}")]
  [SwaggerOperation(
          Summary = "List all Loans from a user",
          Description = "List all Loans from a user",
          OperationId = "ListByUser.Loan",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<List<AccountResponseDto>>> HandleAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
  {
    try
    {
      GetLoansFromUserSpec spec = new(userId);
      List<Loan> loans = await _loanRepository.ListAsync(spec, cancellationToken);
      List<AccountResponseDto> loanResponseDto = _mapper.Map<List<AccountResponseDto>>(loans);

      await _logService.CreateLog(HttpContext, "Listado de prestamos de un usuario obtenida satisfactoriamente", ActionStatus.Success, cancellationToken: cancellationToken);

      var result = Result<List<AccountResponseDto>>.Success(loanResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error obteniendo los prestamos del usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<List<AccountResponseDto>>.Error(new string[] { "Ha ocurrido un error obteniendo los prestamos del usuario" }));
    }
  }
}


