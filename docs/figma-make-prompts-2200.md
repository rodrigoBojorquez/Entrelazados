# Entrelazados para Figma Make - serie de prompts de máximo 2,200 caracteres

Usa todos los prompts **en orden y dentro del mismo archivo/proyecto de Figma Make**. Espera a que termine cada generación antes de pegar el siguiente. Adjunta el PNG del logo con el Prompt 1.

## Prompt 1 - Base del producto y estilo

```text
Crea una app web responsive mobile-first llamada Entrelazados, en español, para parejas que ya están juntas. No es una app para conocer personas. Sirve para decidir quién planea la próxima cita, obtener inspiración, crear un itinerario compartido, consultar la próxima cita y recordar citas anteriores. Construye un prototipo funcional con React y datos mock; no una landing ni pantallas aisladas. Conserva este contexto para mis siguientes prompts y amplía siempre el mismo proyecto.

Usa el símbolo PNG adjunto como logo oficial sin redibujarlo, recolorearlo ni deformarlo. Su concepto son dos hilos/corazones entrelazados. La experiencia debe sentirse bonita, tierna, minimalista, cálida, inclusiva y privada; emocional en los detalles, pero nunca infantil o empalagosa. Inspiración conceptual: la claridad, ligereza y tarjetas explorables de Lovewick, sin copiar su marca, contenido ni pantallas.

Paleta: texto #0A0316, fondo #FFFAFC, primario #4D2A92, secundario #F561AD y acento #F2DA30. Deriva fondos pastel accesibles. Usa Inter, mucho aire, tarjetas de 20-24 px, controles de 12-16 px, iconos lineales redondeados, bordes lavanda y sombras sutiles. Morado para CTA, rosa para detalles afectivos y amarillo solo para acentos/celebración. Evita glassmorphism, degradados fuertes y corazones decorativos repetidos.

En móvil crea navegación inferior: Inicio, Citas, Inspiración, Juego y Perfil; en desktop, sidebar. Usa una capa de datos mock separada de la futura API. Estado demo inicial: Sofi Martínez y Dani Rivera, pareja activa desde 14 feb 2025. Añade un selector discreto de estados demo. No agregues chat, fotos, pagos, reservas, ubicación en vivo, notificaciones, cambio de email/contraseña ni desvinculación.
```

## Prompt 2 - Acceso y conexión de pareja

```text
Continúa el mismo proyecto. Implementa bienvenida, registro, login y conexión de pareja, respetando el estilo existente.

Bienvenida: logo protagonista, frase “Planeen menos. Vivan más juntos.”, explicación breve y botones Crear cuenta/Iniciar sesión.

Registro: nombre y apellido obligatorios (máx. 100 cada uno), correo válido y único (máx. 320), contraseña de 8-128 caracteres y confirmación solo del frontend. Incluye mostrar/ocultar contraseña, labels persistentes, errores inline, loading y correo ya registrado. Al registrarse, llevar a login; no asumir inicio automático.

Login: correo, contraseña, mostrar/ocultar y error general “El correo o la contraseña no son válidos.”. Simula sesión expirada y renovación silenciosa. No añadas “Olvidé mi contraseña”.

Si no hay pareja, Inicio/Citas/Inspiración/Juego muestran un bloqueo amable y CTA Conectar con mi pareja; Perfil sigue disponible. Crea dos opciones: compartir el código personal de exactamente 12 caracteres mediante copiar/compartir, e ingresar el código de la pareja con pegado rápido. Mock: ENLACE-9X2Q7. Incluye confirmación, código inválido/no encontrado, código propio, alguien ya emparejado y éxito. Una persona solo puede tener una pareja activa y no puede vincularse consigo misma.

La respuesta real de pareja solo contiene IDs, fecha de inicio y estado, no el perfil ajeno: jamás muestres GUIDs; usa “Tu pareja” si no hay nombre. En mock sí muestra Dani. En éxito anima suavemente los dos hilos uniéndose, muestra “Ya están entrelazados” y dirige a Inicio.
```

## Prompt 3 - Inicio y estados principales

