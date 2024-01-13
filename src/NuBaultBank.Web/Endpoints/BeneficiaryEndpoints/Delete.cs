using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.BeneficiaryAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Specs;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.BeneficiaryDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.BeneficiaryEndpoints;

[Authorize]
public class Delete : EndpointBaseAsync
    .WithRequest<DeleteRequest>
    .WithActionResult<BeneficiaryResponseDTO>
{
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<Beneficiary> _beneficiaryRepository;
  private readonly IMapper _mapper;
  private readonly ILogService _logService;

  public Delete(IRepository<User> userRepository, IRepository<Beneficiary> beneficiaryRepository, IMapper mapper, ILogService logService)
  {
    _userRepository = userRepository;
    _beneficiaryRepository = beneficiaryRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpDelete("/api/Beneficiary/")]
  [SwaggerOperation(
      Summary = "Delete Beneficiary from User Account",
      Description = "Delete Beneficiary from User Account",
      OperationId = "BeneficiaryEndpoints.Delete",
      Tags = new[] { "BeneficiaryEndpoints" })
  ]
  public override async Task<ActionResult<BeneficiaryResponseDTO>> HandleAsync([FromQuery] DeleteRequest requestDto, CancellationToken cancellationToken = default)
  {
    try
    {
      GetUserWithSpec spec = new(requestDto.UserId);
      User? user = await _userRepository.GetBySpecAsync(spec, cancellationToken);
      if (user is null)
        return BadRequest(Result<BeneficiaryResponseDTO>.Error("El usuario no fue encontrado"));

      Beneficiary? beneficiary = user.Beneficiaries.First(beneficiary => beneficiary.Id == requestDto.BeneficiaryId);
      if(beneficiary is null)
        return BadRequest(Result<BeneficiaryResponseDTO>.Error($"El beneficiario con el id {requestDto.BeneficiaryId} no existe o no pertecene al usuario."));

      // Delete beneficiary from user
      await _beneficiaryRepository.DeleteAsync(beneficiary, cancellationToken);

      var result = Result<string>.Success($"Se ha eliminado el beneficiario {requestDto.UserId} del usuario {requestDto.UserId}");

      await _logService.CreateLog(HttpContext, "Beneficiario agregado con éxito", ActionStatus.Success, user.Id, cancellationToken: cancellationToken);
      return Ok(result);
    }
    catch (Exception ex)
    {

      await _logService.CreateLog(HttpContext, "Ha ocurrido un error al agregar el beneficiario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<BeneficiaryResponseDTO>.Error(new string[] { "Ha ocurrido un error al agregar el beneficiario", ex.Message }));
    }
  }
}
