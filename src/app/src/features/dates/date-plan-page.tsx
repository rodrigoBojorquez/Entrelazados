import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowLeft, CalendarCheck, CalendarDays, Clock3, LockKeyhole, MapPin, PencilLine, Plus, Trash2, UsersRound, X } from 'lucide-react'
import { useCallback, useState } from 'react'
import { useForm } from 'react-hook-form'
import { useNavigate, useParams } from 'react-router'
import { z } from 'zod'
import {
  useAddItineraryItemMutation,
  useAttendDatePlanMutation,
  useCancelDatePlanMutation,
  useGetDatePlanQuery,
  useGetProfileQuery,
  useRemoveItineraryItemMutation,
  useReturnDatePlanToPlanningMutation,
  useScheduleDatePlanMutation,
  useSetCollaborativeEditingMutation,
} from '@/app/api'
import { formatDate, formatTime } from '@/app/format'
import type { ItineraryItem } from '@/app/types'
import { DateMap, LocationPicker } from '@/components/date-map'
import { FieldError, FieldLabel, inputClassName } from '@/components/form-field'
import { EmptyState, LoadingState, Notice } from '@/components/page-state'
import { StatusBadge } from '@/components/status-badge'
import { Button } from '@/components/ui/button'

const itinerarySchema = z.object({
  name: z.string().trim().min(1, 'Escribe el nombre del punto.').max(200),
  startsAt: z.string(),
})

type ItineraryValues = z.infer<typeof itinerarySchema>

interface Coordinates {
  latitude: number
  longitude: number
}

