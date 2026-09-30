namespace FormationGhApi.Services.Dtos;

public record DemandeCongesDto(int Id, int UtilisateurId, string NomUtilisateur, DateOnly DateDebut, DateOnly DateFin, int NombreJours);
