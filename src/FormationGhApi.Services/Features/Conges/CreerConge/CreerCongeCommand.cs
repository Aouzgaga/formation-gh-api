using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;

namespace FormationGhApi.Services.Features.Conges.CreerConge;

public record CreerCongeCommand(string NomUtilisateur, DateOnly DateDebut, DateOnly DateFin) : IRequest<CreerCongeResult>;

public enum CreerCongeStatut
{
    Succes,
    NomUtilisateurManquant,
    UtilisateurIntrouvable,
    UtilisateurAmbigu,
    PlageDatesInvalide,
}

public record CreerCongeResult(CreerCongeStatut Statut, CongeDto? Conge);
