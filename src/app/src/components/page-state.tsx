import { AlertCircle, LoaderCircle } from 'lucide-react'
import { cn } from '@/lib/utils'

interface LoadingStateProps {
  label?: string
}

interface NoticeProps {
  children: React.ReactNode
  tone?: 'error' | 'success' | 'info'
  className?: string
}

interface EmptyStateProps {
  icon: React.ReactNode
  title: string
  description: string
  action?: React.ReactNode
}

export function LoadingState({ label = 'Cargando…' }: LoadingStateProps) {
  return (
    <div className="flex min-h-44 flex-col items-center justify-center gap-3 text-sm text-muted-foreground">
      <LoaderCircle className="size-6 animate-spin text-plum" />
      <span>{label}</span>
    </div>
  )
}

export function Notice({
  children,
  tone = 'error',
  className,
}: NoticeProps) {
  const tones = {
    error: 'border-danger/20 bg-danger/10 text-danger',
    success: 'border-success/20 bg-success-soft text-success',
    info: 'border-lavender bg-lilac text-plum',
  }

  return (
    <div className={cn('flex items-start gap-2 rounded-xl border px-3 py-3 text-sm', tones[tone], className)} role={tone === 'error' ? 'alert' : 'status'}>
      <AlertCircle className="mt-0.5 size-4 shrink-0" />
      <div>{children}</div>
    </div>
  )
}

export function EmptyState({
  icon,
  title,
  description,
  action,
}: EmptyStateProps) {
  return (
    <div className="rounded-3xl border border-lavender bg-white p-6 text-center shadow-card">
      <div className="mx-auto flex size-12 items-center justify-center rounded-2xl bg-lilac text-plum">{icon}</div>
      <h2 className="mt-4 font-serif text-2xl font-semibold text-ink">{title}</h2>
      <p className="mx-auto mt-2 max-w-md text-sm leading-6 text-muted-foreground">{description}</p>
      {action && <div className="mt-5 flex justify-center">{action}</div>}
    </div>
  )
}
