const STORAGE_KEY = 'formation-gh-api.conges'

const initialUsers = [
  { id: 1, nom: 'Dupont', prenom: 'Jean', email: 'jean.dupont@formation.local', joursAcquis: 25, conges: [] },
  { id: 2, nom: 'Martin', prenom: 'Sophie', email: 'sophie.martin@formation.local', joursAcquis: 25, conges: [] },
  { id: 3, nom: 'Bernard', prenom: 'Luc', email: 'luc.bernard@formation.local', joursAcquis: 25, conges: [] },
]

export function getUsers() {
  const storedUsers = localStorage.getItem(STORAGE_KEY)
  if (storedUsers) {
    return JSON.parse(storedUsers)
  }

  localStorage.setItem(STORAGE_KEY, JSON.stringify(initialUsers))
  return initialUsers
}

function saveUsers(users) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(users))
}

export function workingDays(start, end) {
  if (!start || !end || end < start) {
    return 0
  }

  const firstDay = new Date(`${start}T00:00:00Z`)
  const lastDay = new Date(`${end}T00:00:00Z`)
  if (Number.isNaN(firstDay.getTime()) || Number.isNaN(lastDay.getTime())) {
    return 0
  }

  let count = 0
  for (let day = firstDay; day <= lastDay; day.setUTCDate(day.getUTCDate() + 1)) {
    if (day.getUTCDay() !== 0 && day.getUTCDay() !== 6) {
      count += 1
    }
  }
  return count
}

export function addLeave(userId, start, end) {
  const users = getUsers()
  const user = users.find((item) => item.id === userId)
  if (!user) {
    throw new Error('Cet utilisateur n’existe pas.')
  }

  const days = workingDays(start, end)
  if (days === 0) {
    throw new Error('La période doit contenir au moins un jour ouvré.')
  }

  if (user.conges.some((leave) => start <= leave.dateFin && end >= leave.dateDebut)) {
    throw new Error('Cette période chevauche un congé déjà posé.')
  }

  const usedDays = user.conges.reduce((sum, leave) => sum + leave.nombreJours, 0)
  if (days > user.joursAcquis - usedDays) {
    throw new Error('Le solde disponible est insuffisant pour cette période.')
  }

  user.conges.push({
    id: crypto.randomUUID(),
    dateDebut: start,
    dateFin: end,
    nombreJours: days,
  })
  saveUsers(users)
}

export function deleteLeave(userId, leaveId) {
  const users = getUsers()
  const user = users.find((item) => item.id === userId)
  if (!user) {
    return
  }

  user.conges = user.conges.filter((leave) => leave.id !== leaveId)
  saveUsers(users)
}
