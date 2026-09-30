using FormationGhApi.Api.Contracts;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Features.Conges.CreerDemandeConges;
using FormationGhApi.Services.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace FormationGhApi.Api.Controllers;

[ApiController]
[Route("api/demandes-conges")]
public class DemandesCongesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DemandesCongesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<DemandeCongesDto>> Creer([FromBody] CreerDemandeCongesRequest request, CancellationToken cancellationToken)
    {
        var resultat = await _mediator.SendAsync(new CreerDemandeCongesCommand(request.NomUtilisateur, request.DateDebut, request.DateFin), cancellationToken);

        return resultat.Statut switch
        {
            CreerDemandeCongesStatut.Succes => CreatedAtAction(nameof(Creer), new { id = resultat.Demande!.Id }, resultat.Demande),
            CreerDemandeCongesStatut.PlageDatesInvalide => BadRequest("La date de fin doit être postérieure ou égale à la date de début."),
            CreerDemandeCongesStatut.UtilisateurIntrouvable => NotFound(),
            _ => throw new InvalidOperationException($"Statut de création de demande de congés non géré : {resultat.Statut}"),
        };
    }
}
