# formation-gh-api

API et interface de gestion des congés.

## Interface web React

L’interface React utilise le stockage local du navigateur pour conserver les utilisateurs, les soldes et les congés.

```bash
npm --prefix frontend ci
npm --prefix frontend run dev
```

L’écran d’accueil liste les utilisateurs. Ouvrez un utilisateur pour consulter son solde, poser des congés et supprimer les congés existants. L’application utilise React 19 et Vite.

## Déploiement GitHub Pages

Le workflow `Deploy to GitHub Pages` publie automatiquement l’application après chaque push sur `main`. Pour l’activer, choisissez **GitHub Actions** comme source dans **Settings → Pages**. Le workflow peut également être lancé manuellement depuis l’onglet **Actions**. Il configure le chemin de base du dépôt et le repli des URL client comme `/user/{id}`.
