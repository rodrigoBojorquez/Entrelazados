import { isRejectedWithValue, type Middleware, type SerializedError } from '@reduxjs/toolkit'
import type { FetchBaseQueryError } from '@reduxjs/toolkit/query'
import { toast } from 'sonner'
import { getApiErrorMessage } from '@/app/errors'

interface QueryActionMeta {
  arg?: {
    type?: string
  }
}

export const apiErrorToastMiddleware: Middleware = () => (next) => (action) => {
  if (isRejectedWithValue(action)) {
    const meta = action.meta as QueryActionMeta
    if (meta.arg?.type === 'mutation') {
      toast.error(getApiErrorMessage(
        action.payload as FetchBaseQueryError | SerializedError | undefined,
      ))
    }
  }

  return next(action)
}
