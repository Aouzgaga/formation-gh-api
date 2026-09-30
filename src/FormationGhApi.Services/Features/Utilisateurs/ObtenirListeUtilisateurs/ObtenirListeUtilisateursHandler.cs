using FormationGhApi.Services.Data;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Features.Utilisateurs.ObtenirListeUtilisateurs;

public class ObtenirListeUtilisateursHandler : IRequestHandler<ObtenirListeUtilisateursQuery, IReadOnlyList<UtilisateurDto>>
{
    private readonly CongesDbContext _dbContext;

    public ObtenirListeUtilisateursHandler(CongesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UtilisateurDto>> HandleAsync(ObtenirListeUtilisateursQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Utilisateurs
            .AsNoTracking()
            .OrderBy(u => u.Nom)
            .ThenBy(u => u.Prenom)
            .Select(u => new UtilisateurDto(u.Id, u.Nom, u.Prenom, u.Email))
            .ToListAsync(cancellationToken);
    }
}