```text
Continúa el mismo proyecto. Construye un Inicio cálido, útil y no saturado, con variantes controlables desde el selector demo.

1) Sin pareja: mensaje breve y CTA Conectar con mi pareja. 2) Pareja conectada sin plan: saludo “Buenos días, Sofi”, bloque “Sofi y Dani”, CTA Planear nuestra próxima cita y accesos Que el juego decida/Necesito inspiración. 3) Plan en progreso: tarjeta “Su próxima cita está tomando forma”, responsable, cantidad de puntos y Continuar planeando. 4) Cita programada: debe dominar la pantalla; muestra fecha local, cuenta regresiva, quién la planeó, primer punto, cantidad de paradas y mini mapa solo si hay ubicaciones; acciones Ver plan y una opción visual de calendario sin prometer sincronización externa. 5) Después de la cita: acción discreta Marcar como realizada, confirmación, microcelebración y envío al historial.

Añade acceso inferior a citas anteriores y una tarjeta pequeña para completar preferencias. Si no hay datos usa estados vacíos afectuosos, nunca culpabilizantes. No muestres IDs, enums en inglés, coordenadas crudas ni códigos internos.

Datos demo de próxima cita: Dani es responsable; “Ver el atardecer en Playa Delfines” 18:00 con ubicación, “Cena frente al mar” 20:00 con ubicación y “Helado y caminata” sin ubicación. Usa una fecha futura cercana y moneda MXN cuando aplique. En escritorio conserva una columna cómoda, no un dashboard corporativo.
```

## Prompt 4 - Juego en tiempo real

```text
Continúa el mismo proyecto. Implementa el flujo Juego para decidir quién planea. Requiere pareja activa. Pantalla inicial con dos tarjetas: Piedra, papel o tijera y El número más alto gana. Explica: “La primera persona en ganar 2 rondas planifica la próxima cita”. Añade lobby y conexión en tiempo real simulada.

Estados de partida: Lista, En progreso y Finalizada. Cada ronda: En progreso o Finalizada. Cada persona juega una sola vez por ronda. Piedra/papel/tijera admite rock, paper, scissors; la UI los muestra en español con 3 botones grandes. Número más alto admite enteros 1-6 con 6 opciones. Muestra número de ronda y marcador de victorias de Sofi y Dani.

Al elegir, bloquea el control propio y muestra “Elección enviada”. La jugada propia puede verse; la rival queda oculta hasta que ambas existan. Después revela simultáneamente y muestra ganador o empate. Un empate no suma victoria y habilita otra ronda. El juego termina al alcanzar 2 victorias, no necesariamente tras 3 rondas.

Incluye “Esperando a Dani…”, “Dani ya eligió”, offline/reconexión, movimiento duplicado, ronda terminada y partida finalizada. Al terminar, celebra con moderación, muestra “Dani planifica la próxima cita”, crea automáticamente un plan Planeando cuyo responsable es el ganador y ofrece Empezar a planear/Ir a Inicio. Nunca reveles la jugada rival durante una ronda activa.
```

## Prompt 5 - Crear y planear una cita

```text
Continúa el mismo proyecto. En Citas usa tabs Próxima, Planeando e Historial. Permite crear un plan manual: quien lo inicia queda como responsable. Después abre el constructor compartido; cualquiera de las dos personas puede modificarlo mientras esté Planeando.

Modelo del plan: responsable, fecha opcional, estado e itinerario ordenado. Estados: Planeando, Programada, Cancelada y Realizada. En Planeando se agregan/quitan puntos; Programada es solo lectura; Cancelada y Realizada son finales. Una cita Planeando o Programada puede cancelarse. Solo Programada puede marcarse Realizada.

Constructor: timeline vertical numerado, empty state “Empiecen con algo que les emocione”, botón Agregar punto, resumen de paradas y CTA fijo Programar cita. Formulario en bottom sheet móvil/panel desktop: nombre obligatorio máx. 200, hora opcional y ubicación opcional. La ubicación debe terminar en latitud+longitud juntas; permite búsqueda/mapa amigable, pero no inventes un campo de dirección persistido. Un punto puede carecer de hora o ubicación. Al agregar, va al final con orden consecutivo. Permite quitar con confirmación ligera.

Programar abre selección de fecha y repaso. Requiere fecha y al menos un punto; no existe hora global, las horas pertenecen a cada punto. Tras confirmar cambia a Programada y bloquea edición. Incluye loading, error, éxito, plan no encontrado/sin acceso, intento de editar fuera de Planeando e intento de programar itinerario vacío.
```

