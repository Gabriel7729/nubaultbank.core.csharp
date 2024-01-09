using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace NuBaultBank.Infrastructure.Config.RestClientExtensions;
public class ApiAccess
{
  public string? BaseUri { get; set; }
  public string? BaseToken { get; set; }
}
