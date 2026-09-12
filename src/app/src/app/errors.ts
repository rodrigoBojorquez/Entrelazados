import type { FetchBaseQueryError } from '@reduxjs/toolkit/query'
import type { SerializedError } from '@reduxjs/toolkit'
import type { ApiProblem } from './types'

const fallbackMessage = 'No pudimos completar la acción. Inténtalo de nuevo.'

export function getApiErrorMessage(
  error: FetchBaseQueryError | SerializedError | undefined,
  fallback = fallbackMessage,
) {
  if (!error) return fallback

  if ('data' in error) {
    if (typeof error.data === 'string') return error.data
    const problem = error.data as ApiProblem | undefined
    return problem?.errors?.[0]?.description ?? problem?.detail ?? problem?.title ?? fallback
  }

  if ('message' in error && error.message) return error.message
  if ('error' in error && error.error) return error.error
  return fallback
}
