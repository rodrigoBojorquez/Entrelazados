import { Eye, EyeOff } from 'lucide-react'
import { useState } from 'react'
import type { UseFormRegisterReturn } from 'react-hook-form'
import { FieldError, FieldLabel, inputClassName } from '@/components/form-field'

interface PasswordFieldProps {
  label: string
  error?: string
  registration: UseFormRegisterReturn
  autoComplete: string
}

export function PasswordField({
  label,
  error,
  registration,
  autoComplete,
}: PasswordFieldProps) {
  const [visible, setVisible] = useState(false)

  return (
    <FieldLabel>
      {label}
      <div className="relative mt-2">
        <input
          {...registration}
          type={visible ? 'text' : 'password'}
          autoComplete={autoComplete}
          className={`${inputClassName} mt-0! pr-24`}
          aria-invalid={Boolean(error)}
        />
        <button
          type="button"
          onClick={() => setVisible((value) => !value)}
          className="absolute inset-y-0 right-3 flex items-center gap-1 text-xs font-bold text-plum"
        >
          {visible ? <EyeOff className="size-3.5" /> : <Eye className="size-3.5" />}
          {visible ? 'Ocultar' : 'Mostrar'}
        </button>
      </div>
      <FieldError>{error}</FieldError>
    </FieldLabel>
  )
}
