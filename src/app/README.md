# Entrelazados · frontend

Aplicación React + TypeScript + Vite conectada a la API de DatePlanner.

## Desarrollo local

Desde `src/app`:

```bash
npm ci
npm run dev
```

No hace falta un archivo `.env`. Con la API de Docker levantada, Vite sirve la
aplicación en `http://localhost:5173` y redirige `/api` y `/hubs` a
`http://localhost:8081`. Para usar otro backend, pasa la variable al proceso:

```bash
VITE_API_PROXY_TARGET=http://localhost:8081 npm run dev
```

También puedes iniciar API, PostgreSQL y frontend juntos desde la raíz:

```bash
docker compose up --build -d --wait
```

Dentro de Docker, Vite usa `http://dateplanner.api:8080`. El código está montado
desde el host y se comprueban los cambios cada 100 ms para activar la recarga.
La API usa una imagen compilada y se reconstruye manualmente.

## Producción

Desde la raíz, copia `.env.example` a `.env` y configura sus valores. Después:

```bash
docker compose --env-file .env -f compose.production.yaml up --build -d --wait
```

Nginx sirve el frontend compilado y redirige `/api` y `/hubs` a `http://api:8080`.
El navegador siempre usa rutas relativas al mismo dominio.

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
