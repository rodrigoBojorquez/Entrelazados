import { ArrowRight, CalendarDays, Heart, History, LockKeyhole, UsersRound } from 'lucide-react'
import { useNavigate } from 'react-router'
import { useGetCurrentCoupleQuery, useGetDatePlansQuery, useGetProfileQuery } from '@/app/api'
import { formatDate } from '@/app/format'
import type { DatePlanSummary } from '@/app/types'
import { EmptyState, LoadingState } from '@/components/page-state'
import { StatusBadge } from '@/components/status-badge'
import { Button } from '@/components/ui/button'

interface DatePlanCardProps {
  plan: DatePlanSummary
  currentUserId?: string
  onOpen: () => void
}

export function DatesPage() {
  const navigate = useNavigate()
  const { data: couple, isLoading: coupleLoading } = useGetCurrentCoupleQuery()
  const { data: profile } = useGetProfileQuery()
  const { data: plans = [], isLoading: plansLoading } = useGetDatePlansQuery(undefined, {
    skip: !couple?.isActive,
  })

  if (coupleLoading || plansLoading) return <LoadingState />
  if (!couple?.isActive) {
    return (
      <EmptyState
        icon={<CalendarDays />}
        title="Primero conecta con tu pareja"
        description="Las citas viven en su espacio compartido."
        action={<Button onClick={() => navigate('/conectar')}>Conectar ahora <ArrowRight /></Button>}
      />
    )
  }

  const activePlans = plans.filter((plan) => plan.status === 'Planning' || plan.status === 'Booked')
  const history = plans.filter((plan) => plan.status === 'Attended' || plan.status === 'Canceled')

  return (
    <div className="flex flex-col gap-8">
      <div>
        <p className="text-sm text-muted-foreground">Su historia, paso a paso</p>
        <h1 className="mt-1 font-serif text-3xl font-semibold text-ink">Citas</h1>
        <p className="mt-2 text-sm leading-6 text-muted-foreground">Cada plan nace de una partida especial. Quien gana recibe el turno para preparar la sorpresa.</p>
      </div>

      <section>
        <div className="flex items-center gap-3">
          <span className="flex size-10 items-center justify-center rounded-2xl bg-lilac text-plum"><Heart /></span>
          <div><h2 className="font-serif text-2xl font-semibold text-ink">En camino</h2><p className="mt-1 text-sm text-muted-foreground">La próxima aventura que están preparando.</p></div>
        </div>

        {activePlans.length === 0 ? (
          <EmptyState
            icon={<CalendarDays />}
            title="El siguiente plan espera una victoria"
            description="Completen el juego programado y la persona ganadora recibirá su plan automáticamente."
            action={<Button onClick={() => navigate('/juego')}>Ir a Juegos <ArrowRight /></Button>}
          />
        ) : (
          <div className="mt-4 grid gap-4">
            {activePlans.map((plan) => <DatePlanCard key={plan.id} plan={plan} currentUserId={profile?.id} onOpen={() => navigate(`/citas/${plan.id}`)} />)}
          </div>
        )}
      </section>

      <section>
        <div className="flex items-center gap-3">
          <span className="flex size-10 items-center justify-center rounded-2xl bg-pink-soft text-plum"><History /></span>
          <div><h2 className="font-serif text-2xl font-semibold text-ink">Álbum de citas</h2><p className="mt-1 text-sm text-muted-foreground">Planes atendidos y recuerdos de lo que imaginaron.</p></div>
        </div>

        {history.length === 0 ? (
          <div className="mt-4 rounded-2xl border border-dashed border-lavender bg-white/60 p-5 text-center text-sm text-muted-foreground">Después de su primera cita, su historia aparecerá aquí.</div>
        ) : (
          <div className="mt-4 grid gap-3 sm:grid-cols-2">
            {history.map((plan) => <DatePlanCard key={plan.id} plan={plan} currentUserId={profile?.id} onOpen={() => navigate(`/citas/${plan.id}`)} />)}
          </div>
        )}
      </section>
    </div>
  )
}

function DatePlanCard({ plan, currentUserId, onOpen }: DatePlanCardProps) {
  const plannedByMe = plan.partnerWhoPlannedId === currentUserId

  return (
    <article className="rounded-3xl border border-lavender bg-white p-5 shadow-card">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <StatusBadge status={plan.status} />
        <span className="flex items-center gap-1 text-xs text-muted-foreground">
          {plan.isCollaborative ? <UsersRound className="size-3" /> : <LockKeyhole className="size-3" />}
          {plan.isCollaborative ? 'Colaborativa' : 'Sorpresa individual'}
        </span>
      </div>
      <h3 className="mt-4 font-serif text-xl font-semibold text-ink">{plan.title}</h3>
      <p className="mt-2 text-sm text-muted-foreground">{plan.scheduledFor ? formatDate(plan.scheduledFor) : 'Aún sin fecha'} · {plan.itineraryItemCount} {plan.itineraryItemCount === 1 ? 'momento' : 'momentos'}</p>
      <p className="mt-2 text-xs text-muted-foreground">{plannedByMe ? 'Tu turno de demostrar tu amor' : `Planeada por ${plan.plannerName}`}</p>
      <Button onClick={onOpen} variant="outline" className="mt-5 w-full">Abrir plan <ArrowRight /></Button>
    </article>
  )
}
