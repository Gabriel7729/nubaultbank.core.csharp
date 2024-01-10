using NuBaultBank.Core.Enums;
using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.LogAggregate;
public class Log : EntityBase, IAggregateRoot
{
  public string UserName { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public string? ExceptionMessage { get; set; }
  public ActionStatus ActionStatus { get; set; }
  public Guid? UserId { get; set; }
}
