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
public class CreateAccount : EndpointBaseAsync
  .WithRequest<AccountDto>
  .WithActionResult<AccountResponseDto>
{
  private readonly IRepository<Account> _accountRepository;
  private readonly IProductService _productService;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public CreateAccount(
    IRepository<Account> accountRepository,
    IProductService productService,
    ILogService logService,
    IMapper mapper)
  {
    _accountRepository = accountRepository;
    _productService = productService;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpPost("/api/Account")]
  [SwaggerOperation(
          Summary = "Create an account to a user",
          Description = "Create an account to a user",
          OperationId = "Account.Create",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<AccountResponseDto>> HandleAsync([FromBody] AccountDto accountDto, CancellationToken cancellationToken = default)
  {
    try
    {
      Account? accountToExtractBalance = await _accountRepository.GetByIdAsync(accountDto.AccountExtractBalance.AccountId, cancellationToken);
      if (accountToExtractBalance is null)
        return NotFound(Result<AccountResponseDto>.Error(new string[] { $"La cuenta desde donde se va a extraer el dinero para crear la cuenta no existe" }));

      await _accountRepository.BeginTransactionAsync(cancellationToken);

      accountToExtractBalance.Withdraw(accountDto.AccountExtractBalance.Amount);
      await _accountRepository.UpdateAsync(accountToExtractBalance, cancellationToken);

      Account account = new()
      {
        UserId = accountDto.UserId,
        AccountType = AccountType.Savings,
        AccountNumber = await Account.GenerateAccountNumberAsync(_productService)
      };
      account.Deposit(accountDto.AccountExtractBalance.Amount);
      await _accountRepository.AddAsync(account, cancellationToken);

      await _accountRepository.CommitTransactionAsync(cancellationToken);
      await _accountRepository.DisposeTransactionAsync();

      await _logService.CreateLog(HttpContext, "La cuenta de ahorro ha sido creara con exito", ActionStatus.Success, cancellationToken: cancellationToken);
      AccountResponseDto accountResponseDto = _mapper.Map<AccountResponseDto>(account);
      var result = Result<AccountResponseDto>.Success(accountResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _accountRepository.RollbackTransactionAsync(cancellationToken);
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error creando la cuenta del usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<AccountResponseDto>.Error(new string[] { "Ha ocurrido un error creando la cuenta del usuario" }));
    }
  }
}

