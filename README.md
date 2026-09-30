# formation-gh-api

API et interface de gestion des congés.

## Interface web

L’application Blazor WebAssembly utilise le stockage local du navigateur pour conserver les utilisateurs, les soldes et les congés.

```bash
dotnet run --project src/FormationGhApi.Web/FormationGhApi.Web.csproj
```

L’écran d’accueil liste les utilisateurs. Ouvrez un utilisateur pour consulter son solde, poser des congés et supprimer les congés existants.

## Déploiement GitHub Pages

Le workflow `Deploy to GitHub Pages` publie automatiquement l’application après chaque push sur `main`. Pour l’activer, choisissez **GitHub Actions** comme source dans **Settings → Pages**. Le workflow peut également être lancé manuellement depuis l’onglet **Actions**. Il configure le chemin de base du dépôt et le repli des URL client comme `/user/{id}`.