## Prompt 6 - Próxima cita e historial

```text
Continúa el mismo proyecto. Completa las vistas de consulta de citas.

Detalle Programada: fecha local, cuenta regresiva, responsable, timeline en orden y horas opcionales. Incluye mapa con uno o varios pines solo para puntos con ubicación; los puntos sin ubicación permanecen visibles con tratamiento elegante. Acciones Marcar como realizada y Cancelar cita como secundaria destructiva, ambas con confirmación. No permitas editar itinerario.

Cancelada: solo lectura, conserva itinerario y fecha si existían y no muestra acciones inválidas. Realizada: solo lectura y aparece en Historial. Planeando puede cancelarse aunque aún no tenga fecha.

Historial es alcance de producto, pero su endpoint de listado aún no existe: implementa el prototipo con mocks y separa su servicio para integración futura. Muestra realizadas y canceladas, filtro por mes y paginación/carga incremental. Cada tarjeta enseña fecha si existe, estado, responsable, primer punto y cantidad de paradas. Al abrir, reutiliza timeline y mapa en modo lectura. Incluye empty state.

También la obtención automática de “próxima cita” requiere una futura API de listado; no simules que el backend actual ya la ofrece. Para el mock incluye al menos dos realizadas y una cancelada. Traduce estados al español y no enseñes GUIDs, UTC o coordenadas crudas. Mantén navegación y diseño establecidos.
```

## Prompt 7 - Inspiración personalizada

```text
Continúa el mismo proyecto. Implementa Inspiración con mocks. Es alcance del producto y el modelo existe, pero generación/aceptación aún no tienen API; aísla este servicio para integración futura.

Formulario “Ayúdennos a imaginar una cita”: presupuesto preferido no negativo, ciudad obligatoria máx. 150, hora de inicio opcional, hora de fin opcional y resumen de preferencias de la pareja. CTA “Crear una idea para nosotros”. Muestra favoritas, evitadas y comida positiva sin permitir que una evitada se use como recomendación. Añade loading con mensajes cálidos, sin mencionar detalles técnicos de IA.

Resultado: presupuesto preferido, estimado opcional, ciudad, rango horario opcional e itinerario ordenado. Cada punto contiene nombre, etiquetas, hora opcional y ubicación opcional. Muestra mapa solo cuando haya ubicaciones. Acciones Convertir en nuestro plan, Intentar otra idea y Ajustar datos.

Mock: $1,500 MXN preferido, $1,280 estimado, Cancún, 17:30-22:00 y 3 puntos con tags “al aire libre”, “tranquilo” y “vegetariano”. Al aceptar, crea un plan Planeando vinculado a la idea y abre el constructor para editar antes de programar.

No inventes ratings, comentarios comunitarios, reservas, disponibilidad, fotos obligatorias ni negocios patrocinados. Incluye errores de generación, resultado sin presupuesto estimado, puntos sin hora/ubicación y opción de reintentar.
```

## Prompt 8 - Perfil, preferencias y seguridad

