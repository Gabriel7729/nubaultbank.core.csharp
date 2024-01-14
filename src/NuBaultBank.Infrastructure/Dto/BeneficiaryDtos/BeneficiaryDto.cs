
namespace NuBaultBank.Infrastructure.Dto.BeneficiaryDtos;
  public class BeneficiaryDTO
  {
    public string BeneficiaryId { get; set; }= string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string UserId { get; set;} = string.Empty;
  }

public class BeneficiaryResponseDTO : BaseResponseDto
{
  public BeneficiaryResponseDTO(BeneficiaryDTO beneficiaryDTO, string message, bool isSuccess)
  {
    BeneficiaryId = beneficiaryDTO.BeneficiaryId;
    UserId = beneficiaryDTO.UserId;
    Name = beneficiaryDTO.Name;
    PhoneNumber = beneficiaryDTO.PhoneNumber;
    Email = beneficiaryDTO.Email;
  }
  public string BeneficiaryId { get; set; } = string.Empty;
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public string UserId { get; set;} = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;

}

