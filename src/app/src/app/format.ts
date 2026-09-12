import type { DateStatus } from './types'

export function formatDate(value: string | null | undefined, options?: Intl.DateTimeFormatOptions) {
  if (!value) return 'Sin fecha'
  const normalized = value.length === 10 ? `${value}T12:00:00` : value
  return new Intl.DateTimeFormat('es-MX', options ?? { dateStyle: 'long' }).format(new Date(normalized))
}

export function formatTime(value: string | null | undefined) {
  if (!value) return null
  return value.slice(0, 5)
}

export const statusLabels: Record<DateStatus, string> = {
  Planning: 'Planeando',
  Booked: 'Programada',
  Canceled: 'Cancelada',
  Attended: 'Realizada',
}

export function daysUntil(value: string) {
  const target = new Date(`${value}T12:00:00`).getTime()
  const today = new Date()
  today.setHours(12, 0, 0, 0)
  return Math.ceil((target - today.getTime()) / 86_400_000)
}

