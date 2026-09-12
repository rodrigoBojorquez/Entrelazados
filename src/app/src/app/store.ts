import { configureStore } from '@reduxjs/toolkit'
import { datePlannerApi } from './api'
import { apiErrorToastMiddleware } from './api-error-toast'
import { authReducer } from './auth-slice'

export const store = configureStore({
  reducer: {
    auth: authReducer,
    [datePlannerApi.reducerPath]: datePlannerApi.reducer,
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(
    datePlannerApi.middleware,
    apiErrorToastMiddleware,
  ),
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
