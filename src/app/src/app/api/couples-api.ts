import { datePlannerApi } from './base-api'
import type { Couple, Partner } from '@/app/types'

interface BeginRelationshipRequest {
  partnerCode: string
}

const couplesApi = datePlannerApi.injectEndpoints({
  endpoints: (builder) => ({
    getCurrentCouple: builder.query<Couple, void>({
      query: () => '/api/couples/current',
      providesTags: ['Couple'],
    }),
    getCurrentPartner: builder.query<Partner, void>({
      query: () => '/api/couples/current/partner',
      providesTags: ['Partner'],
    }),
    beginRelationship: builder.mutation<void, BeginRelationshipRequest>({
      query: (body) => ({ url: '/api/couples', method: 'POST', body }),
      invalidatesTags: ['Couple', 'Partner', 'TurnGame', 'DatePlan'],
    }),
    endRelationship: builder.mutation<void, void>({
      query: () => ({ url: '/api/couples/current', method: 'DELETE' }),
      invalidatesTags: ['Couple', 'Partner'],
      async onQueryStarted(_argument, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled
          // A missing current couple is represented by 404, so clear stale data
          // before mounted queries load the now-disconnected state again.
          dispatch(datePlannerApi.util.resetApiState())
        } catch {
          // The rejected mutation is handled by the global error toast middleware.
        }
      },
    }),
  }),
})

export const {
  useGetCurrentCoupleQuery,
  useGetCurrentPartnerQuery,
  useBeginRelationshipMutation,
  useEndRelationshipMutation,
} = couplesApi
