import { useEffect, useMemo, useState } from 'react';

const STORAGE_KEY = 'formation-gh-api.conges';
const AUTH_KEY = 'formation-gh-api.auth';

const defaultUsers = [
  { id: 1, nom: 'Dupont', prenom: 'Jean', email: 'jean.dupont@formation.local', joursAcquis: 25, conges: [] },
  { id: 2, nom: 'Martin', prenom: 'Sophie', email: 'sophie.martin@formation.local', joursAcquis: 25, conges: [] },
  { id: 3, nom: 'Bernard', prenom: 'Luc', email: 'luc.bernard@formation.local', joursAcquis: 25, conges: [] },
];

function readUsers() {
  if (typeof window === 'undefined') {
    return defaultUsers;
  }

  const raw = window.localStorage.getItem(STORAGE_KEY);
  if (!raw) {
    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(defaultUsers));
    return defaultUsers;
  }

  try {
    const parsed = JSON.parse(raw);
    return Array.isArray(parsed) && parsed.length > 0 ? parsed : defaultUsers;
  } catch {
    return defaultUsers;
  }
}

function saveUsers(users) {
  if (typeof window !== 'undefined') {
    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(users));
  }
}

function countWorkingDays(dateDebut, dateFin) {
  if (!dateDebut || !dateFin || dateFin < dateDebut) {
    return 0;
  }

  let count = 0;
  const start = new Date(`${dateDebut}T00:00:00`);
  const end = new Date(`${dateFin}T00:00:00`);

  for (let current = new Date(start); current <= end; current.setDate(current.getDate() + 1)) {
    const day = current.getDay();
    if (day !== 0 && day !== 6) {
      count += 1;
    }
  }

  return count;
}

