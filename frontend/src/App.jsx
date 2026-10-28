import { useEffect, useState } from 'react'
import { BrowserRouter, Link, Route, Routes, useParams } from 'react-router-dom'
import PasswordGate from './PasswordGate.jsx'
import { addLeave, deleteLeave, getUsers, workingDays } from './storage.js'

function Layout({ children }) {
  return (
    <>
      <header className="site-header">
        <Link className="brand" to="/">Gestion des congés</Link>
        <nav aria-label="Navigation principale">
          <Link to="/">Utilisateurs</Link>
        </nav>
      </header>
      <main className="content">{children}</main>
      <footer>Vos données sont enregistrées dans ce navigateur.</footer>
    </>
  )
}

function HomePage() {
  const [users] = useState(getUsers)
  useEffect(() => {
    document.title = 'Utilisateurs — Gestion des congés'
  }, [])

  return (
    <Layout>
      <section className="page-heading">
        <p className="eyebrow">ESPACE ÉQUIPE</p>
        <h1>Les utilisateurs</h1>
        <p className="muted">Consultez les soldes et gérez les congés de votre équipe.</p>
      </section>
      <section className="user-list" aria-label="Liste des utilisateurs">
        {users.map((user) => (
          <Link className="user-card" key={user.id} to={`/user/${user.id}`}>
            <span className="avatar" aria-hidden="true">{user.prenom[0]}{user.nom[0]}</span>
            <span className="user-info">
              <strong>{user.prenom} {user.nom}</strong>
              <span className="muted">{user.email}</span>
            </span>
            <span className="card-arrow" aria-hidden="true">→</span>
          </Link>
        ))}
      </section>
    </Layout>
  )
}

function formatDate(date) {
  return new Date(`${date}T00:00:00`).toLocaleDateString('fr-FR')
}

function today() {
  const date = new Date()
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

function UserDetailsPage() {
  const { id } = useParams()
  const userId = Number(id)
  const [user, setUser] = useState(() => getUsers().find((item) => item.id === userId))
  const [startDate, setStartDate] = useState(today)
  const [endDate, setEndDate] = useState(today)
  const [error, setError] = useState('')
  const selectedDays = workingDays(startDate, endDate)

  useEffect(() => {
    setUser(getUsers().find((item) => item.id === userId))
  }, [userId])

  useEffect(() => {
    document.title = user ? `${user.prenom} ${user.nom} — Gestion des congés` : 'Utilisateur — Gestion des congés'
  }, [user])

  function refreshUser() {
    setUser(getUsers().find((item) => item.id === userId))
  }

  function handleAddLeave() {
    setError('')
    try {
      addLeave(userId, startDate, endDate)
      refreshUser()
    } catch (exception) {
      setError(exception.message)
    }
  }

  function handleDeleteLeave(leaveId) {
    deleteLeave(userId, leaveId)
    refreshUser()
  }

  return (
    <Layout>
      <Link className="back-link" to="/">← Tous les utilisateurs</Link>
      {!user ? (
        <h1>Utilisateur introuvable</h1>
      ) : (
        <>
          <section className="page-heading">
            <p className="eyebrow">ESPACE ÉQUIPE / UTILISATEUR</p>
            <h1>{user.prenom} {user.nom}</h1>
            <p className="muted">{user.email}</p>
          </section>

          <section className="balance-grid" aria-label="Solde de congés payés">
            <article className="balance-card">
              <span className="muted">Solde disponible</span>
              <strong>{user.joursAcquis - user.conges.reduce((sum, leave) => sum + leave.nombreJours, 0)} <small>jours</small></strong>
            </article>
            <article className="balance-card">
              <span className="muted">Jours acquis</span>
              <strong>{user.joursAcquis} <small>jours</small></strong>
            </article>
            <article className="balance-card">
              <span className="muted">Jours pris</span>
              <strong>{user.conges.reduce((sum, leave) => sum + leave.nombreJours, 0)} <small>jours</small></strong>
            </article>
          </section>

          <section className="section-card">
            <div className="section-heading">
              <div>
                <p className="eyebrow">PLANIFICATION</p>
                <h2>Poser un congé</h2>
              </div>
            </div>
            <div className="leave-form">
              <label>
                Date de début
                <input type="date" value={startDate} onChange={(event) => setStartDate(event.target.value)} />
              </label>
              <label>
                Date de fin
                <input type="date" value={endDate} onChange={(event) => setEndDate(event.target.value)} />
              </label>
              <button
                className="primary-button"
                type="button"
                onClick={handleAddLeave}
                disabled={selectedDays <= 0 || selectedDays > user.joursAcquis - user.conges.reduce((sum, leave) => sum + leave.nombreJours, 0)}
              >
                Poser le congé
              </button>
            </div>
            <p className="form-hint">
              {selectedDays <= 0
                ? 'Choisissez une période contenant au moins un jour ouvré.'
                : `${selectedDays} jour(s) ouvré(s) sélectionné(s), hors week-ends.`}
            </p>
            {error && <p className="error-message" role="alert">{error}</p>}
          </section>

          <section className="section-card">
            <div className="section-heading">
              <div>
                <p className="eyebrow">HISTORIQUE</p>
                <h2>Congés posés <span className="count">{user.conges.length}</span></h2>
              </div>
            </div>
            {user.conges.length === 0 ? (
              <p className="empty-state">Aucun congé n’a été posé pour le moment.</p>
            ) : (
              <div className="leave-list">
                {[...user.conges].sort((a, b) => a.dateDebut.localeCompare(b.dateDebut)).map((leave) => (
                  <article className="leave-row" key={leave.id}>
                    <span className="leave-icon" aria-hidden="true">↗</span>
                    <span className="leave-dates">
                      <strong>{formatDate(leave.dateDebut)} – {formatDate(leave.dateFin)}</strong>
                      <span className="muted">{leave.nombreJours} jour(s) ouvré(s)</span>
                    </span>
                    <button
                      className="delete-button"
                      type="button"
                      onClick={() => handleDeleteLeave(leave.id)}
                      aria-label={`Supprimer le congé du ${formatDate(leave.dateDebut)}`}
                    >
                      Supprimer
                    </button>
                  </article>
                ))}
              </div>
            )}
          </section>
        </>
      )}
    </Layout>
  )
}

function NotFoundPage() {
  useEffect(() => {
    document.title = 'Page introuvable — Gestion des congés'
  }, [])
  return (
    <Layout>
      <h1>Page introuvable</h1>
      <p>La page demandée n’existe pas.</p>
    </Layout>
  )
}

export default function App() {
  const [authenticated, setAuthenticated] = useState(false)

  if (!authenticated) {
    return <PasswordGate onAuthenticated={() => setAuthenticated(true)} />
  }

  return (
    <BrowserRouter basename={import.meta.env.BASE_URL}>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/user/:id" element={<UserDetailsPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  )
}
