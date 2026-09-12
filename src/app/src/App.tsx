import { RouterProvider } from 'react-router'
import { router } from './AppRoutes'

export default function App() {
  return <RouterProvider router={router} />
}
