import {
  createApi,
  fetchBaseQuery,
  type BaseQueryApi,
  type BaseQueryFn,
  type FetchArgs,
  type FetchBaseQueryError,
} from '@reduxjs/toolkit/query/react'
import { credentialsReceived, sessionEnded } from '@/app/auth-slice'
import type { AuthenticationResponse } from '@/app/types'

interface AppState {
  auth: {
    session: AuthenticationResponse | null
  }
}

const rawBaseQuery = fetchBaseQuery({
  baseUrl: '/',
  prepareHeaders(headers, api) {
    const state = api.getState() as AppState
    const accessToken = state.auth.session?.accessToken
    if (accessToken) headers.set('authorization', `Bearer ${accessToken}`)
    return headers
  },
})

let refreshPromise: Promise<AuthenticationResponse | null> | null = null

function getRequestUrl(args: string | FetchArgs) {
  return typeof args === 'string' ? args : args.url
}

async function refreshSession(
  api: BaseQueryApi,
  extraOptions: object,
  refreshToken: string,
) {
  if (!refreshPromise) {
    refreshPromise = Promise.resolve(rawBaseQuery(
      {
        url: '/api/authentication/refresh',
        method: 'POST',
        body: { refreshToken },
      },
      api,
      extraOptions,
    )).then((refreshResult) => {
      const currentRefreshToken = (api.getState() as AppState).auth.session?.refreshToken

      if (refreshResult.data && currentRefreshToken === refreshToken) {
        const session = refreshResult.data as AuthenticationResponse
        api.dispatch(credentialsReceived(session))
        return session
      }

      // Only an invalid/expired refresh token ends the session. A temporary network
      // failure keeps it available so the next request can retry transparently.
      if (
        currentRefreshToken === refreshToken &&
        (refreshResult.error?.status === 400 || refreshResult.error?.status === 401)
      ) {
        api.dispatch(sessionEnded())
      }

      return null
    }).finally(() => {
      refreshPromise = null
    })
  }

  return refreshPromise
}

const baseQueryWithRefresh: BaseQueryFn<
  string | FetchArgs,
  unknown,
  FetchBaseQueryError
> = async (args, api, extraOptions) => {
  let result = await rawBaseQuery(args, api, extraOptions)
  const session = (api.getState() as AppState).auth.session
  const isRefreshRequest = getRequestUrl(args) === '/api/authentication/refresh'

  if (result.error?.status !== 401 || !session?.refreshToken || isRefreshRequest) {
    return result
  }

  const refreshed = await refreshSession(api, extraOptions, session.refreshToken)
  if (refreshed) result = await rawBaseQuery(args, api, extraOptions)
  return result
}

export const datePlannerApi = createApi({
  reducerPath: 'datePlannerApi',
  baseQuery: baseQueryWithRefresh,
  tagTypes: ['Profile', 'Couple', 'Partner', 'DatePlan', 'TurnGame', 'Sessions'],
  endpoints: () => ({}),
})
