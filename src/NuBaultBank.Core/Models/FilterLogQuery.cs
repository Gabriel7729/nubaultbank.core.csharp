using NuBaultBank.Core.Enums;

namespace NuBaultBank.Core.Models;
public class FilterLogQuery
{
  public string? Criteria { get; set; }
  public ActionStatus? ActionStatus { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public Guid? UserId { get; set; }
  public int? PageNumber { get; set; } = 1;
  public int? PageSize { get; set; } = 10;
}