function formatDate(dateString) {
  if (!dateString) {
    return '';
  }

  return new Date(`${dateString}T00:00:00`).toLocaleDateString('fr-FR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
}

function App() {
  const [users, setUsers] = useState(() => readUsers());
  const [selectedUserId, setSelectedUserId] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(() => {
    if (typeof window === 'undefined') {
      return true;
    }
    return window.localStorage.getItem(AUTH_KEY) === 'true';
  });
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const selectedUser = useMemo(
    () => users.find((user) => user.id === selectedUserId) ?? null,
    [selectedUserId, users],
  );

  useEffect(() => {
    saveUsers(users);
  }, [users]);

  const handleLogin = (event) => {
    event.preventDefault();
    if (!password.trim()) {
      setError('Veuillez saisir un mot de passe.');
      return;
    }

    setIsAuthenticated(true);
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(AUTH_KEY, 'true');
    }
    setError('');
  };

  const handleLogout = () => {
    setIsAuthenticated(false);
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(AUTH_KEY, 'false');
    }
    setPassword('');
    setSelectedUserId(null);
  };

  const addConge = (event) => {
    event.preventDefault();
    const form = event.currentTarget;
    const dateDebut = form.dateDebut.value;
    const dateFin = form.dateFin.value;

    if (!dateDebut || !dateFin) {
      setError('Veuillez sélectionner une période de congés.');
      return;
    }

    const updatedUsers = users.map((user) => {
      if (user.id !== selectedUserId) {
        return user;
      }

      const nombreJours = countWorkingDays(dateDebut, dateFin);
      if (nombreJours <= 0) {
        setError('La période doit contenir au moins un jour ouvré.');
        return user;
      }

      const userHasConflit = user.conges.some((conge) => {
        const start = conge.dateDebut;
        const end = conge.dateFin;
        return dateDebut <= end && dateFin >= start;
      });

      if (userHasConflit) {
        setError('Cette période chevauche un congé déjà posé.');
        return user;
      }

      const totalPris = user.conges.reduce((sum, conge) => sum + conge.nombreJours, 0);
      const soldeRestant = user.joursAcquis - totalPris;
      if (nombreJours > soldeRestant) {
        setError('Le solde disponible est insuffisant pour cette période.');
        return user;
      }

      const newConge = {
        id: crypto.randomUUID(),
        dateDebut,
        dateFin,
        nombreJours,
      };

      return {
        ...user,
        conges: [...user.conges, newConge],
      };
    });

    const hasChanged = updatedUsers !== users;
    if (!hasChanged) {
      return;
    }

    setUsers(updatedUsers);
    setError('');
    form.reset();
  };

  const deleteConge = (congeId) => {
    setUsers((currentUsers) =>
      currentUsers.map((user) =>
        user.id === selectedUserId
          ? { ...user, conges: user.conges.filter((conge) => conge.id !== congeId) }
          : user,
      ),
    );
  };

  if (!isAuthenticated) {
    return (
      <main className="auth-page">
        <section className="auth-card">
          <p className="eyebrow">ESPACE SÉCURISÉ</p>
          <h1>Bienvenue</h1>
          <p className="muted">Saisissez le mot de passe pour accéder à la gestion des congés.</p>
          <form onSubmit={handleLogin}>
            <label>
              Mot de passe
              <input
                type="password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                autoComplete="current-password"
                autoFocus
              />
            </label>
            <button type="submit" className="primary-button">Se connecter</button>
          </form>
          {error ? <p className="error-message">{error}</p> : null}
        </section>
      </main>
    );
  }

  if (!selectedUser) {
    return (
      <main className="container">
        <header className="page-header">
          <div>
            <p className="eyebrow">ESPACE ÉQUIPE</p>
            <h1>Les utilisateurs</h1>
          </div>
          <button type="button" className="secondary-button" onClick={handleLogout}>Déconnexion</button>
        </header>
        <section className="user-list" aria-label="Liste des utilisateurs">
          {users.map((user) => {
            const initials = `${user.prenom[0] ?? ''}${user.nom[0] ?? ''}`;
            const joursPris = user.conges.reduce((sum, conge) => sum + conge.nombreJours, 0);

            return (
              <button
                key={user.id}
                type="button"
                className="user-card"
                onClick={() => setSelectedUserId(user.id)}
              >
                <span className="avatar" aria-hidden="true">{initials}</span>
                <span className="user-info">
                  <strong>{user.prenom} {user.nom}</strong>
                  <span className="muted">{user.email}</span>
                </span>
                <span className="balance-pill">{(user.joursAcquis - joursPris).toFixed(2)} j</span>
              </button>
            );
          })}
        </section>
      </main>
    );
  }

  const joursPris = selectedUser.conges.reduce((sum, conge) => sum + conge.nombreJours, 0);
  const solde = selectedUser.joursAcquis - joursPris;

  return (
    <main className="container">
      <header className="page-header">
        <button type="button" className="back-link" onClick={() => setSelectedUserId(null)}>← Tous les utilisateurs</button>
        <button type="button" className="secondary-button" onClick={handleLogout}>Déconnexion</button>
      </header>

      <section className="page-heading">
        <p className="eyebrow">ESPACE ÉQUIPE / UTILISATEUR</p>
        <h1>{selectedUser.prenom} {selectedUser.nom}</h1>
        <p className="muted">{selectedUser.email}</p>
      </section>

      <section className="balance-grid" aria-label="Solde de congés payés">
        <article className="balance-card">
          <span className="muted">Solde disponible</span>
          <strong>{solde.toFixed(2)} <small>jours</small></strong>
        </article>
        <article className="balance-card">
          <span className="muted">Jours acquis</span>
          <strong>{selectedUser.joursAcquis.toFixed(2)} <small>jours</small></strong>
        </article>
        <article className="balance-card">
          <span className="muted">Jours pris</span>
          <strong>{joursPris.toFixed(2)} <small>jours</small></strong>
        </article>
      </section>

      <section className="section-card">
        <div className="section-heading">
          <div>
            <p className="eyebrow">PLANIFICATION</p>
            <h2>Poser un congé</h2>
          </div>
        </div>
        <form className="leave-form" onSubmit={addConge}>
          <label>
            Date de début
            <input type="date" name="dateDebut" defaultValue={new Date().toISOString().slice(0, 10)} />
          </label>
          <label>
            Date de fin
            <input type="date" name="dateFin" defaultValue={new Date().toISOString().slice(0, 10)} />
          </label>
          <button type="submit" className="primary-button">Poser le congé</button>
        </form>
        {error ? <p className="error-message">{error}</p> : null}
      </section>

      <section className="section-card">
        <div className="section-heading">
          <div>
            <p className="eyebrow">HISTORIQUE</p>
            <h2>Congés posés <span className="count">{selectedUser.conges.length}</span></h2>
          </div>
        </div>
        {selectedUser.conges.length === 0 ? (
          <p className="empty-state">Aucun congé n’a été posé pour le moment.</p>
        ) : (
          <div className="leave-list">
            {[...selectedUser.conges].sort((a, b) => a.dateDebut.localeCompare(b.dateDebut)).map((conge) => (
              <article className="leave-row" key={conge.id}>
                <span className="leave-icon" aria-hidden="true">↗</span>
                <span className="leave-dates">
                  <strong>{formatDate(conge.dateDebut)} – {formatDate(conge.dateFin)}</strong>
                  <span className="muted">{conge.nombreJours} jour(s) ouvré(s)</span>
                </span>
                <button
                  type="button"
                  className="delete-button"
                  onClick={() => deleteConge(conge.id)}
                  aria-label={`Supprimer le congé du ${formatDate(conge.dateDebut)}`}
                >
                  Supprimer
                </button>
              </article>
            ))}
          </div>
        )}
      </section>
    </main>
  );
}

export default App;
