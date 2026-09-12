import {zodResolver} from '@hookform/resolvers/zod'
import {
    Check,
    Copy,
    HeartHandshake,
    KeyRound,
    LogOut,
    Plus,
    ShieldCheck,
    Trash2,
    X,
} from 'lucide-react'
import {useEffect, useState} from 'react'
import {useForm} from 'react-hook-form'
import {useNavigate} from 'react-router'
import {z} from 'zod'
import {
    useAddAvoidedActivityMutation,
    useAddFavoriteActivityMutation,
    useAddFoodPreferenceMutation,
    useChangeNameMutation,
    useEndRelationshipMutation,
    useGetActiveSessionsQuery,
    useGetCurrentCoupleQuery, useGetCurrentPartnerQuery,
    useGetProfileQuery,
    useRemoveAvoidedActivityMutation,
    useRemoveFavoriteActivityMutation,
    useRemoveFoodPreferenceMutation,
    useRevokeAllSessionsMutation,
    useRevokeCurrentSessionMutation,
} from '@/app/api'
import {sessionEnded} from '@/app/auth-slice'
import {getApiErrorMessage} from '@/app/errors'
import {formatDate} from '@/app/format'
import {useAppDispatch, useAppSelector} from '@/app/hooks'
import {FieldError, FieldLabel, inputClassName} from '@/components/form-field'
import {LoadingState, Notice} from '@/components/page-state'
import {Button} from '@/components/ui/button'
import {toast} from 'sonner'

const nameSchema = z.object({
    firstName: z.string().trim().min(1, 'Escribe tu nombre.').max(100),
    lastName: z.string().trim().min(1, 'Escribe tu apellido.').max(100),
})

type NameValues = z.infer<typeof nameSchema>

