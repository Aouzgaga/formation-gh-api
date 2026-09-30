using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;

namespace FormationGhApi.Services.Features.Utilisateurs.ObtenirListeUtilisateurs;

public record ObtenirListeUtilisateursQuery : IRequest<IReadOnlyList<UtilisateurDto>>;
