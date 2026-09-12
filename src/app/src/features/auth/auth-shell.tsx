import { ShieldCheck } from 'lucide-react'
import { BrandLogo } from '@/components/brand-logo'

interface AuthShellProps {
  children: React.ReactNode
}

export function AuthShell({ children }: AuthShellProps) {
  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-5 py-8">
      <div className="w-full max-w-md">
        <div className="flex justify-center">
          <BrandLogo />
        </div>
        {children}
        <p className="mt-5 flex items-center justify-center gap-2 text-center text-xs text-muted-foreground">
          <ShieldCheck className="size-4 text-success" />
          Sus datos son privados y solo de ustedes.
        </p>
      </div>
    </main>
  )
}