```text
Continúa el mismo proyecto. Completa Perfil con tres áreas.

Perfil personal: nombre/apellido editables (máx. 100), correo visible de solo lectura, código de pareja con copiar/compartir y, si existe pareja, nombre mock o “Tu pareja”, fecha de conexión y estado activo. No añadas edición de correo, contraseña, foto ni desvinculación.

Preferencias: chips en “Nos encanta”, “Preferimos evitar” y “Comida que disfruto”. Mock: Cine, Caminar al atardecer, Museos; evitar Karaoke, Deportes extremos; comida Sushi, Italiana, Opciones vegetarianas. Permite agregar/quitar, máximo 200 caracteres, sin duplicados. Una actividad no puede ser favorita y evitada simultáneamente. Si hay conflicto, explícalo y ofrece quitarla primero sin cambiar datos automáticamente. Usa estados vacíos y feedback inmediato.

Seguridad: sesiones reales solo tienen id, familia, fecha de creación y expiración. Muestra filas neutrales como “Sesión iniciada el 9 sep, 14:30”; no inventes dispositivo, navegador, ciudad o IP. El cliente puede revocar la sesión actual usando su refresh token y cerrar todas. La API no revoca una fila ajena por id: esas filas son informativas, sin botón individual. Incluye Cerrar sesión y Cerrar todas las sesiones, ambas con confirmación, más estado vacío tras cerrarlas. Mantén los tokens y datos técnicos fuera de la UI.
```

## Prompt 9 - Sistema visual, estados y accesibilidad

```text
Continúa el mismo proyecto y haz una pasada de consistencia. Crea componentes reutilizables y variantes para: logo lockup, app bars, navegación, botones primary/secondary/tonal/ghost/destructive, inputs, selector de fecha/hora/lugar, chips, tarjetas de cita/idea, timeline, badges de estado, marcador y movimientos del juego, mapa/pines, bottom sheet, modal, panel lateral, toast, banner, skeleton, empty/error state, session row y countdown.

Usa grid de 8 px, targets táctiles mínimos 44x44, contraste WCAG AA y foco visible. No dependas solo del color: acompaña estados con texto/icono. Añade hover, pressed, disabled y loading. Respeta prefers-reduced-motion. Fechas y horas deben mostrarse en formato local, nunca UTC.

Revisa móvil 390x844, tablet y desktop. En desktop usa sidebar y ancho de contenido máximo cercano a 1120 px; evita tablas administrativas. Los formularios tienen labels persistentes y errores inline. Toast solo para éxito breve; errores accionables permanecen visibles. Confirma cancelar cita, cerrar sesión actual y cerrar todas. Añade skeletons, vacíos, offline/reconexión y errores sin culpar.

Crea una ruta interna UI Kit fuera de la navegación con tokens y variantes. No uses lorem ipsum, GUIDs, enums en inglés, códigos internos ni coordenadas crudas en vistas normales. Mantén la identidad de hilos entrelazados sutil, cálida e inclusiva.
```

## Prompt 10 - Auditoría final del prototipo

```text
Haz la revisión final del mismo proyecto; no rediseñes la identidad. Corrige navegación rota, inconsistencias responsive, componentes duplicados, contraste, overflow y estados imposibles. Verifica que estas historias funcionen con datos mock:

1. Crear cuenta -> login -> compartir/ingresar código -> formar pareja -> Inicio.
2. Elegir juego -> enviar movimientos privados -> resolver empate -> ganar 2 rondas -> crear plan Planeando para el ganador.
3. Crear plan -> agregar puntos con/sin hora y ubicación -> programar -> consultar timeline/mapa -> marcar Realizada.
4. Inspiración -> resultado con presupuesto/itinerario -> convertir en plan -> editar -> programar.
5. Editar preferencias -> detectar conflicto favorita/evitada -> reflejarlas en Inspiración.
6. Filtrar historial por mes -> paginar -> abrir Realizada y Cancelada.
7. Consultar sesiones -> cerrar sesión actual -> cerrar todas.

Comprueba reglas: itinerario editable solo en Planeando; Programar exige fecha y al menos un punto; Realizada solo desde Programada; jugada rival oculta hasta cerrar ronda; empate no suma; partida termina con 2 victorias. Confirma que no aparezcan funciones no soportadas: chat, fotos, pagos, reservas, notificaciones, edición de email/contraseña, desvinculación o revocación arbitraria de otra sesión.

El resultado debe sentirse como un espacio privado que una pareja querría abrir antes de una cita: útil primero, tierno en los detalles y reconocible por sus hilos morado/rosa entrelazados.
```

