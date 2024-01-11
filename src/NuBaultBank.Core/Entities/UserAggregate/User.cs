using NuBaultBank.Core.Enums;
using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.UserAggregate;
public class User : EntityBase, IAggregateRoot
{
  public string Name { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string IdNumber { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public bool IsActive { get; set; } = false;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
