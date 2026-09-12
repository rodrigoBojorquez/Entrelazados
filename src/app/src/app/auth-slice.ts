import { createSlice, type PayloadAction } from '@reduxjs/toolkit'
import { loadStoredSession, removeStoredSession, saveSession } from './auth-storage'
import type { AuthenticationResponse } from './types'

type AuthState = {
  session: AuthenticationResponse | null
}

const initialState: AuthState = {
  session: loadStoredSession(),
}

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    credentialsReceived(state, action: PayloadAction<AuthenticationResponse>) {
      state.session = action.payload
      saveSession(action.payload)
    },
    sessionEnded(state) {
      state.session = null
      removeStoredSession()
    },
  },
})

export const { credentialsReceived, sessionEnded } = authSlice.actions
export const authReducer = authSlice.reducer

