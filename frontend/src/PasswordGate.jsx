import { useState } from 'react'

export default function PasswordGate({ onAuthenticated }) {
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  function handleSubmit(event) {
    event.preventDefault()
    if (password !== '1234') {
      setError('Mot de passe incorrect.')
      setPassword('')
      return
    }

    onAuthenticated()
  }

  return (
    <main className="auth-page">
      <section className="auth-card">
        <p className="eyebrow">ESPACE SÉCURISÉ</p>
        <h1>Bienvenue</h1>
        <p className="muted">Saisissez le mot de passe pour accéder à la gestion des congés.</p>
        <form onSubmit={handleSubmit}>
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
          <button className="primary-button" type="submit">Se connecter</button>
        </form>
        {error && <p className="error-message" role="alert">{error}</p>}
      </section>
    </main>
  )
}
