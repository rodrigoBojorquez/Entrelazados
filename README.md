# Date Planner App

**NO TENGAS MIEDO DE METERLE MANO AL CÓDIGO, TE AMO <3**

### ¿Cómo ejecutar la app?

- Asegúrate de tener Docker Instalado
- Asegúrate de estar en la carpeta raíz del proyecto
- Ejecuta el siguiente comando para crear los contenedores y levantar la app:

```bash
docker-compose up --build -d
```

### ¿Dónde esta el front?
Si quieres visualizar la app, puedes acceder a la dirección `http://localhost:5173` en tu navegador.

Si es para editar el código, es en la carpeta `/src/app` del proyecto.


### ¿Dónde esta el back?
Si quieres ver la interfaz Swagger para testear los endpoint, puedes acceder a la dirección `http://localhost:8080/swagger` en tu navegador.

Si quieres editar el código, es en la carpeta `/src/API` del proyecto.

### Despliegue en producción con Docker Compose

El despliegue de producción construye dos imágenes (`dateplanner-api:prod` y
`dateplanner-app:prod`) y levanta PostgreSQL en una red interna. Nginx sirve el
frontend, envía `/api` y `/hubs` al backend, y conserva la documentación pública
de Swagger en `/swagger`.

1. Crea el archivo local de variables de producción:

```bash
cp .env.production.example .env.production
```

2. Sustituye los secretos del archivo. Puedes generar valores hexadecimales
seguros con:

```bash
openssl rand -hex 32
```

3. Construye y levanta el stack:

```bash
docker compose --env-file .env.production -f compose.production.yaml up --build -d
```

La aplicación queda disponible en `http://localhost:8080` (o el valor de
`APP_PORT`) y Swagger en `http://localhost:8080/swagger`. En un servidor público,
configura TLS en el proxy o balanceador situado delante de este stack.

Para revisar el estado y los logs:

```bash
docker compose --env-file .env.production -f compose.production.yaml ps
docker compose --env-file .env.production -f compose.production.yaml logs -f
```

La configuración completa del despliegue automático a GHCR y al VPS está en
[`docs/production-deployment.md`](docs/production-deployment.md).

### ¿Cómo aportar al proyecto?
Puedes contribuir al proyecto de varias maneras:
1. Dandome besitos
2. Reportando bugs o problemas que encuentres mientras usas la app.
2. Sugerir nuevas funcionalidades o mejoras.
3. Agregar código o documentación al proyecto.
4. Invitandome cafecito mientras trabajamos juntos en el proyecto.
