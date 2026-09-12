import {
  CalendarDays,
  Gamepad2,
  Home,
  Lightbulb,
  LogOut,
  UserRound,
} from 'lucide-react'
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router'
import { useEffect, useRef } from 'react'
import { toast } from 'sonner'
import { useRevokeCurrentSessionMutation } from '@/app/api'
import { sessionEnded } from '@/app/auth-slice'
import { useAppDispatch, useAppSelector } from '@/app/hooks'
import { BrandLogo } from '@/components/brand-logo'
import { cn } from '@/lib/utils'
import { useTurnGameHub } from '@/app/turn-game-hub'

const navItems = [
  { to: '/', label: 'Inicio', icon: Home, end: true },
  { to: '/citas', label: 'Citas', icon: CalendarDays, end: false },
  { to: '/inspiracion', label: 'Inspiración', icon: Lightbulb, end: false },
  { to: '/juego', label: 'Juego', icon: Gamepad2, end: false },
  { to: '/perfil', label: 'Perfil', icon: UserRound, end: false },
]

const titles: Record<string, string> = {
  '/': 'Inicio',
  '/citas': 'Citas',
  '/inspiracion': 'Inspiración',
  '/juego': 'Juego',
  '/perfil': 'Perfil',
  '/conectar': 'Pareja',
}

export default function Layout() {
  const location = useLocation()
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const session = useAppSelector((state) => state.auth.session)
  const [revokeCurrentSession] = useRevokeCurrentSessionMutation()
  const { scheduledGameId, selectedGameId } = useTurnGameHub()
  const handledSelectedGameId = useRef<string | null>(null)
  const title = titles[location.pathname]
    ?? (location.pathname.startsWith('/citas/') ? 'Plan de cita' : undefined)
    ?? (location.pathname.startsWith('/juego/') ? 'Partida' : 'Entrelazados')

  useEffect(() => {
    if (!scheduledGameId) return
    toast.info('Tienen un nuevo juego pendiente para decidir quién prepara la próxima cita.', {
      action: { label: 'Elegir juego', onClick: () => navigate('/juego') },
    })
  }, [navigate, scheduledGameId])

  useEffect(() => {
    if (!selectedGameId || handledSelectedGameId.current === selectedGameId) return
    handledSelectedGameId.current = selectedGameId
    if (location.pathname !== `/juego/${selectedGameId}`) {
      navigate(`/juego/${selectedGameId}`)
    }
  }, [location.pathname, navigate, selectedGameId])

  const logout = async () => {
    const refreshToken = session?.refreshToken
    dispatch(sessionEnded())
    if (refreshToken) {
      await revokeCurrentSession({ refreshToken })
    }
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-background">
      <header className="sticky top-0 z-30 flex items-center justify-between border-b border-lavender/70 bg-background/95 px-5 py-3 backdrop-blur md:hidden">
        <BrandLogo compact />
        <span className="font-serif text-lg font-semibold text-ink">{title}</span>
        <div className="size-9" aria-hidden="true" />
      </header>

      <div className="flex">
        <aside className="sticky top-0 hidden h-screen w-64 shrink-0 border-r border-lavender/70 bg-white px-5 py-7 md:block">
          <BrandLogo />
          <p className="mt-9 px-3 text-xs font-bold uppercase tracking-[0.18em] text-muted-foreground">Tu espacio compartido</p>
          <nav className="mt-4 flex flex-col gap-2">
            {navItems.map(({ to, label, icon: Icon, end }) => (
              <NavLink
                key={to}
                to={to}
                end={end}
                className={({ isActive }) => cn(
                  'flex min-h-12 items-center gap-3 rounded-xl px-3 text-sm font-semibold transition',
                  isActive ? 'bg-lilac text-plum' : 'text-muted-foreground hover:bg-pink-soft hover:text-plum',
                )}
              >
                <Icon className="size-5" />
                {label}
              </NavLink>
            ))}
          </nav>
          <button onClick={logout} className="mt-16 flex min-h-11 items-center gap-3 px-3 text-sm font-semibold text-muted-foreground hover:text-danger">
            <LogOut className="size-4" />
            Cerrar sesión
          </button>
        </aside>

        <main className="mx-auto w-full max-w-6xl px-5 py-6 pb-28 md:px-10 md:py-10">
          <div className="mx-auto max-w-3xl"><Outlet /></div>
        </main>
      </div>

      <nav className="fixed inset-x-0 bottom-0 z-30 flex justify-around border-t border-lavender/70 bg-white/95 px-2 py-2 backdrop-blur md:hidden">
        {navItems.map(({ to, label, icon: Icon, end }) => (
          <NavLink
            key={to}
            to={to}
            end={end}
            className={({ isActive }) => cn(
              'flex min-w-14 flex-col items-center gap-1 rounded-xl px-2 py-1 text-[10px] font-bold',
              isActive ? 'text-plum' : 'text-muted-foreground',
            )}
          >
            <Icon className="size-5" />
            {label}
          </NavLink>
        ))}
      </nav>
    </div>
  )
}
