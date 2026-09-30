using FormationGhApi.Services.Data;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Features.Conges.ReinitialiserCompteurConges;

public class ReinitialiserCompteurCongesHandler : IRequestHandler<ReinitialiserCompteurCongesCommand, SoldeCongesDto?>
{
    private readonly CongesDbContext _dbContext;

    public ReinitialiserCompteurCongesHandler(CongesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SoldeCongesDto?> HandleAsync(ReinitialiserCompteurCongesCommand request, CancellationToken cancellationToken)
    {
        var compteur = await _dbContext.CompteursConges
            .FirstOrDefaultAsync(c => c.UtilisateurId == request.UtilisateurId && c.Annee == request.Annee, cancellationToken);

        if (compteur is null)
        {
            return null;
        }

        compteur.JoursAcquis = 0;
        compteur.JoursPris = 0;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SoldeCongesDto(compteur.UtilisateurId, compteur.Annee, compteur.JoursAcquis, compteur.JoursPris, compteur.JoursSolde);
    }
}
