using Newtonsoft.Json;

namespace NuBaultBank.Infrastructure.Dto;
public abstract class BaseResponseDto
{
  [JsonProperty(Order = -2)]
  public Guid Id { get; set; }
}
