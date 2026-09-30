using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;

namespace FormationGhApi.Services.Features.Conges.CreerDemandeConges;

public record CreerDemandeCongesCommand(string NomUtilisateur, DateOnly DateDebut, DateOnly DateFin) : IRequest<CreerDemandeCongesResult>;

public enum CreerDemandeCongesStatut
{
    Succes,
    UtilisateurIntrouvable,
    PlageDatesInvalide,
}

public record CreerDemandeCongesResult(CreerDemandeCongesStatut Statut, DemandeCongesDto? Demande);
