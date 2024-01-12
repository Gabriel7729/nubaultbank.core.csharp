using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.ProductDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.ProductEndpoints;

public class RequestLoan : EndpointBaseAsync
  .WithRequest<LoanDto>
  .WithActionResult<LoanResponseDto>
{
  private readonly IRepository<Loan> _loanRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public RequestLoan(
    IRepository<Loan> loanRepository,
    ILogService logService,
    IMapper mapper)
  {
    _loanRepository = loanRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpPost("/api/Loan/Request")]
  [SwaggerOperation(
          Summary = "Create a request for a loan to a user",
          Description = "Create a request for a loan to a user",
          OperationId = "Loan.Request",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<LoanResponseDto>> HandleAsync([FromBody] LoanDto loanDto, CancellationToken cancellationToken = default)
  {
    try
    {
      Loan loan = new() { UserId = loanDto.UserId};
      loan.AddLoanData(loanDto.LoanAmount, loanDto.LoanDurationMonths);

      Loan loanResponse = await _loanRepository.AddAsync(loan, cancellationToken);
      await _logService.CreateLog(HttpContext, "El prestamo ha sido solicitado con exito", ActionStatus.Success, cancellationToken: cancellationToken);

      LoanResponseDto loanResponseDto = _mapper.Map<LoanResponseDto>(loanResponse);
      var result = Result<LoanResponseDto>.Success(loanResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error solicitando el prestamo", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<LoanResponseDto>.Error(new string[] { "Ha ocurrido un error solicitando el prestamo" }));
    }
  }
}

