import { cn } from '@/lib/utils'

interface BrandLogoProps {
  compact?: boolean
  className?: string
}

export function BrandLogo({ compact = false, className }: BrandLogoProps) {
  return (
    <div className={cn('flex items-center gap-2.5', className)}>
      <img
        src="/entrelazados-icon.png"
        alt={compact ? 'Entrelazados' : ''}
        className={compact ? 'size-9 object-contain' : 'size-12 object-contain'}
      />
      {!compact && <span className="font-serif text-xl font-semibold text-ink">Entrelazados</span>}
    </div>
  )
}
