using NuBaultBank.Core.Enums;
using NuBaultBank.Infrastructure.Dto.TransferDtos;

namespace NuBaultBank.Infrastructure.Dto.ProductDtos;
public class AccountDto
{
  public Guid UserId { get; set; }
  public AccountExtractBalanceDto AccountExtractBalance { get; set; } = new();
}

public class AccountExtractBalanceDto
{
  public Guid AccountId { get; set; }
  public decimal Amount { get; set; }
}

public class AccountResponseDto : BaseResponseDto
{
  public string AccountNumber { get; set; } = string.Empty;
  public decimal Balance { get; private set; }
  public AccountType AccountType { get; set; }
  public Guid UserId { get; set; }

  public ICollection<TransferResponseDto> Transfers { get; set; } = new List<TransferResponseDto>();
}
