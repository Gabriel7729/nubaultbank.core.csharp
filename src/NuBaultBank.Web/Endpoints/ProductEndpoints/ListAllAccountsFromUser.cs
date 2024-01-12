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
public class ListAllAccountsFromAccount : EndpointBaseAsync
  .WithRequest<Guid>
  .WithActionResult<List<AccountResponseDto>>
{
  private readonly IRepository<Account> _accountRepository;
  private readonly IMapper _mapper;
  private readonly ILogService _logService;

  public ListAllAccountsFromAccount(
      IRepository<Account> accountRepository,
      IMapper mapper,
      ILogService logService)
  {
    _accountRepository = accountRepository;
    _mapper = mapper;
    _logService = logService;
  }

  [HttpGet("/api/Account/User/{userId:Guid}")]
  [SwaggerOperation(
          Summary = "List all Accounts from a user",
          Description = "List all Accounts from a user",
          OperationId = "ListByUser.Account",
          Tags = new[] { "ProductEndpoints" })
  ]
  public override async Task<ActionResult<List<AccountResponseDto>>> HandleAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
  {
    try
    {
      GetAccountsFromUserSpec spec = new(userId);
      List<Account> accounts = await _accountRepository.ListAsync(spec, cancellationToken);
      List<AccountResponseDto> accountResponseDto = _mapper.Map<List<AccountResponseDto>>(accounts);

      await _logService.CreateLog(HttpContext, "Listado de cuentas de un usuario obtenida satisfactoriamente", ActionStatus.Success, cancellationToken: cancellationToken);

      var result = Result<List<AccountResponseDto>>.Success(accountResponseDto);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error obteniendo las cuentas del usuario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<List<AccountResponseDto>>.Error(new string[] { "Ha ocurrido un error obteniendo las cuentas del usuario" }));
    }
  }
}

