import { jwtDecode } from 'jwt-decode'
import type { AuthenticationResponse } from './types'

const storageKey = 'entrelazados.session'

type JwtPayload = {
  sub?: string
  exp?: number
}

export function loadStoredSession(): AuthenticationResponse | null {
  try {
    const raw = localStorage.getItem(storageKey)
    if (!raw) return null

    const session = JSON.parse(raw) as AuthenticationResponse
    const refreshExpiresAt = Date.parse(session.refreshTokenExpiresAtUtc)
    jwtDecode<JwtPayload>(session.accessToken)

    if (!session.refreshToken || Number.isNaN(refreshExpiresAt) || refreshExpiresAt <= Date.now()) {
      localStorage.removeItem(storageKey)
      return null
    }

    return session
  } catch {
    localStorage.removeItem(storageKey)
    return null
  }
}

export function saveSession(session: AuthenticationResponse) {
  localStorage.setItem(storageKey, JSON.stringify(session))
}

export function removeStoredSession() {
  localStorage.removeItem(storageKey)
}

export function getUserIdFromToken(accessToken: string | null): string | null {
  if (!accessToken) return null

  try {
    return jwtDecode<JwtPayload>(accessToken).sub ?? null
  } catch {
    return null
  }
}

