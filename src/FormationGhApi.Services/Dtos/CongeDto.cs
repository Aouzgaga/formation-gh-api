namespace FormationGhApi.Services.Dtos;

public record CongeDto(int Id, int UtilisateurId, string NomUtilisateur, DateOnly DateDebut, DateOnly DateFin, int NombreJours);
