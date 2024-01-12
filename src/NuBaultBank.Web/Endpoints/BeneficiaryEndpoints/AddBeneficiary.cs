using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.BeneficiaryAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.BeneficiaryDtos;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NuBaultBank.Web.Endpoints.BeneficiaryEndpoints;

[Authorize]
public class AddBeneficiary : EndpointBaseAsync
    .WithRequest<BeneficiaryResponseDTO>
    .WithActionResult<GeneralResponse>
{
  private readonly IRepository<User> _userRepository;
  private readonly ILogService _logService;

  public AddBeneficiary(IRepository<User> userRepository, ILogService logService)
  {
    _userRepository = userRepository;
    _logService = logService;
  }

  [HttpPost("/api/Beneficiary/Add")]
  [SwaggerOperation(
      Summary = "Add Beneficiary to User Account",
      Description = "Add Beneficiary to User Account",
      OperationId = "AddBeneficiary.User",
      Tags = new[] { "BeneficiaryEndpoints" })
  ]
  public override async Task<ActionResult<GeneralResponse>> HandleAsync([FromBody] BeneficiaryResponseDTO requestDto, CancellationToken cancellationToken = default)
  {
    try
    {
      User? user = await _userRepository.GetByIdAsync(requestDto.UserId, cancellationToken);
      if (user is null)
        return BadRequest(Result<GeneralResponse>.Error("El usuario no fue encontrado"));

      /*     if (user.Beneficiaries.Any(b => b.Email == requestDto.Email))
           {
             return BadRequest(Result<GeneralResponse>.Error("El beneficiario ya existe para este usuario"));
           }*/

      var newBeneficiary = new Beneficiary
      {
        Name = requestDto.Name,
        PhoneNumber = requestDto.PhoneNumber,
        Email = requestDto.Email,
     
      };

    /*  user.Beneficiaries.Add(newBeneficiary);*/

      await _logService.CreateLog(HttpContext, "Beneficiario agregado con éxito", ActionStatus.Success, user.Id, cancellationToken: cancellationToken);

      var result = Result<GeneralResponse>.Success(new GeneralResponse(true, "Beneficiario agregado con éxito"));
      return Ok(result);
    }
    catch (Exception ex)
    {

      await _logService.CreateLog(HttpContext, "Ha ocurrido un error al agregar el beneficiario", ActionStatus.Error, exceptionMessage: ex.ToString(), cancellationToken: cancellationToken);
      return BadRequest(Result<GeneralResponse>.Error(new string[] { "Ha ocurrido un error al agregar el beneficiario", ex.Message }));
    }
  }
}
