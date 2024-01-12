using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Entities.BeneficiaryAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.BeneficiaryDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.BeneficiaryEndpoints;

[Authorize]
public class EditBeneficiary : EndpointBaseAsync
    .WithRequest<BeneficiaryDTO>
    .WithActionResult<BeneficiaryResponseDTO>
{
  private readonly IBeneficiary _beneficiaryAppService;
  private readonly ILogService _logService;
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<Beneficiary> _beneficiaryRepository;

  public EditBeneficiary(IBeneficiary beneficiaryAppService, ILogService logService, IRepository<User> userRepository, IRepository<Beneficiary> beneficiaryRepository)
  {
    _beneficiaryAppService = beneficiaryAppService;
    _logService = logService;
    _userRepository = userRepository;
    _beneficiaryRepository = beneficiaryRepository;
  }

  [HttpPut("/api/Beneficiary/Edit")]
  [SwaggerOperation(
      Summary = "Edit Beneficiary",
      Description = "Edit Beneficiary",
      OperationId = "EditBeneficiary.User",
      Tags = new[] { "BeneficiaryEndpoints" })
  ]
  public override async Task<ActionResult<BeneficiaryResponseDTO>> HandleAsync([FromBody] BeneficiaryDTO requestDto, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Guid.TryParse(requestDto.UserId, out Guid userIdGuid))
        return BadRequest(Result<BeneficiaryResponseDTO>.Error("El formato del UserId no es válido"));

      User? user = await _userRepository.GetByIdAsync(userIdGuid, cancellationToken);
      if (user is null)
        return BadRequest(Result<BeneficiaryResponseDTO>.Error("El usuario no fue encontrado"));

      if (!Guid.TryParse(requestDto.BeneficiaryId, out Guid beneficiaryIdGuid))
        return BadRequest(Result<BeneficiaryResponseDTO>.Error("El formato del BeneficiaryId no es válido"));

      //aqui intento verificar si el veneficiario ya existe, pero no entendi bien xd

      /*      GetBeneficiaryByIdSpec beneficiaryByIdSpec = new(beneficiaryIdGuid);
            Beneficiary? existingBeneficiary = await _beneficiaryRepository.GetBySpecAsync(beneficiaryByIdSpec, cancellationToken);

            GetIfUserAlreadyhasABeneficiarySpec alreadyHasBeneficiarySpec = new(userIdGuid, requestDto.Email);
            Beneficiary? existingBeneficiaryByEmail = await _beneficiaryRepository.GetBySpecAsync(alreadyHasBeneficiarySpec, cancellationToken);
      */

      //cree una vaina vacia para que no explote
      Beneficiary existingBeneficiary = new Beneficiary();

      if (existingBeneficiary != null)
      {
        existingBeneficiary.Name = requestDto.Name;
        existingBeneficiary.PhoneNumber = requestDto.PhoneNumber;
        existingBeneficiary.Email = requestDto.Email;


      }
      else
      {
        return BadRequest(Result<BeneficiaryResponseDTO>.Error("El beneficiario no existe."));
      }

      await _userRepository.UpdateAsync(user, cancellationToken);

      await _logService.CreateLog(HttpContext, "Beneficiario editado con éxito", ActionStatus.Success, user.Id, cancellationToken: cancellationToken);

      var result = Result<BeneficiaryResponseDTO>.Success(new BeneficiaryResponseDTO
      {
        Success = true,
        Message = "Beneficiario editado con éxito",
        UserId = user.Id.ToString(), 
        Name = requestDto.Name,
        PhoneNumber = requestDto.PhoneNumber,
        Email = requestDto.Email,
      });

      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error al editar el beneficiario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return StatusCode(500, Result<BeneficiaryResponseDTO>.Error("Ha ocurrido un error interno."));
    }
  }
}
