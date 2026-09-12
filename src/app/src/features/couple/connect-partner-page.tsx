import { zodResolver } from '@hookform/resolvers/zod'
import { Check, Copy, HeartHandshake, Link2, QrCode, Share2 } from 'lucide-react'
import { QRCodeSVG } from 'qrcode.react'
import { useForm } from 'react-hook-form'
import { useNavigate, useSearchParams } from 'react-router'
import { toast } from 'sonner'
import { z } from 'zod'
import { useBeginRelationshipMutation, useGetCurrentCoupleQuery, useGetProfileQuery } from '@/app/api'
import { getApiErrorMessage } from '@/app/errors'
import { FieldError, FieldLabel, inputClassName } from '@/components/form-field'
import { LoadingState, Notice } from '@/components/page-state'
import { Button } from '@/components/ui/button'

const schema = z.object({
  partnerCode: z.string().trim().length(12, 'El código debe tener 12 caracteres.'),
})

type Values = z.infer<typeof schema>

export function ConnectPartnerPage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const codeFromQr = searchParams.get('codigo')?.trim() ?? ''
  const { data: profile, isLoading: profileLoading } = useGetProfileQuery()
  const { data: couple, isLoading: coupleLoading } = useGetCurrentCoupleQuery()
  const [beginRelationship, mutation] = useBeginRelationshipMutation()
  const { register, handleSubmit, formState: { errors }, setError } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { partnerCode: codeFromQr },
  })

  if (profileLoading || coupleLoading) return <LoadingState />

  if (couple?.isActive) {
    return (
      <Notice tone="success">
        <p className="font-semibold">Ya están entrelazados.</p>
        <button onClick={() => navigate('/')} className="mt-2 font-bold underline">Volver a Inicio</button>
      </Notice>
    )
  }

  const copyCode = async () => {
    if (profile?.partnerCode) {
      await navigator.clipboard.writeText(profile.partnerCode)
      toast.success('Código copiado.')
    }
  }

  const shareCode = async () => {
    if (!profile?.partnerCode) return
    const url = new URL('/conectar', window.location.origin)
    url.searchParams.set('codigo', profile.partnerCode)
    const text = `Conéctate conmigo en Entrelazados con este código: ${profile.partnerCode}`
    if (navigator.share) await navigator.share({ title: 'Entrelazados', text, url: url.toString() })
    else {
      await navigator.clipboard.writeText(url.toString())
      toast.success('Enlace copiado.')
    }
  }

  const qrUrl = profile?.partnerCode
    ? `${window.location.origin}/conectar?codigo=${encodeURIComponent(profile.partnerCode)}`
    : ''

  const onSubmit = handleSubmit(async ({ partnerCode }) => {
    if (partnerCode === profile?.partnerCode) {
      setError('partnerCode', { message: 'Ese es tu propio código. Escribe el de tu pareja.' })
      return
    }
    try {
      await beginRelationship({ partnerCode }).unwrap()
      navigate('/', { replace: true, state: { linked: true } })
    } catch {
      // The mutation state renders the API problem.
    }
  })

  return (
    <div className="flex flex-col gap-6">
      <div>
        <p className="text-sm text-muted-foreground">Un espacio que empieza con dos</p>
        <h1 className="mt-1 font-serif text-3xl font-semibold text-ink">Conecta con tu pareja</h1>
      </div>

      <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
        <div className="flex items-start gap-3">
          <div className="flex size-11 shrink-0 items-center justify-center rounded-2xl bg-pink-soft text-plum"><HeartHandshake /></div>
          <div><h2 className="font-serif text-xl font-semibold text-ink">Comparte tu código</h2><p className="mt-1 text-sm text-muted-foreground">Tu pareja puede usarlo una sola vez para crear su espacio compartido.</p></div>
        </div>
        <div className="mt-5 rounded-2xl bg-lilac p-4 text-center">
          <code className="font-mono text-xl font-bold tracking-[0.15em] text-plum">{profile?.partnerCode}</code>
        </div>
        {qrUrl && (
          <div className="mt-4 flex flex-col items-center rounded-2xl border border-lavender bg-white p-5 text-center">
            <div className="rounded-xl bg-white p-2 shadow-sm">
              <QRCodeSVG value={qrUrl} size={184} level="M" marginSize={1} />
            </div>
            <p className="mt-3 flex items-center gap-2 text-sm font-semibold text-ink"><QrCode className="size-4 text-plum" />Escanea para entrelazarse</p>
            <p className="mt-1 text-xs leading-5 text-muted-foreground">Tu pareja puede escanearlo con la cámara de su teléfono. El código se completará automáticamente.</p>
          </div>
        )}
        <div className="mt-3 grid grid-cols-2 gap-3">
          <Button variant="outline" onClick={copyCode}><Copy />Copiar</Button>
          <Button variant="outline" onClick={shareCode}><Share2 />Compartir</Button>
        </div>
      </section>

      <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
        <div className="flex items-center gap-3"><Link2 className="text-plum" /><h2 className="font-serif text-xl font-semibold text-ink">Ingresa su código</h2></div>
        {codeFromQr && <Notice tone="info" className="mt-4">Código leído desde el QR. Confirma para enlazar sus espacios.</Notice>}
        {mutation.error && <Notice className="mt-4">{getApiErrorMessage(mutation.error)}</Notice>}
        <form onSubmit={onSubmit} className="mt-5" noValidate>
          <FieldLabel>
            Código de pareja
            <input {...register('partnerCode')} maxLength={12} autoCapitalize="none" autoCorrect="off" className={`${inputClassName} font-mono tracking-wider`} placeholder="12 caracteres" aria-invalid={Boolean(errors.partnerCode)} />
            <FieldError>{errors.partnerCode?.message}</FieldError>
          </FieldLabel>
          <Button type="submit" disabled={mutation.isLoading} className="mt-5 w-full">
            {mutation.isLoading ? 'Conectando…' : <><Check />Entrelazarnos</>}
          </Button>
        </form>
      </section>
    </div>
  )
}
