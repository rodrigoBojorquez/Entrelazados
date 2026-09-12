import type { DateStatus } from '@/app/types'
import { statusLabels } from '@/app/format'

const tones: Record<DateStatus, string> = {
  Planning: 'bg-lilac text-plum',
  Booked: 'bg-pink-soft text-plum',
  Canceled: 'bg-danger/10 text-danger',
  Attended: 'bg-success-soft text-success',
}

interface StatusBadgeProps {
  status: DateStatus
}

export function StatusBadge({ status }: StatusBadgeProps) {
  return <span className={`inline-flex rounded-full px-3 py-1 text-xs font-bold ${tones[status]}`}>{statusLabels[status]}</span>
}
