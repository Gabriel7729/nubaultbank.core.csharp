using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.ProductAggregate.Specs;
using NuBaultBank.Core.Entities.TransferAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.TransferDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.TransferEndpoints;

public class ExpressTransfer : EndpointBaseAsync
  .WithRequest<ExpressTransferDto>
  .WithActionResult
{
  private readonly IRepository<Transfer> _transferRepository;
  private readonly IRepository<Account> _accountRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public ExpressTransfer(
    IRepository<Transfer> transferRepository,
    IRepository<Account> accountRepository,
    ILogService logService,
    IMapper mapper)
  {
    _transferRepository = transferRepository;
    _accountRepository = accountRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpPost("/api/Transfer/Express")]
  [SwaggerOperation(
          Summary = "Transfer Between accounts express",
          Description = "Transfer Between accounts express",
          OperationId = "Transfer.ExpressTransfer",
          Tags = new[] { "TransferEndpoints" })
  ]
  public override async Task<ActionResult> HandleAsync([FromBody] ExpressTransferDto request, CancellationToken cancellationToken = default)
  {
    try
    {
      Account? sourceAccount = await _accountRepository.GetByIdAsync(request.SourceAccountId, cancellationToken);
      if (sourceAccount == null)
        return BadRequest(Result<TransferResponseDto>.Error(new string[] { "La cuenta de origen no existe" }));

      GetAccountWithAccountNumberSpec spec = new(request.AccountDestinationNumber);
      Account? destinationAccount = await _accountRepository.GetBySpecAsync(spec, cancellationToken);
      if (destinationAccount == null)
        return BadRequest(Result<TransferResponseDto>.Error(new string[] { "La cuenta de destino no existe" }));

      await _accountRepository.BeginTransactionAsync(cancellationToken);

      sourceAccount.Withdraw(request.Amount);
      destinationAccount.Deposit(request.Amount);

      await _accountRepository.UpdateAsync(sourceAccount, cancellationToken);
      await _accountRepository.UpdateAsync(destinationAccount, cancellationToken);

      Transfer transfer = _mapper.Map<Transfer>(request);
      transfer.UpdateTransference();

      Transfer transferResponse = await _transferRepository.AddAsync(transfer, cancellationToken);
      await _logService.CreateLog(HttpContext, $"La transferencia desde la cuenta {sourceAccount.AccountNumber} a la cuenta {destinationAccount.AccountNumber} con el monto de {transferDto.Amount} ha sido realizada con exito", ActionStatus.Success, cancellationToken: cancellationToken);

      await _accountRepository.CommitTransactionAsync(cancellationToken);
      await _accountRepository.DisposeTransactionAsync();

      TransferResponseDto transferResponseDto = _mapper.Map<TransferResponseDto>(transferResponse);
      var result = Result<TransferResponseDto>.Success(transferResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _accountRepository.RollbackTransactionAsync(cancellationToken);
      await _logService.CreateLog(HttpContext, "Ha ocurrido un realizando la transferencia entre mis cuentas", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<TransferResponseDto>.Error(new string[] { "Ha ocurrido un realizando la transferencia entre mis cuentas" }));
    }
  }
}


