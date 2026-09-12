import { cn } from '@/lib/utils'

export const inputClassName =
  'mt-2 min-h-12 w-full rounded-xl border border-lavender bg-background px-4 text-sm font-normal text-ink outline-none transition placeholder:text-muted-foreground/70 focus:border-plum focus:ring-2 focus:ring-plum/10 aria-invalid:border-danger'

interface FieldErrorProps {
  children?: React.ReactNode
}

interface FieldLabelProps {
  children: React.ReactNode
  className?: string
}

export function FieldError({ children }: FieldErrorProps) {
  if (!children) return null
  return <p className="mt-1.5 text-xs font-medium text-danger">{children}</p>
}

export function FieldLabel({ children, className }: FieldLabelProps) {
  return <label className={cn('block text-sm font-semibold text-ink', className)}>{children}</label>
}
