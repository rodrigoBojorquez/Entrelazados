import { datePlannerApi } from './base-api'
import type { UserProfile } from '@/app/types'

interface ChangeNameRequest {
  firstName: string
  lastName: string
}

interface ActivityRequest {
  activity: string
}

interface FoodRequest {
  food: string
}

const usersApi = datePlannerApi.injectEndpoints({
  endpoints: (builder) => ({
    getProfile: builder.query<UserProfile, void>({
      query: () => '/api/users/me',
      providesTags: ['Profile'],
    }),
    changeName: builder.mutation<void, ChangeNameRequest>({
      query: (body) => ({ url: '/api/users/me/name', method: 'PUT', body }),
      invalidatesTags: ['Profile'],
    }),
    addFavoriteActivity: builder.mutation<void, ActivityRequest>({
      query: (body) => ({
        url: '/api/users/me/dating-preferences/favorite-activities',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Profile'],
    }),
    removeFavoriteActivity: builder.mutation<void, ActivityRequest>({
      query: (body) => ({
        url: '/api/users/me/dating-preferences/favorite-activities',
        method: 'DELETE',
        body,
      }),
      invalidatesTags: ['Profile'],
    }),
    addAvoidedActivity: builder.mutation<void, ActivityRequest>({
      query: (body) => ({
        url: '/api/users/me/dating-preferences/avoided-activities',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Profile'],
    }),
    removeAvoidedActivity: builder.mutation<void, ActivityRequest>({
      query: (body) => ({
        url: '/api/users/me/dating-preferences/avoided-activities',
        method: 'DELETE',
        body,
      }),
      invalidatesTags: ['Profile'],
    }),
    addFoodPreference: builder.mutation<void, FoodRequest>({
      query: (body) => ({
        url: '/api/users/me/dating-preferences/food-preferences',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Profile'],
    }),
    removeFoodPreference: builder.mutation<void, FoodRequest>({
      query: (body) => ({
        url: '/api/users/me/dating-preferences/food-preferences',
        method: 'DELETE',
        body,
      }),
      invalidatesTags: ['Profile'],
    }),
  }),
})

export const {
  useGetProfileQuery,
  useChangeNameMutation,
  useAddFavoriteActivityMutation,
  useRemoveFavoriteActivityMutation,
  useAddAvoidedActivityMutation,
  useRemoveAvoidedActivityMutation,
  useAddFoodPreferenceMutation,
  useRemoveFoodPreferenceMutation,
} = usersApi
