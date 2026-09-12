import { Navigate, Outlet, useLocation } from 'react-router'
import { useAppSelector } from './hooks'

export function RequireAuth() {
  const location = useLocation()
  const session = useAppSelector((state) => state.auth.session)
  return session ? <Outlet /> : <Navigate to="/login" replace state={{ from: `${location.pathname}${location.search}` }} />
}

interface PublicOnlyProps {
  children: React.ReactNode
}

export function PublicOnly({ children }: PublicOnlyProps) {
  const session = useAppSelector((state) => state.auth.session)
  return session ? <Navigate to="/" replace /> : children
}
