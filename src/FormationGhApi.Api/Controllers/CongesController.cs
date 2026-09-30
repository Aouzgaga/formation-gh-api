using FormationGhApi.Api.Contracts;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Features.Conges.CreerConge;
using FormationGhApi.Services.Features.Conges.DeclarerPriseConges;
using FormationGhApi.Services.Features.Conges.ObtenirSoldeConges;
using FormationGhApi.Services.Features.Conges.ReinitialiserCompteurConges;
using FormationGhApi.Services.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace FormationGhApi.Api.Controllers;

[ApiController]
[Route("api/utilisateurs/{utilisateurId:int}/conges/{annee:int}")]
public class CongesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CongesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SoldeCongesDto>> ObtenirSolde(int utilisateurId, int annee, CancellationToken cancellationToken)
    {
        var solde = await _mediator.SendAsync(new ObtenirSoldeCongesQuery(utilisateurId, annee), cancellationToken);
        if (solde is null)
        {
            return NotFound();
        }

        return Ok(solde);
    }

    [HttpPost("prises")]
    public async Task<ActionResult<SoldeCongesDto>> DeclarerPrise(int utilisateurId, int annee, [FromBody] DeclarerPriseCongesRequest request, CancellationToken cancellationToken)
    {
        var resultat = await _mediator.SendAsync(new DeclarerPriseCongesCommand(utilisateurId, annee, request.NombreJours), cancellationToken);

        return resultat.Statut switch
        {
            DeclarerPriseCongesStatut.Succes => Ok(resultat.Solde),
            DeclarerPriseCongesStatut.SoldeInsuffisant => Conflict(resultat.Solde),
            _ => NotFound(),
        };
    }

    [HttpPost("reinitialisation")]
    public async Task<ActionResult<SoldeCongesDto>> Reinitialiser(int utilisateurId, int annee, CancellationToken cancellationToken)
    {
        var solde = await _mediator.SendAsync(new ReinitialiserCompteurCongesCommand(utilisateurId, annee), cancellationToken);
        if (solde is null)
        {
            return NotFound();
        }

        return Ok(solde);
    }

    [HttpPost("~/api/conges")]
    public async Task<ActionResult<CongeDto>> Creer([FromBody] CreerCongeRequest request, CancellationToken cancellationToken)
    {
        var resultat = await _mediator.SendAsync(new CreerCongeCommand(request.NomUtilisateur, request.DateDebut, request.DateFin), cancellationToken);

        return resultat.Statut switch
        {
            CreerCongeStatut.Succes => CreatedAtAction(nameof(ObtenirSolde), new { utilisateurId = resultat.Conge!.UtilisateurId, annee = resultat.Conge.DateDebut.Year }, resultat.Conge),
            CreerCongeStatut.UtilisateurIntrouvable => NotFound(),
            CreerCongeStatut.PlageDatesInvalide => BadRequest("La date de fin doit être postérieure ou égale à la date de début."),
            _ => BadRequest(),
        };
    }
}
