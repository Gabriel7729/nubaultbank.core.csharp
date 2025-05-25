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
public class Create : EndpointBaseAsync
    .WithRequest<BeneficiaryDTO>
    .WithActionResult<BeneficiaryResponseDTO>
{
  private readonly IRepository<User> _userRepository;
  private readonly IMapper _mapper;
  private readonly ILogService _logService;

  public Create(IRepository<User> userRepository, IMapper mapper, ILogService logService)
  {
    _userRepository = userRepository;
    _logService = logService;
    _mapper = mapper;
  }

  [HttpPost("/api/Beneficiary")]
  [SwaggerOperation(
      Summary = "Add Beneficiary to User Account",
      Description = "Add Beneficiary to User Account",
      OperationId = "BeneficiaryEndpoints.Create",
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

      // Add beneficiary to user
      Beneficiary beneficiary = _mapper.Map<Beneficiary>(requestDto);
      user.Beneficiaries.Add(beneficiary);
      await _userRepository.UpdateAsync(user, cancellationToken);

      // Return DTO of added beneficiary
      BeneficiaryDTO beneficiaryDTO = _mapper.Map<BeneficiaryDTO>(beneficiary);
      var result = Result<BeneficiaryDTO>.Success(beneficiaryDTO, "Beneficiario agregado con éxito");

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
