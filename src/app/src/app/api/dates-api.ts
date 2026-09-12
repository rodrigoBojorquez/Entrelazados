import { datePlannerApi } from './base-api'
import type { DatePlan, DatePlanSummary } from '@/app/types'

interface CreateDatePlanResponse {
  id: string
}

interface AddItineraryItemRequest {
  datePlanId: string
  name: string
  latitude: number | null
  longitude: number | null
  startsAt: string | null
}

interface RemoveItineraryItemRequest {
  datePlanId: string
  itemId: string
}

interface ScheduleDatePlanRequest {
  datePlanId: string
  scheduledFor: string
}

const datesApi = datePlannerApi.injectEndpoints({
  endpoints: (builder) => ({
    createDatePlan: builder.mutation<CreateDatePlanResponse, void>({
      query: () => ({ url: '/api/dates', method: 'POST' }),
      invalidatesTags: [{ type: 'DatePlan', id: 'LIST' }],
    }),
    getDatePlans: builder.query<DatePlanSummary[], void>({
      query: () => '/api/dates',
      providesTags: (result) => [
        { type: 'DatePlan', id: 'LIST' },
        ...(result?.map(({ id }) => ({ type: 'DatePlan' as const, id })) ?? []),
      ],
    }),
    getDatePlan: builder.query<DatePlan, string>({
      query: (datePlanId) => `/api/dates/${datePlanId}`,
      providesTags: (_result, _error, id) => [{ type: 'DatePlan', id }],
    }),
    addItineraryItem: builder.mutation<void, AddItineraryItemRequest>({
      query: ({ datePlanId, ...body }) => ({
        url: `/api/dates/${datePlanId}/itinerary`,
        method: 'POST',
        body,
      }),
      invalidatesTags: (_result, _error, arg) => [
        { type: 'DatePlan', id: arg.datePlanId },
        { type: 'DatePlan', id: 'LIST' },
      ],
    }),
    removeItineraryItem: builder.mutation<void, RemoveItineraryItemRequest>({
      query: ({ datePlanId, itemId }) => ({
        url: `/api/dates/${datePlanId}/itinerary/${itemId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, arg) => [
        { type: 'DatePlan', id: arg.datePlanId },
        { type: 'DatePlan', id: 'LIST' },
      ],
    }),
    scheduleDatePlan: builder.mutation<void, ScheduleDatePlanRequest>({
      query: ({ datePlanId, scheduledFor }) => ({
        url: `/api/dates/${datePlanId}/schedule`,
        method: 'PUT',
        body: { scheduledFor },
      }),
      invalidatesTags: (_result, _error, arg) => [
        { type: 'DatePlan', id: arg.datePlanId },
        { type: 'DatePlan', id: 'LIST' },
      ],
    }),
    returnDatePlanToPlanning: builder.mutation<void, string>({
      query: (datePlanId) => ({
        url: `/api/dates/${datePlanId}/planning`,
        method: 'PUT',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'DatePlan', id },
        { type: 'DatePlan', id: 'LIST' },
      ],
    }),
    attendDatePlan: builder.mutation<void, string>({
      query: (datePlanId) => ({
        url: `/api/dates/${datePlanId}/attendance`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'DatePlan', id },
        { type: 'DatePlan', id: 'LIST' },
        'TurnGame',
      ],
    }),
    cancelDatePlan: builder.mutation<void, string>({
      query: (datePlanId) => ({ url: `/api/dates/${datePlanId}`, method: 'DELETE' }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'DatePlan', id },
        { type: 'DatePlan', id: 'LIST' },
      ],
    }),
    setCollaborativeEditing: builder.mutation<void, { datePlanId: string; isCollaborative: boolean }>({
      query: ({ datePlanId, isCollaborative }) => ({
        url: `/api/dates/${datePlanId}/collaboration`,
        method: 'PUT',
        body: { isCollaborative },
      }),
      invalidatesTags: (_result, _error, arg) => [
        { type: 'DatePlan', id: arg.datePlanId },
        { type: 'DatePlan', id: 'LIST' },
      ],
    }),
  }),
})

export const {
  useCreateDatePlanMutation,
  useGetDatePlansQuery,
  useGetDatePlanQuery,
  useAddItineraryItemMutation,
  useRemoveItineraryItemMutation,
  useScheduleDatePlanMutation,
  useReturnDatePlanToPlanningMutation,
  useAttendDatePlanMutation,
  useCancelDatePlanMutation,
  useSetCollaborativeEditingMutation,
} = datesApi
