using Ardalis.ApiEndpoints;
using Ardalis.Result;
using AutoMapper;
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
  private readonly IMapper _mapper;
  private readonly ILogService _logService;
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<Beneficiary> _beneficiaryRepository;

  public EditBeneficiary(
    IMapper mapper,
    ILogService logService, 
    IRepository<User> userRepository, 
    IRepository<Beneficiary> beneficiaryRepository)
  {
    _logService = logService;
    _userRepository = userRepository;
    _beneficiaryRepository = beneficiaryRepository;
    _mapper = mapper;
  }

  [HttpPut("/api/Beneficiary")]
  [SwaggerOperation(
      Summary = "Edit Beneficiary",
      Description = "Edit Beneficiary",
      OperationId = "BeneficiaryEndpoints.Edit",
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

      Beneficiary beneficiary = _mapper.Map<Beneficiary>(requestDto);
      await _beneficiaryRepository.UpdateAsync(beneficiary, cancellationToken);

      BeneficiaryDTO beneficiaryDTO = _mapper.Map<BeneficiaryDTO>(beneficiary);
      BeneficiaryResponseDTO response = new BeneficiaryResponseDTO(beneficiaryDTO, "Beneficiario editado con éxito", true);

      var result = Result<BeneficiaryResponseDTO>.Success(response);
      await _logService.CreateLog(HttpContext, "Beneficiario editado con éxito", ActionStatus.Success, user.Id, cancellationToken: cancellationToken);

      return Ok(result);
    }
    catch (Exception ex)
    {
      await _logService.CreateLog(HttpContext, "Ha ocurrido un error al editar el beneficiario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return StatusCode(500, Result<BeneficiaryResponseDTO>.Error("Ha ocurrido un error interno."));
    }
  }
}
