using FormationGhApi.Services.Data;
using FormationGhApi.Services.Domain;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Features.Conges.CreerConge;

public class CreerCongeHandler : IRequestHandler<CreerCongeCommand, CreerCongeResult>
{
    private readonly CongesDbContext _dbContext;

    public CreerCongeHandler(CongesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreerCongeResult> HandleAsync(CreerCongeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NomUtilisateur))
        {
            return new CreerCongeResult(CreerCongeStatut.NomUtilisateurManquant, null);
        }

        if (request.DateFin < request.DateDebut)
        {
            return new CreerCongeResult(CreerCongeStatut.PlageDatesInvalide, null);
        }

        var nomRecherche = request.NomUtilisateur.Trim().ToLower();

        var utilisateursCorrespondants = await _dbContext.Utilisateurs
            .Where(u => (u.Prenom + " " + u.Nom).ToLower() == nomRecherche || u.Nom.ToLower() == nomRecherche)
            .ToListAsync(cancellationToken);

        if (utilisateursCorrespondants.Count == 0)
        {
            return new CreerCongeResult(CreerCongeStatut.UtilisateurIntrouvable, null);
        }

        if (utilisateursCorrespondants.Count > 1)
        {
            return new CreerCongeResult(CreerCongeStatut.UtilisateurAmbigu, null);
        }

        var utilisateur = utilisateursCorrespondants[0];

        var conge = new Conge
        {
            UtilisateurId = utilisateur.Id,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
        };

        _dbContext.Conges.Add(conge);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var nomComplet = $"{utilisateur.Prenom} {utilisateur.Nom}";
        var congeDto = new CongeDto(conge.Id, conge.UtilisateurId, nomComplet, conge.DateDebut, conge.DateFin, conge.NombreJours);
        return new CreerCongeResult(CreerCongeStatut.Succes, congeDto);
    }
}
