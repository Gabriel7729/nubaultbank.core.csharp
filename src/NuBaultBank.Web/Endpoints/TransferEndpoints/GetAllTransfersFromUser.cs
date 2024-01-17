using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.TransferAggregate;
using NuBaultBank.Core.Entities.TransferAggregate.Specs;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.TransferDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.TransferEndpoints;

[Authorize]
public class GetAllTransfersFromUser : EndpointBaseAsync
  .WithRequest<Guid>
  .WithActionResult<List<TransferResponseDto>>
{
  private readonly IRepository<Transfer> _transferRepository;
  private readonly ILogService _logService;
  private readonly IMapper _mapper;

  public GetAllTransfersFromUser(
    IRepository<Transfer> transferRepository,
    ILogService logService,
    IMapper mapper)
  {
    _transferRepository = transferRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpGet("/api/Transfer/User/{userId:Guid}")]
  [SwaggerOperation(
          Summary = "Transfer Between accounts express",
          Description = "Transfer Between accounts express",
          OperationId = "Transfer.ListByUser",
          Tags = new[] { "TransferEndpoints" })
  ]
  public override async Task<ActionResult<List<TransferResponseDto>>> HandleAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
  {
    try
    {
      GetTransferFromUserSpec spec = new(userId);
      List<Transfer> transfer = await _transferRepository.ListAsync(spec, cancellationToken);
      if (transfer == null)
        return BadRequest(Result<List<TransferResponseDto>>.Error(new string[] { "No se pudieron obtener las transferencias del usuario" }));

      List<TransferResponseDto> transferResponseDtos = _mapper.Map<List<TransferResponseDto>>(transfer);
      await _logService.CreateLog(HttpContext, $"Se han consultado las transferencias del usuario con exito", ActionStatus.Success, cancellationToken: cancellationToken);
      var result = Result<List<TransferResponseDto>>.Success(transferResponseDtos);
      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un realizando la transferencia entre mis cuentas", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<List<Transfer>>.Error(new string[] { "Ha ocurrido un realizando la transferencia entre mis cuentas" }));
    }
  }
}
