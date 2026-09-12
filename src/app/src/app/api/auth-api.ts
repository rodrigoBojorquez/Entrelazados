import { datePlannerApi } from './base-api'
import type { ActiveSession, AuthenticationResponse } from '@/app/types'

interface LoginRequest {
  email: string
  password: string
}

interface RegisterUserRequest {
  email: string
  firstName: string
  lastName: string
  password: string
}

interface RegisterUserResponse {
  id: string
}

interface RevokeCurrentSessionRequest {
  refreshToken: string
}

const authApi = datePlannerApi.injectEndpoints({
  endpoints: (builder) => ({
    registerUser: builder.mutation<RegisterUserResponse, RegisterUserRequest>({
      query: (body) => ({ url: '/api/users', method: 'POST', body }),
    }),
    login: builder.mutation<AuthenticationResponse, LoginRequest>({
      query: (body) => ({ url: '/api/authentication/login', method: 'POST', body }),
    }),
    revokeCurrentSession: builder.mutation<void, RevokeCurrentSessionRequest>({
      query: (body) => ({ url: '/api/authentication/revoke', method: 'POST', body }),
    }),
    getActiveSessions: builder.query<ActiveSession[], void>({
      query: () => '/api/authentication/sessions',
      providesTags: ['Sessions'],
    }),
    revokeAllSessions: builder.mutation<void, void>({
      query: () => ({ url: '/api/authentication/sessions', method: 'DELETE' }),
      invalidatesTags: ['Sessions'],
    }),
  }),
})

export const {
  useRegisterUserMutation,
  useLoginMutation,
  useRevokeCurrentSessionMutation,
  useGetActiveSessionsQuery,
  useRevokeAllSessionsMutation,
} = authApi
