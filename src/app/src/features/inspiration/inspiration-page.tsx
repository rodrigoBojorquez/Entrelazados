import {Clock3, Lightbulb, MapPin, Sparkles, WalletCards} from 'lucide-react'
import {useNavigate} from 'react-router'
import {useGetCurrentCoupleQuery, useGetProfileQuery} from '@/app/api'
import {EmptyState, LoadingState, Notice} from '@/components/page-state'
import {Button} from '@/components/ui/button'

export function InspirationPage() {
    const navigate = useNavigate()
    const {data: couple, isLoading: coupleLoading} = useGetCurrentCoupleQuery()
    const {data: profile, isLoading: profileLoading} = useGetProfileQuery()

    if (coupleLoading || profileLoading) return <LoadingState/>
    if (!couple?.isActive) return <EmptyState icon={<Lightbulb/>} title="La inspiración se construye entre dos"
                                              description="Conecta con tu pareja para usar sus preferencias al crear ideas."
                                              action={<Button onClick={() => navigate('/conectar')}>Conectar con mi
                                                  pareja</Button>}/>

    return (
        <div className="flex flex-col gap-6">
            <div>
                <p className="text-sm text-muted-foreground">
                    Una chispa para empezar
                </p>
                <h1 className="mt-1 font-serif text-3xl font-semibold text-ink">
                    Inspiración
                </h1>
                <p className="mt-2 text-sm leading-6 text-muted-foreground">
                    Ideas construidas alrededor de lo que disfrutan.
                </p>
            </div>
            <Notice tone="info">
                <strong>
                    Funcionalidad en desarrollo.
                </strong>
                <br/>
                Estamos trabajando constantemente para liberar esta funcionalidad :)
            </Notice>
            <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
                <div className="flex size-12 items-center justify-center rounded-2xl bg-pink-soft text-plum"><Sparkles/>
                </div>
                <h2 className="mt-4 font-serif text-2xl font-semibold text-ink">Ayúdennos a imaginar una cita</h2>
                <div className="mt-5 grid gap-3 sm:grid-cols-2">
                    <PreviewField icon={<WalletCards/>} label="Presupuesto preferido" value="$1,500 MXN"/>
                    <PreviewField icon={<MapPin/>} label="Ciudad" value="Cancún"/>
                    <PreviewField icon={<Clock3/>} label="Hora de inicio" value="17:30"/>
                    <PreviewField icon={<Clock3/>} label="Hora de fin" value="22:00"/>
                </div>
                <div className="mt-5 rounded-2xl bg-lilac p-4"><p
                    className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Tus preferencias
                    disponibles</p><p
                    className="mt-2 text-sm leading-6 text-plum">{profile?.datingPreferences.favoriteActivities.join(' · ') || 'Agrega actividades favoritas desde Perfil.'}</p>
                </div>
                <Button disabled className="mt-5 w-full">Crear una idea para nosotros</Button>
                <p className="mt-2 text-center text-xs text-muted-foreground">Se habilitará en futuras versiones.</p>
            </section>
        </div>
    )
}

interface PreviewFieldProps {
    icon: React.ReactNode
    label: string
    value: string
}

function PreviewField({icon, label, value}: PreviewFieldProps) {
    return <div className="flex items-center gap-3 rounded-2xl border border-lavender bg-background p-3"><span
        className="text-plum">{icon}</span>
        <div><p className="text-xs text-muted-foreground">{label}</p><p
            className="text-sm font-semibold text-ink">{value}</p></div>
    </div>
}
