import { z } from 'zod'

export const loginSchema = z.object({
  email: z.email('Escribe un correo válido.').max(320),
  password: z.string().min(1, 'Escribe tu contraseña.').max(128),
})

export const registerSchema = z.object({
  firstName: z.string().trim().min(1, 'Escribe tu nombre.').max(100),
  lastName: z.string().trim().min(1, 'Escribe tu apellido.').max(100),
  email: z.email('Escribe un correo válido.').max(320),
  password: z.string().min(8, 'Usa al menos 8 caracteres.').max(128),
  confirmPassword: z.string(),
}).refine((values) => values.password === values.confirmPassword, {
  path: ['confirmPassword'],
  message: 'Las contraseñas no coinciden.',
})

export type LoginValues = z.infer<typeof loginSchema>
export type RegisterValues = z.infer<typeof registerSchema>
