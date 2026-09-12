# Entrelazados · frontend

Aplicación React + TypeScript + Vite conectada a la API de DatePlanner.

## Desarrollo local

Desde `src/app`:

```bash
npm ci
npm run dev
```

Vite sirve la aplicación en `http://localhost:5173` y redirige `/api` y `/hubs` a `http://localhost:8081`. Para usar otro backend:

```bash
VITE_API_PROXY_TARGET=http://localhost:8081 npm run dev
```

También puedes iniciar API, PostgreSQL y frontend juntos desde la raíz:

```bash
docker compose up --build
```

## Arquitectura

- React Router en Data Mode, con rutas públicas y protegidas.
- Redux Toolkit y RTK Query para sesión, renovación automática del token y estado del servidor.
- React Hook Form + Zod para autenticación, perfil, conexión de pareja e itinerarios.
- SignalR para partidas por turnos y reconexión automática.
- MapCN/MapLibre para seleccionar ubicaciones y visualizar todos los puntos de una cita.
- Tailwind CSS y componentes Shadcn adaptados a la identidad visual de Entrelazados.

## Funcionalidad disponible

- Registro, inicio y cierre de sesión, renovación de token y cierre de todas las sesiones.
- Perfil y preferencias de citas.
- Conexión mediante código de pareja.
- Creación de planes, itinerario colaborativo, ubicación opcional, programación, asistencia y cancelación.
- Piedra, papel o tijera y número mayor mediante SignalR; quien gana crea el siguiente plan.

La API todavía no expone un listado de citas ni endpoints de ideas. La interfaz lo comunica y, para mantener continuidad, recuerda por pareja el último plan abierto en este navegador.
