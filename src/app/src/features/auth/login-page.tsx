import { zodResolver } from '@hookform/resolvers/zod'
import { LogIn } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { Link, useLocation, useNavigate } from 'react-router'
import { useLoginMutation } from '@/app/api'
import { credentialsReceived } from '@/app/auth-slice'
import { getApiErrorMessage } from '@/app/errors'
import { useAppDispatch } from '@/app/hooks'
import { Notice } from '@/components/page-state'
import { FieldError, FieldLabel, inputClassName } from '@/components/form-field'
import { Button } from '@/components/ui/button'
import { AuthShell } from './auth-shell'
import { loginSchema, type LoginValues } from './auth-schemas'
import { PasswordField } from './password-field'

interface LoginLocationState {
  from?: string
  message?: string
}

export function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const dispatch = useAppDispatch()
  const [login, loginState] = useLoginMutation()
  const locationState = location.state as LoginLocationState | null
  const { register, handleSubmit, formState: { errors } } = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '' },
  })

  const onSubmit = handleSubmit(async (values) => {
    try {
      const session = await login(values).unwrap()
      dispatch(credentialsReceived(session))
      navigate(locationState?.from ?? '/', { replace: true })
    } catch {
      // El middleware global muestra el detalle enviado por la API.
    }
  })

  return (
    <AuthShell>
      <div className="mt-8 rounded-3xl border border-lavender bg-white p-6 shadow-card sm:p-8">
        <div className="text-center">
          <h1 className="mt-4 font-serif text-3xl font-semibold text-ink">Qué gusto verte</h1>
          <p className="mt-2 text-sm leading-6 text-muted-foreground">Continúen creando momentos especiales.</p>
        </div>
        
        {locationState?.message && <Notice tone="success" className="mt-5">{locationState.message}</Notice>}
        
        {loginState.error && (
          <Notice className="mt-5">
            {getApiErrorMessage(loginState.error, 'El correo o la contraseña no son válidos.')}
          </Notice>
        )}
        
        <form onSubmit={onSubmit} className="mt-7 flex flex-col gap-4" noValidate>
          <FieldLabel>
            Correo electrónico{''}
            <input
              {...register('email')}
              type="email"
              autoComplete="email"
              placeholder="tu@correo.com"
              className={inputClassName}
              aria-invalid={Boolean(errors.email)}
            />
            <FieldError>{errors.email?.message}</FieldError>
          </FieldLabel>
          <PasswordField
            label="Contraseña"
            error={errors.password?.message}
            registration={register('password')}
            autoComplete="current-password"
          />
          <Button type="submit" disabled={loginState.isLoading} className="mt-2 w-full">
            <LogIn className="size-4" />
            {loginState.isLoading ? 'Entrando…' : 'Iniciar sesión'}
          </Button>
        </form>
        <div className="my-6 flex items-center gap-3 text-xs text-muted-foreground">
          <span className="h-px flex-1 bg-lavender" />o<span className="h-px flex-1 bg-lavender" />
        </div>
        <Link to="/registro" className="block min-h-11 text-center text-sm font-semibold text-plum">
          ¿Aún no tienes cuenta? Regístrate
        </Link>
      </div>
    </AuthShell>
  )
}