export function ProfilePage() {
    const navigate = useNavigate()
    const dispatch = useAppDispatch()
    const authSession = useAppSelector((state) => state.auth.session)
    const {data: profile, isLoading: profileLoading} = useGetProfileQuery()
    const {data: couple} = useGetCurrentCoupleQuery()
    const {data: sessions = [], isLoading: sessionsLoading} = useGetActiveSessionsQuery()
    const {data: partner, isLoading: partnerLoading, error: partnerError} = useGetCurrentPartnerQuery(undefined, {
        skip: !couple?.isActive,
    })
    const [changeName, changeState] = useChangeNameMutation()
    const [addFavorite] = useAddFavoriteActivityMutation()
    const [removeFavorite] = useRemoveFavoriteActivityMutation()
    const [addAvoided] = useAddAvoidedActivityMutation()
    const [removeAvoided] = useRemoveAvoidedActivityMutation()
    const [addFood] = useAddFoodPreferenceMutation()
    const [removeFood] = useRemoveFoodPreferenceMutation()
    const [revokeCurrent] = useRevokeCurrentSessionMutation()
    const [revokeAll, revokeAllState] = useRevokeAllSessionsMutation()
    const [endRelationship, endRelationshipState] = useEndRelationshipMutation()
    const [saved, setSaved] = useState(false)
    const [preferenceError, setPreferenceError] = useState<string | null>(null)
    const {
        register,
        handleSubmit,
        reset,
        formState: {errors}
    } = useForm<NameValues>({resolver: zodResolver(nameSchema)})

    useEffect(() => {
        if (profile) reset({firstName: profile.firstName, lastName: profile.lastName})
    }, [profile, reset])

    if (profileLoading) return <LoadingState/>
    if (!profile) return <Notice>No pudimos cargar tu perfil.</Notice>

    const saveName = handleSubmit(async (values) => {
        try {
            await changeName(values).unwrap()
            setSaved(true)
        } catch {
            setSaved(false)
        }
    })

    const runPreference = async (operation: () => Promise<unknown>) => {
        setPreferenceError(null)
        try {
            await operation()
        } catch (reason) {
            const queryError = reason as Parameters<typeof getApiErrorMessage>[0]
            setPreferenceError(getApiErrorMessage(queryError))
            throw reason
        }
    }

    const copyCode = async () => navigator.clipboard.writeText(profile.partnerCode)

    const closeCurrentSession = async () => {
        if (!window.confirm('¿Cerrar la sesión actual?')) return
        const refreshToken = authSession?.refreshToken
        if (refreshToken) await revokeCurrent({refreshToken})
        dispatch(sessionEnded())
        navigate('/login', {replace: true})
    }

    const closeAllSessions = async () => {
        if (!window.confirm('¿Cerrar todas tus sesiones activas? Tendrás que iniciar sesión otra vez.')) return
        try {
            await revokeAll().unwrap()
            dispatch(sessionEnded())
            navigate('/login', {replace: true})
        } catch {
            // The mutation state renders the API problem.
        }
    }

    const unlinkPartner = async () => {
        if (!window.confirm('¿Desenlazar a tu pareja? Conservarán su historial, pero ya no podrán crear planes ni jugar juntos.')) return
        try {
            await endRelationship().unwrap()
            toast.success('La pareja quedó desenlazada.')
        } catch {
            // El middleware global muestra el detalle enviado por la API.
        }
    }

    return (
        <div className="flex flex-col gap-6">
            <div>
                <p className="text-sm text-muted-foreground">
                    Tu espacio y sus acuerdos
                </p>
                <h1 className="mt-1 font-serif text-3xl font-semibold text-ink">
                    Perfil
                </h1>
            </div>

            <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
                <div className="flex items-center gap-3">
                    <span
                        className="flex size-11 items-center justify-center rounded-full bg-lilac font-serif text-lg font-semibold text-plum">
                        {profile.firstName[0]}
                    </span>
                    <div>
                        <h2 className="font-serif text-xl font-semibold text-ink">Tus datos
                        </h2>
                        <p className="text-xs text-muted-foreground">{profile.email}</p>
                    </div>
                </div>

                {changeState.error && <Notice className="mt-4">{getApiErrorMessage(changeState.error)}</Notice>}

                {saved && <Notice tone="success" className="mt-4"><Check className="size-4"/>Tu nombre quedó
                    actualizado.</Notice>}

                <form onSubmit={saveName} className="mt-5 grid gap-4 sm:grid-cols-2" noValidate>
                    <FieldLabel>
                        Nombre{''}
                        <input {...register('firstName')} className={inputClassName}
                               aria-invalid={Boolean(errors.firstName)}/>
                        <FieldError>
                            {errors.firstName?.message}
                        </FieldError>
                    </FieldLabel>
                    <FieldLabel>
                        Apellido{''}
                        <input {...register('lastName')} className={inputClassName}
                               aria-invalid={Boolean(errors.lastName)}/>
                        <FieldError>
                            {errors.lastName?.message}
                        </FieldError>
                    </FieldLabel>
                    <Button
                        type="submit"
                        disabled={changeState.isLoading}
                        className="sm:col-span-2"
                    >
                        {changeState.isLoading ? 'Guardando…' : 'Guardar nombre'}
                    </Button>
                </form>
            </section>

            <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
                <div className="flex items-start gap-3">
                    <HeartHandshake className="mt-0.5 text-pink"/>
                    <div className="min-w-0 flex-1">
                        <h2 className="font-serif text-xl font-semibold text-ink">
                            {couple?.isActive ? 'Tu pareja' : 'Conecta con tu pareja'}
                        </h2>
                        <p className="mt-1 text-sm text-muted-foreground">
                            {couple?.isActive ?
                                `Entrelazados desde ${formatDate(couple.beginRelationshipAt, {dateStyle: 'medium'})}`
                                :
                                'Comparte tu código o escribe el de la otra persona.'
                            }
                        </p>
                    </div>
                </div>
                
                {couple?.isActive ?
                    partnerLoading ?
                        <LoadingState label="Cargando información de la pareja…" />
                        :
                        partner ?
                            <div className="mt-5 space-y-4">
                                <div className="flex items-center gap-3 rounded-2xl bg-lilac p-4">
                                    <span className="flex size-10 shrink-0 items-center justify-center rounded-full bg-white font-serif text-lg font-semibold text-plum">
                                        {partner.partnerName[0]}
                                    </span>
                                    <div className="min-w-0">
                                        <p className="font-semibold text-ink">{partner.partnerName}</p>
                                        <p className="text-xs text-muted-foreground">Tu persona favorita</p>
                                    </div>
                                </div>
                                <PartnerPreferences preferences={partner.partnerPreferences} />
                                <Button
                                    variant="destructive"
                                    onClick={unlinkPartner}
                                    disabled={endRelationshipState.isLoading}
                                    className="w-full"
                                >
                                    <HeartHandshake />
                                    {endRelationshipState.isLoading ? 'Desenlazando…' : 'Desenlazar'}
                                </Button>
                            </div>
                            : <Notice className="mt-4">{getApiErrorMessage(partnerError)}</Notice>
                    :
                    <>
                        <div className="mt-5 flex items-center gap-2 rounded-2xl bg-lilac p-3">
                            <code className="min-w-0 flex-1 truncate px-2 font-mono font-bold tracking-wider text-plum">
                                {profile.partnerCode}
                            </code>
                            <Button variant="outline" size="icon" onClick={copyCode} aria-label="Copiar código">
                                <Copy/>
                            </Button>
                        </div>
                        <Button onClick={() => navigate('/conectar')} className="mt-4 w-full">
                            Conectar nuestros espacios
                        </Button>
                    </>
                }
            </section>

            <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
                <div><h2 className="font-serif text-xl font-semibold text-ink">Mis preferencias</h2><p
                    className="mt-1 text-sm text-muted-foreground">Ayudan a crear planes más cercanos a lo que
                    disfrutan.</p></div>
                {preferenceError && <Notice className="mt-4">{preferenceError}</Notice>}
                <div className="mt-5 space-y-6">
                    <PreferenceSection title="Me encanta" values={profile.datingPreferences.favoriteActivities}
                                       placeholder="Ej. Museos" tone="success"
                                       onAdd={(value) => runPreference(() => addFavorite({activity: value}).unwrap())}
                                       onRemove={(value) => runPreference(() => removeFavorite({activity: value}).unwrap())}/>
                    <PreferenceSection title="Prefiero evitar" values={profile.datingPreferences.avoidedActivities}
                                       placeholder="Ej. Karaoke" tone="danger"
                                       onAdd={(value) => runPreference(() => addAvoided({activity: value}).unwrap())}
                                       onRemove={(value) => runPreference(() => removeAvoided({activity: value}).unwrap())}/>
                    <PreferenceSection title="Comida que disfruto" values={profile.datingPreferences.foodPreferences}
                                       placeholder="Ej. Sushi" tone="purple"
                                       onAdd={(value) => runPreference(() => addFood({food: value}).unwrap())}
                                       onRemove={(value) => runPreference(() => removeFood({food: value}).unwrap())}/>
                </div>
            </section>

            <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
                <div className="flex items-start gap-3"><ShieldCheck className="mt-0.5 text-success"/>
                    <h2 className="font-serif text-xl font-semibold text-ink">Seguridad y sesiones</h2>
                </div>
                {sessionsLoading ?
                    <LoadingState label="Consultando sesiones…"/>
                    :
                    <div>
                        {sessions.map((session) =>
                            <div key={session.id} className="flex items-center gap-3 rounded-2xl bg-background p-3">
                                <KeyRound className="size-4 text-plum"/>
                                <div className="min-w-0 flex-1">
                                    <p className="text-sm font-semibold text-ink">
                                        Sesión iniciada {formatDate(session.createdAtUtc, {
                                        dateStyle: 'medium',
                                        timeStyle: 'short'
                                    })}
                                    </p>
                                    <p className="text-xs text-muted-foreground">
                                        Expira {formatDate(session.expiresAtUtc, {
                                        dateStyle: 'medium',
                                        timeStyle: 'short'
                                    })}
                                    </p>
                                </div>
                            </div>)
                        }
                    </div>
                }
                {revokeAllState.error && <Notice className="mt-4">{getApiErrorMessage(revokeAllState.error)}</Notice>}
                <div className="mt-5 grid gap-3 sm:grid-cols-2"><Button variant="outline" onClick={closeCurrentSession}><LogOut/>Cerrar
                    esta sesión</Button><Button variant="destructive" onClick={closeAllSessions}
                                                disabled={revokeAllState.isLoading}><Trash2/>Cerrar todas</Button></div>
            </section>
        </div>
    )
}

