using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Features.Utilisateurs.ObtenirListeUtilisateurs;
using FormationGhApi.Services.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace FormationGhApi.Api.Controllers;

[ApiController]
[Route("api/utilisateurs")]
public class UtilisateursController : ControllerBase
{
    private readonly IMediator _mediator;

    public UtilisateursController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UtilisateurDto>>> ObtenirListe(CancellationToken cancellationToken)
    {
        var utilisateurs = await _mediator.SendAsync(new ObtenirListeUtilisateursQuery(), cancellationToken);
        return Ok(utilisateurs);
    }
}
