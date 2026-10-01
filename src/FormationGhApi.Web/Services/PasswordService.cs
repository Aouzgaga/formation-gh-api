using System.Security.Cryptography;
using System.Text;
using Microsoft.JSInterop;

namespace FormationGhApi.Web.Services;

public sealed class PasswordService(IJSRuntime jsRuntime)
{
    private const string StorageKey = "formation-gh-api.password-hash";

    public bool EstConfigure { get; private set; }
    public bool EstAuthentifie { get; private set; }

    public async Task InitialiserAsync()
    {
        var hash = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        EstConfigure = !string.IsNullOrWhiteSpace(hash);
    }

    public async Task<bool> ConfigurerAsync(string motDePasse)
    {
        if (string.IsNullOrWhiteSpace(motDePasse))
        {
            return false;
        }

        await EnregistrerEmpreinteAsync(motDePasse);
        EstConfigure = true;
        EstAuthentifie = true;
        return true;
    }

    public async Task<bool> AuthentifierAsync(string motDePasse)
    {
        var empreinte = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (empreinte is null || !empreinte.Equals(CalculerEmpreinte(motDePasse), StringComparison.Ordinal))
        {
            return false;
        }

        EstAuthentifie = true;
        return true;
    }

    private async Task EnregistrerEmpreinteAsync(string motDePasse)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, CalculerEmpreinte(motDePasse));
    }

    private static string CalculerEmpreinte(string motDePasse) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(motDePasse)));
}