interface PreferenceSectionProps {
    title: string
    values: string[]
    placeholder: string
    tone: 'success' | 'danger' | 'purple'
    onAdd: (value: string) => Promise<void>
    onRemove: (value: string) => Promise<void>
}

interface PartnerPreferencesProps {
    preferences: {
        favoriteActivities: string[]
        avoidedActivities: string[]
        foodPreferences: string[]
    }
}

function PartnerPreferences({preferences}: PartnerPreferencesProps) {
    const summaries = [
        {label: 'Le encanta', values: preferences.favoriteActivities},
        {label: 'Prefiere evitar', values: preferences.avoidedActivities},
        {label: 'Disfruta comer', values: preferences.foodPreferences},
    ]

    return (
        <div className="space-y-3">
            {summaries.map(({label, values}) => (
                <div key={label}>
                    <p className="text-xs font-bold uppercase tracking-wider text-muted-foreground">{label}</p>
                    <p className="mt-1 text-sm text-ink">{values.join(' · ') || 'Aún no ha agregado preferencias.'}</p>
                </div>
            ))}
        </div>
    )
}

function PreferenceSection({
                               title,
                               values,
                               placeholder,
                               tone,
                               onAdd,
                               onRemove,
                           }: PreferenceSectionProps) {
    const [value, setValue] = useState('')
    const [saving, setSaving] = useState(false)
    const tones = {
        success: 'bg-success-soft text-success',
        danger: 'bg-danger/10 text-danger',
        purple: 'bg-lilac text-plum'
    }

    const add = async () => {
        const normalized = value.trim()
        if (!normalized || normalized.length > 200) return
        setSaving(true)
        try {
            await onAdd(normalized)
            setValue('')
        } catch {
            // The parent renders the API problem.
        } finally {
            setSaving(false)
        }
    }

    return (
        <div><p className="text-xs font-bold uppercase tracking-wider text-muted-foreground">{title}</p>
            <div className="mt-2 flex flex-wrap gap-2">{values.length === 0 &&
                <span className="text-sm text-muted-foreground">Aún no agregas ninguna.</span>}{values.map((item) =>
                <span key={item}
                      className={`inline-flex min-h-9 items-center gap-1 rounded-full px-3 text-sm font-semibold ${tones[tone]}`}>{item}
                    <button onClick={() => {
                        void onRemove(item).catch(() => undefined)
                    }} aria-label={`Quitar ${item}`} className="ml-1 rounded-full p-1 hover:bg-white/60"><X
                        className="size-3"/></button></span>)}</div>
            <div className="mt-3 flex gap-2"><input value={value} maxLength={200}
                                                    onChange={(event) => setValue(event.target.value)}
                                                    onKeyDown={(event) => {
                                                        if (event.key === 'Enter') {
                                                            event.preventDefault();
                                                            void add()
                                                        }
                                                    }} className={`${inputClassName} mt-0 min-w-0 flex-1`}
                                                    placeholder={placeholder}/><Button size="icon" onClick={add}
                                                                                       disabled={saving || !value.trim()}
                                                                                       aria-label={`Agregar a ${title}`}><Plus/></Button>
            </div>
        </div>
    )
}
