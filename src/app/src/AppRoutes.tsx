import { Navigate, createBrowserRouter } from 'react-router'
import { PublicOnly, RequireAuth } from '@/app/route-guards'
import { TurnGameHubProvider } from '@/app/turn-game-hub'
import Layout from '@/Layout'
import { LoginPage } from '@/features/auth/login-page'
import { RegisterPage } from '@/features/auth/register-page'

export const router = createBrowserRouter([
  { path: '/login', element: <PublicOnly><LoginPage /></PublicOnly> },
  { path: '/registro', element: <PublicOnly><RegisterPage /></PublicOnly> },
  {
    element: <RequireAuth />,
    children: [{
      element: <TurnGameHubProvider><Layout /></TurnGameHubProvider>,
      children: [
        { index: true, lazy: () => import('@/features/home/home-page').then((module) => ({ Component: module.HomePage })) },
        { path: 'conectar', lazy: () => import('@/features/couple/connect-partner-page').then((module) => ({ Component: module.ConnectPartnerPage })) },
        { path: 'citas', lazy: () => import('@/features/dates/dates-page').then((module) => ({ Component: module.DatesPage })) },
        { path: 'citas/:datePlanId', lazy: () => import('@/features/dates/date-plan-page').then((module) => ({ Component: module.DatePlanPage })) },
        { path: 'inspiracion', lazy: () => import('@/features/inspiration/inspiration-page').then((module) => ({ Component: module.InspirationPage })) },
        { path: 'juego', lazy: () => import('@/features/games/game-lobby-page').then((module) => ({ Component: module.GameLobbyPage })) },
        { path: 'juego/:turnGameId', lazy: () => import('@/features/games/turn-game-page').then((module) => ({ Component: module.TurnGamePage })) },
        { path: 'perfil', lazy: () => import('@/features/profile/profile-page').then((module) => ({ Component: module.ProfilePage })) },
      ],
    }],
  },
  { path: '*', element: <Navigate to="/" replace /> },
])
