using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.ProductDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.ProductEndpoints;

public class DepositAccount : EndpointBaseAsync
  .WithRequest<DepositAccountRequest>
  .WithActionResult<AccountResponseDto>
{
  private readonly IRepository<Account> _accountRepository;
  private readonly ILogService _logService;

  public DepositAccount(
    IRepository<Account> accountRepository,
    ILogService logService)
  {
    _accountRepository = accountRepository;
    _logService = logService;
  }

  [HttpPatch("/api/Account/Deposit")]
  [SwaggerOperation(
          Summary = "Deposit a specific amount to an account",
          Description = "Deposit a specific amount to an account",
          OperationId = "Account.Deposit",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<AccountResponseDto>> HandleAsync([FromBody] DepositAccountRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      Account? account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
      if (account is null)
        return NotFound(Result<AccountResponseDto>.Error(new string[] { $"La cuenta no existe" }));

      account.Deposit(request.Amount);
      await _accountRepository.UpdateAsync(account, cancellationToken);

      await _accountRepository.CommitTransactionAsync(cancellationToken);
      await _accountRepository.DisposeTransactionAsync();

      await _logService.CreateLog(HttpContext, $"Se ha depositado el monto de {request.Amount} a la cuenta {account.AccountNumber}", ActionStatus.Success, cancellationToken: cancellationToken);
      var result = Result<GeneralResponse>.Success(new GeneralResponse(true, $"Se ha depositado el monto de {request.Amount} a la cuenta {account.AccountNumber}"));
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error depositando el monto a la cuenta", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<AccountResponseDto>.Error(new string[] { "Ha ocurrido un error depositando el monto a la cuenta" }));
    }
  }
}


