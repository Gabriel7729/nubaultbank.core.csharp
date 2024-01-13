using NuBaultBank.Core.Entities.BeneficiaryAggregate;

namespace NuBaultBank.Infrastructure.Dto.UserDtos;
public class UserDto
{
  public string Name { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string IdNumber { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

public class UserResponseDto : BaseResponseDto
{
  public UserResponseDto()
  {
    Beneficiaries = new HashSet<Beneficiary>();
  }
  public string Name { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string IdNumber { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public ICollection<Beneficiary> Beneficiaries { get; set; }
}
