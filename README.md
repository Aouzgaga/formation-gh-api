# formation-gh-api

API et interface de gestion des congés.

## Interface web

L’application front-end est maintenant une SPA React + Vite. Les données sont conservées localement dans le navigateur pour les utilisateurs, les soldes et les congés.

```bash
cd src/FormationGhApi.Web
npm install
npm run dev
```

L’écran d’accueil liste les utilisateurs. Ouvrez un utilisateur pour consulter son solde, poser des congés et supprimer les congés existants.

## Déploiement GitHub Pages

Le workflow `Deploy to GitHub Pages` publie automatiquement l’application après chaque push sur `main`. Pour l’activer, choisissez **GitHub Actions** comme source dans **Settings → Pages**. Le workflow peut également être lancé manuellement depuis l’onglet **Actions**. Il configure le chemin de base du dépôt et le repli des URL client comme `/user/{id}`.
