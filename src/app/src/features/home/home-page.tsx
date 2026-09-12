import {
  ArrowRight,
  CalendarDays,
  Gamepad2,
  HeartHandshake,
  Lightbulb,
  MapPin,
} from 'lucide-react'
import { useLocation, useNavigate } from 'react-router'
import { useState } from 'react'
import {
  useGetCurrentCoupleQuery,
  useGetCurrentPartnerQuery,
  useGetCurrentScheduledTurnGameQuery,
  useGetDatePlanQuery,
  useGetDatePlansQuery,
  useGetProfileQuery,
} from '@/app/api'
import { daysUntil, formatDate, formatTime } from '@/app/format'
import { useTurnGameHub } from '@/app/turn-game-hub'
import type { TurnGameType } from '@/app/types'
import { DateMap } from '@/components/date-map'
import { EmptyState, LoadingState, Notice } from '@/components/page-state'
import { StatusBadge } from '@/components/status-badge'
import { Button } from '@/components/ui/button'
import { ScheduledGameCard } from '@/features/games/scheduled-game-card'

export function HomePage() {
  const navigate = useNavigate()
  const location = useLocation()
  const { data: profile, isLoading: profileLoading } = useGetProfileQuery()
  const { data: couple, isLoading: coupleLoading } = useGetCurrentCoupleQuery()
  const { data: partner, isLoading: partnerLoading } = useGetCurrentPartnerQuery(undefined, {
    skip: !couple?.isActive,
  })
  const { data: plans = [], isLoading: datePlanLoading } = useGetDatePlansQuery(undefined, {
    skip: !couple?.isActive,
  })
  const activePlan = plans.find((plan) => plan.status === 'Planning' || plan.status === 'Booked')
  const { data: datePlan, isLoading: activePlanLoading } = useGetDatePlanQuery(activePlan?.id ?? '', {
    skip: !activePlan,
  })
  const { data: scheduledGame, isLoading: scheduledGameLoading } = useGetCurrentScheduledTurnGameQuery(undefined, {
    skip: !couple?.isActive,
  })
  const { connectionStatus, selectScheduledGame } = useTurnGameHub()
  const [selecting, setSelecting] = useState<TurnGameType | null>(null)

  if (profileLoading || coupleLoading || partnerLoading || datePlanLoading || activePlanLoading || scheduledGameLoading) return <LoadingState label="Preparando su espacio…" />

  const selectGame = async (gameType: TurnGameType) => {
    const pendingGame = scheduledGame?.game
    if (!pendingGame) return
    setSelecting(gameType)
    try {
      const selected = await selectScheduledGame(pendingGame.id, gameType)
      navigate(`/juego/${selected.id}`)
    } catch {
      setSelecting(null)
    }
  }

  if (!couple?.isActive) {
    return (
      <div className="flex flex-col gap-6">
        <WelcomeHeading firstName={profile?.firstName} />
        <EmptyState
          icon={<HeartHandshake />}
          title="Este espacio comienza con dos"
          description="Conecta tu cuenta con la de tu pareja para planear citas y jugar juntos."
          action={<Button onClick={() => navigate('/conectar')}>Conectar con mi pareja <ArrowRight /></Button>}
        />
      </div>
    )
  }

  const futureDays = datePlan?.scheduledFor ? daysUntil(datePlan.scheduledFor) : null
  const locatedItems = datePlan?.itinerary.filter((item) => item.latitude !== null && item.longitude !== null) ?? []

  return (
    <div className="flex flex-col gap-6">
      <WelcomeHeading firstName={profile?.firstName} />
      {(location.state as { linked?: boolean } | null)?.linked && <Notice tone="success"><strong>Ya están entrelazados.</strong> Su primer juego para decidir quién prepara la cita quedó agendado.</Notice>}
      {scheduledGame?.game && (
        <ScheduledGameCard
          game={scheduledGame.game}
          connectionStatus={connectionStatus}
          selecting={selecting}
          onSelect={selectGame}
          onContinue={() => navigate(`/juego/${scheduledGame.game!.id}`)}
        />
      )}

      {datePlan && datePlan.status === 'Planning' && (
        <section className="overflow-hidden rounded-3xl border border-lavender bg-white shadow-card">
          <div className="p-5 sm:p-6">
            <StatusBadge status={datePlan.status} />
            <h2 className="mt-3 font-serif text-2xl font-semibold text-ink">Su próxima cita está tomando forma</h2>
            <p className="mt-2 text-sm text-muted-foreground">
              {datePlan.partnerWhoPlannedId === profile?.id ? 'Tú estás planeando' : 'Tu pareja está planeando'} · {datePlan.itinerary.length} {datePlan.itinerary.length === 1 ? 'punto' : 'puntos'}
            </p>
            <Button onClick={() => navigate(`/citas/${datePlan.id}`)} className="mt-5 w-full">Continuar planeando <ArrowRight /></Button>
          </div>
        </section>
      )}

      {datePlan && datePlan.status === 'Booked' && (
        <section className="overflow-hidden rounded-3xl border border-lavender bg-white shadow-card">
          <div className="border-b border-lavender/70 px-5 py-4 sm:px-6">
            <StatusBadge status={datePlan.status} />
            <p className="mt-2 text-xs text-muted-foreground">{formatDate(datePlan.scheduledFor)}{futureDays !== null && futureDays >= 0 ? ` · En ${futureDays} días` : ''}</p>
          </div>
          <div className="p-5 sm:p-6">
            <h2 className="font-serif text-2xl font-semibold text-ink">Próxima cita</h2>
            <p className="mt-2 text-sm text-muted-foreground">{datePlan.partnerWhoPlannedId === profile?.id ? 'La planeaste tú.' : 'Tu pareja está preparando este momento.'}</p>
            {locatedItems.length > 0 && <div className="mt-5"><DateMap items={datePlan.itinerary} /></div>}
            <div className="mt-5 space-y-3">
              {datePlan.itinerary.slice(0, 2).map((item) => (
                <div key={item.id} className="flex items-start gap-3">
                  <span className="flex size-7 shrink-0 items-center justify-center rounded-full bg-plum text-xs font-bold text-white">{item.order}</span>
                  <div><p className="text-sm font-semibold text-ink">{item.name}</p><p className="mt-1 flex items-center gap-2 text-xs text-muted-foreground">{formatTime(item.startsAt) ?? 'Sin hora'}{item.latitude !== null && <><MapPin className="size-3" />Con ubicación</>}</p></div>
                </div>
              ))}
            </div>
            <Button onClick={() => navigate(`/citas/${datePlan.id}`)} className="mt-5 w-full">Ver plan <ArrowRight /></Button>
          </div>
        </section>
      )}

      {!datePlan && !scheduledGame?.game && (
        <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
          <div className="flex size-12 items-center justify-center rounded-2xl bg-pink-soft text-plum"><CalendarDays /></div>
          <h2 className="mt-4 font-serif text-2xl font-semibold text-ink">Su siguiente momento está por comenzar</h2>
          <p className="mt-2 text-sm leading-6 text-muted-foreground">Cuando termine la partida programada aparecerá aquí el plan de la persona ganadora.</p>
        </section>
      )}

      <div className="grid grid-cols-2 gap-3">
        <button onClick={() => navigate('/juego')} className="rounded-2xl bg-lilac p-4 text-left transition hover:-translate-y-0.5"><Gamepad2 className="size-5 text-plum" /><p className="mt-3 text-sm font-semibold text-ink">Juega con tu pareja</p></button>
        <button onClick={() => navigate('/inspiracion')} className="rounded-2xl bg-pink-soft p-4 text-left transition hover:-translate-y-0.5"><Lightbulb className="size-5 text-plum" /><p className="mt-3 text-sm font-semibold text-ink">Necesito inspiración</p></button>
      </div>

      <div className="rounded-2xl border border-lavender bg-white p-4">
        <p className="flex items-center gap-2 text-sm font-semibold text-ink"><HeartHandshake className="size-4 text-pink" />Juntos en Entrelazados</p>
        <p className="mt-2 font-serif text-lg font-semibold text-ink">{profile?.firstName} y {partner?.partnerName ?? 'tu pareja'}</p>
        <p className="mt-1 text-xs text-muted-foreground">Desde {formatDate(couple.beginRelationshipAt, { dateStyle: 'medium' })}</p>
        {partner?.partnerPreferences.favoriteActivities.length ? (
          <p className="mt-3 text-xs leading-5 text-muted-foreground">A {partner.partnerName} le gusta: {partner.partnerPreferences.favoriteActivities.slice(0, 3).join(' · ')}</p>
        ) : null}
      </div>
    </div>
  )
}

interface WelcomeHeadingProps {
  firstName?: string
}

function WelcomeHeading({ firstName }: WelcomeHeadingProps) {
  return (
    <div>
      <p className="text-sm text-muted-foreground">{formatDate(new Date().toISOString(), { weekday: 'long', day: 'numeric', month: 'long' })}</p>
      <h1 className="mt-1 font-serif text-3xl font-semibold text-ink">Buenos días{firstName ? `, ${firstName}` : ''}</h1>
      <p className="mt-2 text-sm leading-6 text-muted-foreground">Un plan bonito puede empezar con una idea sencilla.</p>
    </div>
  )
}
