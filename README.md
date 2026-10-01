# formation-gh-api

API et interface de gestion des congés.

## Interface web

L’application Blazor WebAssembly utilise le stockage local du navigateur pour conserver les utilisateurs, les soldes et les congés.

Au démarrage, un écran demande un mot de passe. La valeur par défaut est `1234` ; elle se configure dans `src/FormationGhApi.Web/wwwroot/appsettings.json` avec la clé `ApplicationPassword`.

Cette protection masque l’interface, mais ne sécurise pas les données : le mot de passe d’une application Blazor WebAssembly statique est accessible aux visiteurs. Pour protéger réellement l’application, il faut vérifier l’authentification côté serveur.

```bash
dotnet run --project src/FormationGhApi.Web/FormationGhApi.Web.csproj
```

L’écran d’accueil liste les utilisateurs. Ouvrez un utilisateur pour consulter son solde, poser des congés et supprimer les congés existants.

## Déploiement GitHub Pages

Le workflow `Deploy to GitHub Pages` publie automatiquement l’application après chaque push sur `main`. Pour l’activer, choisissez **GitHub Actions** comme source dans **Settings → Pages**. Le workflow peut également être lancé manuellement depuis l’onglet **Actions**. Il configure le chemin de base du dépôt et le repli des URL client comme `/user/{id}`.
