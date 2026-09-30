using FormationGhApi.Services.Data;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Features.Conges.DeclarerPriseConges;

public class DeclarerPriseCongesHandler : IRequestHandler<DeclarerPriseCongesCommand, DeclarerPriseCongesResult>
{
    private readonly CongesDbContext _dbContext;

    public DeclarerPriseCongesHandler(CongesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeclarerPriseCongesResult> HandleAsync(DeclarerPriseCongesCommand request, CancellationToken cancellationToken)
    {
        var compteur = await _dbContext.CompteursConges
            .FirstOrDefaultAsync(c => c.UtilisateurId == request.UtilisateurId && c.Annee == request.Annee, cancellationToken);

        if (compteur is null)
        {
            return new DeclarerPriseCongesResult(DeclarerPriseCongesStatut.UtilisateurOuCompteurIntrouvable, null);
        }

        if (compteur.JoursAcquis - compteur.JoursPris < request.NombreJours)
        {
            var soldeInsuffisant = new SoldeCongesDto(compteur.UtilisateurId, compteur.Annee, compteur.JoursAcquis, compteur.JoursPris, compteur.JoursSolde);
            return new DeclarerPriseCongesResult(DeclarerPriseCongesStatut.SoldeInsuffisant, soldeInsuffisant);
        }

        compteur.JoursPris += request.NombreJours;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var solde = new SoldeCongesDto(compteur.UtilisateurId, compteur.Annee, compteur.JoursAcquis, compteur.JoursPris, compteur.JoursSolde);
        return new DeclarerPriseCongesResult(DeclarerPriseCongesStatut.Succes, solde);
    }
}
