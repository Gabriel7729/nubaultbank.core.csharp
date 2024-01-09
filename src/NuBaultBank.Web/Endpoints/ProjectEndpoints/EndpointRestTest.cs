using Ardalis.ApiEndpoints;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Config;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.ProjectEndpoints;

public class EndpointRestTest : EndpointBaseAsync
      .WithRequest<PokemonRequest>
      .WithResult<PokemonResponse>
{
  private IRestClient _restClient;
  public EndpointRestTest(IRestClient restClient)
  {
    _restClient = restClient;
  }
  [HttpPost("/Test/{PokemonId:int}")]
  [SwaggerOperation(
          Summary = "Get Api Request",
          Description = "Get Api Request",
          OperationId = "Test.Get",
          Tags = new[] { "TestEndpoints" })
      ]
  public override async Task<PokemonResponse> HandleAsync([FromRoute]PokemonRequest request, CancellationToken cancellationToken = default)
  {
    var result = await _restClient.GetAsync<PokemonResponse>($"pokemon/{request.PokemonId}");
    return result;
  }
}
