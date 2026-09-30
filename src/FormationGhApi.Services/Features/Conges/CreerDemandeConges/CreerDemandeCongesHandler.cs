using FormationGhApi.Services.Data;
using FormationGhApi.Services.Domain;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Features.Conges.CreerDemandeConges;

public class CreerDemandeCongesHandler : IRequestHandler<CreerDemandeCongesCommand, CreerDemandeCongesResult>
{
    private readonly CongesDbContext _dbContext;

    public CreerDemandeCongesHandler(CongesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreerDemandeCongesResult> HandleAsync(CreerDemandeCongesCommand request, CancellationToken cancellationToken)
    {
        if (request.DateFin < request.DateDebut)
        {
            return new CreerDemandeCongesResult(CreerDemandeCongesStatut.PlageDatesInvalide, null);
        }

        var utilisateur = await _dbContext.Utilisateurs
            .FirstOrDefaultAsync(u => u.Nom == request.NomUtilisateur, cancellationToken);

        if (utilisateur is null)
        {
            return new CreerDemandeCongesResult(CreerDemandeCongesStatut.UtilisateurIntrouvable, null);
        }

        var demande = new DemandeConges
        {
            UtilisateurId = utilisateur.Id,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
        };

        _dbContext.DemandesConges.Add(demande);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var demandeDto = new DemandeCongesDto(demande.Id, demande.UtilisateurId, utilisateur.Nom, demande.DateDebut, demande.DateFin, demande.NombreJours);
        return new CreerDemandeCongesResult(CreerDemandeCongesStatut.Succes, demandeDto);
    }
}
