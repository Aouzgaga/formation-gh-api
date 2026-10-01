namespace FormationGhApi.Web.Services;

public sealed class PasswordService
{
    private readonly string motDePasseAttendu;

    public PasswordService(IConfiguration configuration)
    {
        motDePasseAttendu = configuration["ApplicationPassword"] ?? string.Empty;
    }

    public bool EstAuthentifie { get; private set; }

    public Task InitialiserAsync() => Task.CompletedTask;

    public Task<bool> AuthentifierAsync(string motDePasse)
    {
        if (string.IsNullOrEmpty(motDePasseAttendu) || motDePasse != motDePasseAttendu)
        {
            return Task.FromResult(false);
        }

        EstAuthentifie = true;
        return Task.FromResult(true);
    }
}
