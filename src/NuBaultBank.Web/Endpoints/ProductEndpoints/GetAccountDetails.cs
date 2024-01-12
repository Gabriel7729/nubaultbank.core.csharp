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
public class GetAccountDetails : EndpointBaseAsync
  .WithRequest<Guid>
  .WithActionResult<AccountResponseDto>
{
  private readonly IRepository<Account> _accountRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public GetAccountDetails(
    IRepository<Account> accountRepository,
    ILogService logService,
    IMapper mapper)
  {
    _accountRepository = accountRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpGet("/api/Account/{accountId:Guid}")]
  [SwaggerOperation(
          Summary = "Get account details",
          Description = "Get account details",
          OperationId = "GetLoan.Details",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<AccountResponseDto>> HandleAsync([FromRoute] Guid accountId, CancellationToken cancellationToken = default)
  {
    try
    {
      GetAccountDetailsSpec spec = new(accountId);
      Account? account = await _accountRepository.GetBySpecAsync(spec, cancellationToken);
      if (account is null)
        return NotFound(Result<AccountResponseDto>.Error(new string[] { $"La cuenta no fue encontrado" }));

      await _logService.CreateLog(HttpContext, "Se ha obtenido el detalle de la cuenta correctamente", ActionStatus.Success, cancellationToken: cancellationToken);

      AccountResponseDto accountResponseDto = _mapper.Map<AccountResponseDto>(account);
      var result = Result<AccountResponseDto>.Success(accountResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error obteniendo el detalle de la cuenta", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<AccountResponseDto>.Error(new string[] { "Ha ocurrido un error obteniendo el detalle de la cuenta" }));
    }
  }
}
