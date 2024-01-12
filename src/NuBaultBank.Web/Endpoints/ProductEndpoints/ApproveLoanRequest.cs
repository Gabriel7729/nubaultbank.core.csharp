using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.ProductDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.ProductEndpoints;

[Authorize]
public class ApproveLoanRequest : EndpointBaseAsync
  .WithRequest<ApproveLoanRequestDto>
  .WithActionResult<LoanResponseDto>
{
  private readonly IRepository<Loan> _loanRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public ApproveLoanRequest(
    IRepository<Loan> loanRepository,
    ILogService logService,
    IMapper mapper)
  {
    _loanRepository = loanRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpPatch("/api/Loan/Request/Approve")]
  [SwaggerOperation(
          Summary = "Approve the request of the Loan",
          Description = "Approve the request of the Loan",
          OperationId = "Loan.RequestApprove",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<LoanResponseDto>> HandleAsync([FromBody] ApproveLoanRequestDto loanRequest, CancellationToken cancellationToken = default)
  {
    try
    {
      Loan? loan = await _loanRepository.GetByIdAsync(loanRequest.LoanId, cancellationToken);
      if (loan is null)
        return BadRequest(Result<LoanResponseDto>.Error(new string[] { "El prestamo no existe" }));

      loan.StartLoan(loanRequest.AnnualInterestRate);

      Loan loanResponse = await _loanRepository.AddAsync(loan, cancellationToken);
      await _logService.CreateLog(HttpContext, "El prestamo ha sido Aprobado con exito", ActionStatus.Success, cancellationToken: cancellationToken);

      LoanResponseDto loanResponseDto = _mapper.Map<LoanResponseDto>(loanResponse);
      var result = Result<LoanResponseDto>.Success(loanResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error aprobando el prestamo", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<LoanResponseDto>.Error(new string[] { "Ha ocurrido un error aprobando el prestamo" }));
    }
  }
}
