using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.ProductAggregate.Specs;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.ProductDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.ProductEndpoints;

[Authorize]
public class GetLoanDetails : EndpointBaseAsync
  .WithRequest<Guid>
  .WithActionResult<LoanResponseDto>
{
  private readonly IRepository<Loan> _loanRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public GetLoanDetails(
    IRepository<Loan> loanRepository,
    ILogService logService,
    IMapper mapper)
  {
    _loanRepository = loanRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpGet("/api/Loan/{loanId:Guid}")]
  [SwaggerOperation(
          Summary = "Get loan details",
          Description = "Get loan details",
          OperationId = "GetLoan.Details",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<LoanResponseDto>> HandleAsync([FromRoute] Guid loanId, CancellationToken cancellationToken = default)
  {
    try
    {
      GetLoanDetailsSpec spec = new(loanId);
      Loan? loan = await _loanRepository.GetBySpecAsync(spec, cancellationToken);
      if (loan is null)
        return NotFound(Result<LoanResponseDto>.Error(new string[] { $"El prestamo no fue encontrado" }));

      await _logService.CreateLog(HttpContext, "Se han obtenido el detalle del prestamo correctamente", ActionStatus.Success, cancellationToken: cancellationToken);

      LoanResponseDto loanResponseDto = _mapper.Map<LoanResponseDto>(loan);
      var result = Result<LoanResponseDto>.Success(loanResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error obteniendo el detalle del prestamo", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<LoanResponseDto>.Error(new string[] { "Ha ocurrido un error obteniendo el detalle del prestamo" }));
    }
  }
}

