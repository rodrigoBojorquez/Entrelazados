import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { Link, useNavigate } from 'react-router'
import { useRegisterUserMutation } from '@/app/api'
import { getApiErrorMessage } from '@/app/errors'
import { FieldError, FieldLabel, inputClassName } from '@/components/form-field'
import { Notice } from '@/components/page-state'
import { Button } from '@/components/ui/button'
import { AuthShell } from './auth-shell'
import { registerSchema, type RegisterValues } from './auth-schemas'
import { PasswordField } from './password-field'

export function RegisterPage() {
  const navigate = useNavigate()
  const [registerUser, registerState] = useRegisterUserMutation()
  const { register, handleSubmit, formState: { errors } } = useForm<RegisterValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: { firstName: '', lastName: '', email: '', password: '', confirmPassword: '' },
  })

  const onSubmit = handleSubmit(async (values) => {
    try {
      await registerUser({
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        password: values.password,
      }).unwrap()
      navigate('/login', {
        replace: true,
        state: { message: 'Tu cuenta está lista. Ahora inicia sesión.' },
      })
    } catch {
      // El middleware global muestra el detalle enviado por la API.
    }
  })

  return (
    <AuthShell>
      <div className="mt-8 rounded-3xl border border-lavender bg-white p-6 shadow-card sm:p-8">
        <div className="text-center">
          <h1 className="mt-4 font-serif text-3xl font-semibold text-ink">Crea tu espacio</h1>
          <p className="mt-2 text-sm leading-6 text-muted-foreground">Planea momentos inolvidables y compártelos con tu pareja.</p>
        </div>
        {registerState.error && <Notice className="mt-5">{getApiErrorMessage(registerState.error)}</Notice>}
        <form onSubmit={onSubmit} className="mt-7 flex flex-col gap-4" noValidate>
          <div className="grid gap-4 sm:grid-cols-2">
            <FieldLabel>
              Nombre
              <input {...register('firstName')} autoComplete="given-name" className={inputClassName} aria-invalid={Boolean(errors.firstName)} />
              <FieldError>{errors.firstName?.message}</FieldError>
            </FieldLabel>
            <FieldLabel>
              Apellido
              <input {...register('lastName')} autoComplete="family-name" className={inputClassName} aria-invalid={Boolean(errors.lastName)} />
              <FieldError>{errors.lastName?.message}</FieldError>
            </FieldLabel>
          </div>
          <FieldLabel>
            Correo electrónico
            <input {...register('email')} type="email" autoComplete="email" className={inputClassName} aria-invalid={Boolean(errors.email)} />
            <FieldError>{errors.email?.message}</FieldError>
          </FieldLabel>
          <PasswordField label="Contraseña" error={errors.password?.message} registration={register('password')} autoComplete="new-password" />
          <PasswordField label="Confirma tu contraseña" error={errors.confirmPassword?.message} registration={register('confirmPassword')} autoComplete="new-password" />
          <Button type="submit" disabled={registerState.isLoading} className="mt-2 w-full">
            {registerState.isLoading ? 'Creando cuenta…' : 'Crear mi cuenta'}
            <ArrowRight className="size-4" />
          </Button>
        </form>
        <Link to="/login" className="mt-6 block min-h-11 text-center text-sm font-semibold text-plum">Ya tengo una cuenta</Link>
      </div>
    </AuthShell>
  )
}
