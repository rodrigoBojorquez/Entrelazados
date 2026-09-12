import { datePlannerApi } from './base-api'
import type { CurrentScheduledTurnGame, TurnGameSummary } from '@/app/types'

const turnGamesApi = datePlannerApi.injectEndpoints({
  endpoints: (builder) => ({
    getTurnGameHistory: builder.query<TurnGameSummary[], void>({
      query: () => '/api/turn-games',
      providesTags: ['TurnGame'],
    }),
    getCurrentScheduledTurnGame: builder.query<CurrentScheduledTurnGame, void>({
      query: () => '/api/turn-games/current-scheduled',
      providesTags: ['TurnGame'],
    }),
  }),
})

export const {
  useGetTurnGameHistoryQuery,
  useGetCurrentScheduledTurnGameQuery,
} = turnGamesApi
