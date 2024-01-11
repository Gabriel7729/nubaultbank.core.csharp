using Microsoft.AspNetCore.Http;
using NuBaultBank.Core.Enums;

namespace NuBaultBank.Core.Interfaces;
public interface ILogService
{
  Task CreateLog(HttpContext context, string message, ActionStatus actionStatus, Guid? userId = null, string? exceptionMessage = null, CancellationToken cancellationToken = default);
  Task CreateLog(string username, string message, ActionStatus actionStatus, Guid? userId = null, string? exceptionMessage = null, CancellationToken cancellationToken = default);
}
