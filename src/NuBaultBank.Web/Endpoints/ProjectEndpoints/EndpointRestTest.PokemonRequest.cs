using System.ComponentModel.DataAnnotations;

namespace NuBaultBank.Web.Endpoints.ProjectEndpoints;

public class PokemonRequest
{
  [Required]
  public int PokemonId { get; set; }
}
