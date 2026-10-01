namespace FormationGhApi.Web.Services;

public sealed class PasswordService
{
    public bool EstAuthentifie { get; private set; }

    public Task InitialiserAsync() => Task.CompletedTask;

    public Task<bool> AuthentifierAsync(string motDePasse)
    {
        if (motDePasse != "1234")
        {
            return Task.FromResult(false);
        }

        EstAuthentifie = true;
        return Task.FromResult(true);
    }
}