export function DatePlanPage() {
  const { datePlanId = '' } = useParams()
  const navigate = useNavigate()
  const { data: profile } = useGetProfileQuery()
  const { data: plan, isLoading, error } = useGetDatePlanQuery(datePlanId, { skip: !datePlanId })
  const [addItem, addState] = useAddItineraryItemMutation()
  const [removeItem, removeState] = useRemoveItineraryItemMutation()
  const [schedulePlan, scheduleState] = useScheduleDatePlanMutation()
  const [returnToPlanning, returnToPlanningState] = useReturnDatePlanToPlanningMutation()
  const [attendPlan, attendState] = useAttendDatePlanMutation()
  const [cancelPlan, cancelState] = useCancelDatePlanMutation()
  const [setCollaborativeEditing, collaborationState] = useSetCollaborativeEditingMutation()
  const [showAdd, setShowAdd] = useState(false)
  const [coordinates, setCoordinates] = useState<Coordinates | null>(null)
  const [scheduledFor, setScheduledFor] = useState('')
  const { register, handleSubmit, reset, formState: { errors } } = useForm<ItineraryValues>({
    resolver: zodResolver(itinerarySchema),
    defaultValues: { name: '', startsAt: '' },
  })
  const onCoordinatesChange = useCallback((value: Coordinates) => setCoordinates(value), [])

  if (isLoading) return <LoadingState label="Abriendo el plan…" />
  if (error || !plan) {
    return (
      <EmptyState
        icon={<CalendarDays />}
        title="No encontramos este plan"
        description="Puede que ya no exista o que no pertenezca a tu pareja."
        action={<Button onClick={() => navigate('/citas')}>Volver a Citas</Button>}
      />
    )
  }

  const isPlanning = plan.status === 'Planning'
  const isBooked = plan.status === 'Booked'
  const isPlanner = plan.partnerWhoPlannedId === profile?.id
  const canEdit = isPlanner || plan.isCollaborative
  const mutationError = addState.error ?? removeState.error ?? scheduleState.error ?? returnToPlanningState.error ?? attendState.error ?? cancelState.error ?? collaborationState.error

  const submitItem = handleSubmit(async (values) => {
    try {
      await addItem({
        datePlanId: plan.id,
        name: values.name,
        startsAt: values.startsAt ? `${values.startsAt}:00` : null,
        latitude: coordinates?.latitude ?? null,
        longitude: coordinates?.longitude ?? null,
      }).unwrap()
      reset()
      setCoordinates(null)
      setShowAdd(false)
    } catch {
      // El middleware global muestra el detalle enviado por la API.
    }
  })

  const remove = async (item: ItineraryItem) => {
    if (!window.confirm(`¿Quitar “${item.name}” del itinerario?`)) return
    await removeItem({ datePlanId: plan.id, itemId: item.id })
  }

  const schedule = async () => {
    if (!scheduledFor || plan.itinerary.length === 0) return
    await schedulePlan({ datePlanId: plan.id, scheduledFor })
  }

  const cancel = async () => {
    if (!window.confirm('¿Cancelar esta cita? El itinerario quedará en modo de solo lectura.')) return
    await cancelPlan(plan.id)
  }

  const attend = async () => {
    if (!window.confirm('¿Marcar esta cita como realizada?')) return
    await attendPlan(plan.id)
  }

  const editAgain = async () => {
    if (!window.confirm('¿Volver a editar esta cita? La fecha programada se quitará y tendrás que programarla nuevamente.')) return
    await returnToPlanning(plan.id)
  }

  const toggleCollaboration = async () => {
    await setCollaborativeEditing({
      datePlanId: plan.id,
      isCollaborative: !plan.isCollaborative,
    })
  }

  return (
    <div className="flex flex-col gap-6">
      <button onClick={() => navigate('/citas')} className="flex min-h-11 items-center gap-2 self-start text-sm font-semibold text-plum"><ArrowLeft />Volver a Citas</button>
      <div>
        <div className="flex flex-wrap items-center gap-3">
          <StatusBadge status={plan.status} />
          <span className="text-xs text-muted-foreground">{plan.partnerWhoPlannedId === profile?.id ? 'Planeada por ti' : 'Planeada por tu pareja'}</span>
        </div>
        <h1 className="mt-3 font-serif text-3xl font-semibold text-ink">{plan.itinerary[0]?.name ?? 'Su próxima aventura'}</h1>
        <p className="mt-2 text-sm text-muted-foreground">{plan.scheduledFor ? formatDate(plan.scheduledFor) : 'Aún sin fecha'}</p>
      </div>

      {mutationError && <Notice>No pudimos guardar el cambio. Revisa los datos e inténtalo otra vez.</Notice>}
      {isPlanning && isPlanner && (
        <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
          <div className="flex items-start justify-between gap-4">
            <div className="flex min-w-0 items-start gap-3">
              <span className="flex size-10 shrink-0 items-center justify-center rounded-2xl bg-lilac text-plum">{plan.isCollaborative ? <UsersRound /> : <LockKeyhole />}</span>
              <div>
                <h2 className="font-semibold text-ink">{plan.isCollaborative ? 'Plan colaborativo' : 'Tu sorpresa, a tu manera'}</h2>
                <p className="mt-1 text-sm leading-6 text-muted-foreground">{plan.isCollaborative ? 'Tu pareja también puede editar, programar o cancelar este plan.' : 'Sólo tú puedes editarlo. Así tienes espacio para preparar cada detalle con amor.'}</p>
              </div>
            </div>
            <button
              type="button"
              role="switch"
              aria-checked={plan.isCollaborative}
              aria-label="Permitir edición colaborativa"
              disabled={collaborationState.isLoading}
              onClick={toggleCollaboration}
              className={`relative mt-1 h-7 w-12 shrink-0 rounded-full transition ${plan.isCollaborative ? 'bg-plum' : 'bg-muted'}`}
            >
              <span className={`absolute top-1 size-5 rounded-full bg-white shadow-sm transition ${plan.isCollaborative ? 'left-6' : 'left-1'}`} />
            </button>
          </div>
        </section>
      )}
      {isPlanning && !isPlanner && !plan.isCollaborative && <Notice tone="info">Tu pareja está preparando esta cita como una sorpresa. Podrás verla, pero sólo quien ganó la partida puede editarla.</Notice>}
      {plan.itinerary.some((item) => item.latitude !== null) && <DateMap items={plan.itinerary} className="h-72" />}

      <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
        <div className="flex items-center justify-between gap-3">
          <div>
            <h2 className="font-serif text-2xl font-semibold text-ink">Itinerario</h2>
            <p className="mt-1 text-sm text-muted-foreground">{isPlanning && canEdit ? (plan.isCollaborative ? 'Ambas personas pueden agregar y quitar puntos.' : 'Este itinerario es tuyo para sorprender a tu pareja.') : 'Este itinerario está en modo de solo lectura.'}</p>
          </div>
          {isPlanning && canEdit && <Button size="sm" onClick={() => setShowAdd(true)}><Plus />Agregar</Button>}
        </div>
        {plan.itinerary.length === 0 ? (
          <div className="mt-6 rounded-2xl bg-lilac p-5 text-center">
            <MapPin className="mx-auto text-plum" />
            <p className="mt-3 font-semibold text-ink">Empiecen con algo que les emocione</p>
            <p className="mt-1 text-sm text-muted-foreground">Puede ser un lugar o simplemente un momento.</p>
          </div>
        ) : (
          <div className="mt-6 space-y-5">
            {plan.itinerary.map((item, index) => (
              <div key={item.id} className="relative flex items-start gap-3">
                {index < plan.itinerary.length - 1 && <span className="absolute left-3.5 top-8 h-[calc(100%+0.5rem)] w-px bg-lavender" />}
                <span className="z-10 flex size-7 shrink-0 items-center justify-center rounded-full bg-plum text-xs font-bold text-white">{item.order}</span>
                <div className="min-w-0 flex-1">
                  <p className="text-sm font-semibold text-ink">{item.name}</p>
                  <div className="mt-1 flex flex-wrap gap-3 text-xs text-muted-foreground">
                    <span className="flex items-center gap-1"><Clock3 className="size-3" />{formatTime(item.startsAt) ?? 'Sin hora'}</span>
                    <span className="flex items-center gap-1"><MapPin className="size-3" />{item.latitude !== null ? 'Con ubicación' : 'Sin ubicación'}</span>
                  </div>
                </div>
                {isPlanning && canEdit && (
                  <button onClick={() => remove(item)} disabled={removeState.isLoading} aria-label={`Quitar ${item.name}`} className="flex size-10 items-center justify-center rounded-xl text-danger hover:bg-danger/10">
                    <Trash2 className="size-4" />
                  </button>
                )}
              </div>
            ))}
          </div>
        )}
      </section>

      {showAdd && isPlanning && canEdit && (
        <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
          <div className="flex items-center justify-between">
            <h2 className="font-serif text-xl font-semibold text-ink">Agregar punto</h2>
            <button onClick={() => setShowAdd(false)} aria-label="Cerrar" className="flex size-10 items-center justify-center rounded-xl text-muted-foreground hover:bg-muted"><X /></button>
          </div>
          <form onSubmit={submitItem} className="mt-5 space-y-4" noValidate>
            <FieldLabel>
              Nombre
              <input {...register('name')} maxLength={200} className={inputClassName} placeholder="Ej. Cena frente al mar" aria-invalid={Boolean(errors.name)} />
              <FieldError>{errors.name?.message}</FieldError>
            </FieldLabel>
            <FieldLabel>Hora de inicio <span className="font-normal text-muted-foreground">(opcional)</span><input {...register('startsAt')} type="time" className={inputClassName} /></FieldLabel>
            <div>
              <p className="text-sm font-semibold text-ink">Ubicación <span className="font-normal text-muted-foreground">(opcional)</span></p>
              <p className="mb-3 mt-1 text-xs text-muted-foreground">Toca el mapa o usa tu ubicación. Puedes arrastrar el pin para afinarla.</p>
              <LocationPicker value={coordinates} onChange={onCoordinatesChange} />
              {coordinates && <div className="mt-2 flex items-center justify-between rounded-xl bg-lilac px-3 py-2 text-xs text-plum"><span>Ubicación seleccionada</span><button type="button" onClick={() => setCoordinates(null)} className="font-bold">Quitar</button></div>}
            </div>
            <Button type="submit" disabled={addState.isLoading} className="w-full">{addState.isLoading ? 'Guardando…' : 'Agregar al itinerario'}</Button>
          </form>
        </section>
      )}

      {isPlanning && canEdit && (
        <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
          <h2 className="font-serif text-xl font-semibold text-ink">Programar la cita</h2>
          <p className="mt-1 text-sm text-muted-foreground">Necesitan una fecha y al menos un punto en el itinerario.</p>
          <FieldLabel className="mt-4">Fecha<input type="date" value={scheduledFor} onChange={(event) => setScheduledFor(event.target.value)} className={inputClassName} /></FieldLabel>
          <Button onClick={schedule} disabled={!scheduledFor || plan.itinerary.length === 0 || scheduleState.isLoading} className="mt-4 w-full"><CalendarCheck />{scheduleState.isLoading ? 'Programando…' : 'Programar cita'}</Button>
        </section>
      )}

      {isBooked && canEdit && <Button variant="outline" onClick={editAgain} disabled={returnToPlanningState.isLoading} className="w-full"><PencilLine />{returnToPlanningState.isLoading ? 'Volviendo a planeación…' : 'Volver a editar'}</Button>}
      {isBooked && <Button onClick={attend} disabled={attendState.isLoading} className="w-full"><CalendarCheck />Marcar como atendida</Button>}
      {((isPlanning && canEdit) || (isBooked && canEdit)) && <Button variant="destructive" onClick={cancel} disabled={cancelState.isLoading} className="w-full">Cancelar cita</Button>}
      {(plan.status === 'Canceled' || plan.status === 'Attended') && <Notice tone={plan.status === 'Attended' ? 'success' : 'info'}>{plan.status === 'Attended' ? 'Esta cita ya forma parte de su historia.' : 'Esta cita fue cancelada y se conserva como referencia.'}</Notice>}
    </div>
  )
}
