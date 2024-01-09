using System.Runtime.Serialization;

namespace NuBaultBank.Web.Endpoints.ProjectEndpoints;

[DataContract]
public class PokemonResponse
{
  [DataMember(Name = "name")]
  public string? Name { get; set; }
}
