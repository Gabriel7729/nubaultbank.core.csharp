using NuBaultBank.Core.Enums;

namespace NuBaultBank.Infrastructure.Dto.LogDtos;
public class LogResponseDto : BaseResponseDto
{
  public string UserName { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public string? ExceptionMessage { get; set; }
  public ActionStatus ActionStatus { get; set; }
}

