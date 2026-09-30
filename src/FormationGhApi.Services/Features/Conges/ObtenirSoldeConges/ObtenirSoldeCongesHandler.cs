using FormationGhApi.Services.Data;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Features.Conges.ObtenirSoldeConges;

public class ObtenirSoldeCongesHandler : IRequestHandler<ObtenirSoldeCongesQuery, SoldeCongesDto?>
{
    private readonly CongesDbContext _dbContext;

    public ObtenirSoldeCongesHandler(CongesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SoldeCongesDto?> HandleAsync(ObtenirSoldeCongesQuery request, CancellationToken cancellationToken)
    {
        var compteur = await _dbContext.CompteursConges
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UtilisateurId == request.UtilisateurId && c.Annee == request.Annee, cancellationToken);

        if (compteur is null)
        {
            return null;
        }

        return new SoldeCongesDto(compteur.UtilisateurId, compteur.Annee, compteur.JoursAcquis, compteur.JoursPris, compteur.JoursSolde);
    }
}
