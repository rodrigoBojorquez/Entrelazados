# Prompt maestro para Figma Make - Entrelazados

> Copia desde "INICIO DEL PROMPT" hasta "FIN DEL PROMPT" en Figma Make. Adjunta también el archivo PNG del símbolo de Entrelazados. La paleta ya está especificada dentro del prompt.

---

## INICIO DEL PROMPT

Actúa como product designer senior y frontend prototyper. Diseña y construye en Figma Make una aplicación web responsive, mobile-first, de alta fidelidad y completamente navegable llamada **Entrelazados**. La interfaz debe estar en **español**.

No quiero solamente una landing page ni una colección de pantallas aisladas. Quiero un producto coherente y demostrable, con navegación, formularios, estados, validaciones, modales, feedback visual y datos simulados realistas. Prioriza primero la experiencia móvil y después adapta los layouts a tablet y escritorio.

### 1. Contexto del producto

Entrelazados es una aplicación para parejas que ya están en una relación. No es una app de citas para conocer personas. Su propósito es ayudarles a decidir quién planifica la próxima cita, encontrar inspiración y convertir una idea en un itinerario compartido que después puedan consultar y recordar.

El nombre está inspirado en el crochet: entrelazar hilos y, simbólicamente, entrelazar dos corazones, dos personas y sus experiencias. La promesa emocional es: **menos fricción al planear, más tiempo para conectar**.

La experiencia debe sentirse:

- bonita, tierna, cálida y optimista;
- minimalista y ligera, nunca infantil ni empalagosa;
- inclusiva para cualquier pareja, sin asumir género, orientación, edad o roles tradicionales;
- privada y segura, porque maneja información íntima de la relación;
- juguetona únicamente donde aporta valor, sobre todo en el juego para elegir al planificador;
- clara y tranquila durante tareas prácticas como iniciar sesión, editar preferencias o agendar una cita.

Toma como referencia conceptual a Lovewick por su tono amable, sus tarjetas fáciles de explorar y su capacidad de convertir actividades de pareja en experiencias ligeras. **No copies su identidad visual, su arquitectura, su contenido ni sus pantallas.** Entrelazados debe tener una identidad propia basada en hilos entrelazados, curvas suaves, dos colores que se encuentran y una planificación compartida mucho más enfocada.

### 2. Activos de marca y dirección visual

Usa el símbolo PNG adjunto como logo oficial. Es un corazón formado por dos hilos entrelazados, uno morado y uno rosa, con fondo transparente. No lo redibujes, no cambies sus colores, no lo deformes y no lo encierres dentro de otra forma. Úsalo completo en bienvenida, acceso y estados celebratorios; usa una versión pequeña junto al nombre “Entrelazados” en encabezados cuando haya espacio.

Paleta oficial:

- Texto principal: `#0A0316`
- Fondo: `#FFFAFC`
- Primario morado: `#4D2A92`
- Secundario rosa: `#F561AD`
- Acento amarillo: `#F2DA30`

Construye una paleta de interfaz accesible a partir de esos colores:

- superficie principal blanca o casi blanca;
- fondos pastel muy suaves derivados del morado, rosa y amarillo;
- morado para acciones principales, navegación activa y foco;
- rosa para elementos afectivos, ilustraciones, chips seleccionados y momentos de conexión;
- amarillo solo como acento pequeño para celebración, resultado ganador o llamada visual; nunca para párrafos ni texto de bajo contraste;
- texto secundario en un tono ciruela/gris con contraste suficiente;
- verde sobrio para éxito y rojo frambuesa sobrio para peligro, sin romper la paleta;
- bordes finos en lavanda grisácea y sombras muy suaves.

Usa **Inter** como tipografía principal, con jerarquía clara y títulos grandes de peso 650-700. La personalidad debe provenir de la composición, el color y las formas, no de una tipografía decorativa difícil de leer.

Lenguaje visual:

- tarjetas con radio de 20-24 px;
- inputs y botones con radio de 12-16 px;
- mucho aire, bloques cortos y jerarquía muy clara;
- patrón gráfico sutil de líneas curvas o “hilos” que se cruzan, usado con moderación;
- ilustraciones abstractas u objetos cotidianos relacionados con una cita, no fotografías románticas genéricas en todas las pantallas;
- iconos lineales redondeados, consistentes y fáciles de reconocer;
- microanimaciones suaves: dos hilos que se unen al emparejarse, una pequeña celebración al ganar y transiciones discretas al completar el itinerario;
- evita glassmorphism excesivo, degradados saturados, corazones repetidos por todas partes, sombras pesadas, bloques enormes de color, interfaces tipo dashboard corporativo y estética infantil.

### 3. Principios de experiencia

1. **La pareja es el centro.** Usa nombres reales en la interfaz, por ejemplo “Sofi y Dani”, en vez de IDs técnicos.
2. **La próxima cita siempre es fácil de encontrar.** Si existe una cita programada, debe dominar Inicio.
3. **Planear es un flujo progresivo.** Primero se crea el plan, luego se agregan paradas y al final se programa la fecha.
4. **Ambas personas colaboran.** Cualquiera de las dos puede agregar o quitar elementos del itinerario mientras el plan está en estado de planeación.
5. **El juego es justo y privado.** La jugada de la otra persona permanece oculta hasta que ambas hayan elegido.
6. **El sistema nunca culpa.** Los errores deben explicar cómo continuar con un tono amable y directo.
7. **No inventes capacidades de backend.** No agregues chat, comentarios, fotos, notificaciones, ubicación en vivo, pagos, reservas, cambio de email/contraseña ni ruptura/desvinculación de pareja.

### 4. Arquitectura de información

En móvil usa una barra de navegación inferior persistente con cinco destinos:

1. **Inicio**
2. **Citas**
3. **Inspiración**
4. **Juego**
5. **Perfil**

En escritorio transforma esa navegación en una barra lateral compacta. Mantén el contenido principal en una columna de lectura cómoda, con un máximo aproximado de 1120 px. No conviertas la versión desktop en una tabla administrativa.

Antes de que exista una pareja activa, Inicio, Citas, Inspiración y Juego deben presentar un estado bloqueado amable que dirige al flujo **Conectar con mi pareja**. Perfil y seguridad sí permanecen disponibles.

### 5. Modelo funcional que debe respetar el prototipo

#### Usuario

Datos disponibles:

- id;
- correo electrónico;
- nombre;
- apellido;
- nombre completo;
- código personal de pareja de exactamente 12 caracteres;
- preferencias para citas.

El correo se muestra, pero la versión actual solo permite editar nombre y apellido. No muestres edición de correo.

#### Pareja

Datos disponibles:

- dos integrantes;
- fecha y hora en que comenzó la relación dentro de la aplicación;
- estado activo.

Cada persona puede pertenecer únicamente a una pareja activa. Una persona no puede vincularse consigo misma.

La respuesta actual de pareja contiene IDs, fecha de inicio y estado, pero no el perfil público de la otra persona. La UI nunca debe mostrar GUIDs: mientras no exista una respuesta enriquecida, usa “Tu pareja”. En los datos mock del prototipo sí se puede mostrar el nombre “Dani”.

#### Preferencias para citas

Existen tres listas independientes:

- actividades favoritas;
- actividades que se prefieren evitar;
- preferencias de comida, siempre expresadas de forma positiva.

Una misma actividad no puede ser favorita y evitada al mismo tiempo. No se permiten duplicados. Cada elemento acepta hasta 200 caracteres.

#### Plan de cita

Datos disponibles:

- pareja propietaria;
- persona responsable de haberlo planificado;
- fecha programada opcional;
- estado;
- itinerario ordenado.

Estados y reglas:

- **Planeando:** estado inicial. Se pueden agregar y quitar puntos del itinerario. La fecha aún puede estar vacía.
- **Programada:** requiere al menos un punto de itinerario y una fecha. Después de programarla, el itinerario queda de solo lectura.
- **Cancelada:** estado final y de solo lectura.
- **Realizada:** solo puede marcarse desde Programada; es estado final y aparece en el historial.

Una cita Planeando o Programada se puede cancelar. Una cita Cancelada o Realizada no se puede cancelar otra vez.

#### Punto de itinerario

Datos disponibles:

- orden automático comenzando en 1;
- nombre obligatorio, máximo 200 caracteres;
- hora de inicio opcional;
- ubicación opcional representada por latitud y longitud.

Si existe ubicación, latitud y longitud deben existir juntas. En la UI puedes ofrecer búsqueda de lugar o selección en mapa para que la interacción sea humana, pero el resultado final debe expresarse como coordenadas y no debes asumir que existe un campo de dirección persistido. Un punto puede no tener hora y puede no tener ubicación.

#### Juego para decidir quién planifica

Tipos disponibles:

- **Piedra, papel o tijera**: movimientos `rock`, `paper`, `scissors`.
- **El número más alto gana**: cada persona elige un entero del 1 al 6.

Estados de partida:

- Lista para comenzar;
- En progreso;
- Finalizada.

Cada ronda está En progreso o Finalizada. Cada persona puede jugar una sola vez por ronda. La jugada propia se puede ver después de enviarla, pero la jugada de la pareja debe aparecer oculta mientras la ronda siga abierta. Cuando ambas personas hayan jugado, se revelan simultáneamente las dos elecciones y el resultado.

Ambos juegos son **primero en ganar 2 rondas**, no simplemente una sola ronda. Los empates no suman victoria y requieren iniciar otra ronda. Cuando alguien alcanza dos victorias, la partida termina, esa persona se convierte en responsable de planificar la próxima cita y el sistema crea automáticamente un nuevo plan en estado Planeando.

La partida funciona en tiempo real. Diseña estados de conexión y espera: “Esperando a Dani…”, “Dani ya eligió”, reconexión y actualización del resultado sin recargar la pantalla.

#### Sesiones de acceso

La aplicación conoce únicamente:

- id de sesión;
- familia de sesión;
- fecha de creación;
- fecha de expiración.

Permite listar sesiones activas, cerrar la sesión actual mediante el refresh token que conoce el cliente y cerrar todas las sesiones. La API actual no permite revocar una fila arbitraria por su id, así que las filas de otras sesiones deben ser informativas y no llevar una acción individual. No inventes nombre de dispositivo, ciudad, navegador o dirección IP, porque esos datos no existen todavía.

### 6. Flujos y pantallas obligatorias

Construye todas las siguientes pantallas y conéctalas en un prototipo funcional.

#### A. Bienvenida, registro e inicio de sesión

1. **Bienvenida**
   - logo protagonista;
   - frase “Planeen menos. Vivan más juntos.”;
   - explicación breve de Entrelazados;
   - botones “Crear cuenta” e “Iniciar sesión”.

2. **Crear cuenta**
   - nombre;
   - apellido;
   - correo;
   - contraseña;
   - confirmación de contraseña solo como ayuda del frontend;
   - contraseña entre 8 y 128 caracteres;
   - nombre y apellido máximo 100 caracteres;
   - correo válido, único y máximo 320 caracteres;
   - checkbox de mostrar/ocultar contraseña;
   - errores inline y resumen amable si falla el envío.

3. **Iniciar sesión**
   - correo;
   - contraseña;
   - mostrar/ocultar contraseña;
   - error general “El correo o la contraseña no son válidos.” sin revelar cuál falló.

4. Incluye estados de carga, credenciales inválidas, correo ya registrado, sesión expirada y recuperación automática mediante refresh token como comportamiento silencioso. No agregues flujo de “Olvidé mi contraseña” porque aún no existe.

#### B. Conectar con la pareja

1. Estado previo al emparejamiento con dos acciones equivalentes:
   - **Compartir mi código**: mostrar el código personal de 12 caracteres en una tarjeta clara, con copiar y compartir;
   - **Ingresar código de mi pareja**: campo de 12 caracteres, pegado rápido y CTA “Conectar”.
2. Mostrar vista previa de confirmación antes de conectar, si se dispone del nombre en el mock.
3. Estados: código inválido, código no encontrado, intento de usar el código propio, una de las personas ya tiene pareja y éxito.
4. En éxito, animar los dos hilos del logo uniéndose y mostrar “Ya están entrelazados”. Después dirigir a Inicio.

#### C. Inicio de la pareja

Diseña variantes de Inicio para:

1. **Sin pareja:** CTA principal para conectar.
2. **Pareja conectada sin plan:** saludo “Buenos días, Sofi” y bloque compartido “Sofi y Dani”; CTA principal “Planear nuestra próxima cita”; accesos secundarios “Que el juego decida” y “Necesito inspiración”.
3. **Plan en progreso:** tarjeta “Su próxima cita está tomando forma”, persona responsable, cantidad de puntos y CTA “Continuar planeando”.
4. **Próxima cita programada:** esta tarjeta debe ser el foco principal. Mostrar fecha, cuenta regresiva, persona que la planeó, primer punto del itinerario, cantidad total de paradas y mini mapa si hay ubicaciones. Acciones “Ver plan” y una secundaria de calendario visual, sin prometer sincronización externa.
5. **Después de la cita:** CTA discreto para marcarla como realizada. Tras confirmar, mostrar una microcelebración y enviarla al historial.

Incluye en la parte inferior un acceso a las citas anteriores y una tarjeta breve de preferencias pendientes, sin saturar el dashboard.

#### D. Elegir al planificador con un juego

1. Pantalla de selección con dos tarjetas:
   - Piedra, papel o tijera;
   - El número más alto gana.
2. Antes de comenzar, explicar claramente: “La primera persona en ganar 2 rondas planifica la próxima cita”.
3. Lobby/espera de pareja con estado de conexión en tiempo real.
4. Partida de piedra, papel o tijera:
   - tres botones grandes, accesibles y visuales;
   - marcador de victorias de ambas personas;
   - número de ronda;
   - tras elegir, bloquear el control propio y mostrar “Elección enviada”; la elección rival sigue cubierta;
   - al recibir ambas elecciones, revelar, mostrar ganador o empate y habilitar “Siguiente ronda”.
5. Partida de número más alto:
   - seis opciones del 1 al 6;
   - mismas reglas de privacidad, marcador y rondas.
6. Resultado final:
   - celebración contenida;
   - “Dani planifica la próxima cita”;
   - mensaje de que ya se creó un plan nuevo;
   - CTA “Empezar a planear” y secundaria “Ir a Inicio”.
7. Diseña también: error de conexión, reconexión, pareja aún no elige, movimiento duplicado, ronda ya terminada y partida finalizada.

#### E. Citas y planeación manual

La sección Citas debe tener tabs o filtros claros: **Próxima**, **Planeando** e **Historial**.

1. **Crear plan manualmente**
   - una persona puede iniciar un plan y queda registrada como quien lo planeó;
   - después se abre el constructor de itinerario.

2. **Constructor de itinerario en estado Planeando**
   - encabezado con estado, responsable y colaboración de la pareja;
   - timeline vertical numerado;
   - botón “Agregar punto”;
   - formulario en bottom sheet móvil / panel lateral desktop con nombre, hora opcional y ubicación opcional;
   - selector de mapa o búsqueda de lugar que termine en latitud + longitud;
   - permitir quitar puntos con confirmación ligera y reorden visual coherente con el orden automático;
   - empty state: “Empiecen con algo que les emocione”;
   - resumen con cantidad de paradas;
   - CTA fijo “Programar cita”.

3. **Programar cita**
   - seleccionar fecha;
   - repasar itinerario;
   - impedir programación si no existe al menos un punto;
   - confirmación final que cambia el estado a Programada;
   - no inventar una hora global para la cita; las horas pertenecen a cada punto.

4. **Detalle de próxima cita Programada**
   - fecha y cuenta regresiva;
   - nombre de la persona que la planeó;
   - itinerario ordenado con horas opcionales;
   - mapa con uno o varios pines solo para puntos con ubicación;
   - mostrar de manera elegante los puntos sin ubicación, sin ocultarlos;
   - CTA “Marcar como realizada”;
   - acción destructiva secundaria “Cancelar cita”, con confirmación.

5. **Cita Cancelada**
   - estado final de solo lectura;
   - conservar itinerario y fecha si existían;
   - no mostrar acciones inválidas.

6. **Cita Realizada**
   - estado final de solo lectura;
   - mostrar en Historial.

7. **Historial de citas** - definido como requerimiento de producto, aunque la API de listado todavía no está implementada:
   - lista de citas realizadas y canceladas;
   - filtro por mes;
   - paginación o carga incremental clara;
   - cada tarjeta muestra fecha, estado, persona que planeó, primer punto y cantidad de paradas;
   - detalle reutiliza la vista del itinerario y mapa en modo lectura;
   - empty state amable.

#### F. Inspiración para citas

Este es un requerimiento de producto con modelo de datos preparado, aunque el flujo de generación aún no está expuesto por la API. Diseña la experiencia completa y marca su integración como futura en la estructura de código; en el prototipo sí debe funcionar con datos mock.

1. Formulario “Ayúdennos a imaginar una cita”:
   - presupuesto preferido, valor no negativo;
   - ciudad, obligatoria, máximo 150 caracteres;
   - hora de inicio opcional;
   - hora de fin opcional;
   - resumen de gustos de la pareja: actividades favoritas, actividades a evitar y preferencias de comida;
   - CTA “Crear una idea para nosotros”.
2. Estado de generación con mensajes cálidos y breves; no muestres detalles técnicos de IA.
3. Resultado de idea:
   - presupuesto preferido;
   - presupuesto estimado opcional;
   - ciudad;
   - rango horario opcional;
   - itinerario ordenado;
   - por cada punto: nombre, etiquetas, hora opcional y ubicación opcional;
   - mapa si hay ubicaciones;
   - CTA “Convertir en nuestro plan”;
   - acciones “Intentar otra idea” y “Ajustar datos”.
4. Al aceptar una idea, convertirla en un plan Planeando, vincular ambos conceptos y permitir editar el itinerario antes de programarlo.
5. No inventes calificaciones, comentarios de comunidad, fotos obligatorias, reservas ni disponibilidad de negocios.

#### G. Perfil y preferencias

1. **Perfil**
   - nombre y apellido editables;
   - correo visible y de solo lectura;
   - código personal de pareja con copiar/compartir;
   - si existe pareja, mostrar nombres, fecha de conexión y estado activo;
   - sin opción de desvincular porque no existe esa operación.
2. **Preferencias para citas**
   - tres secciones con chips: “Nos encanta”, “Preferimos evitar” y “Comida que disfruto”;
   - agregar por input o sugerencias visuales;
   - quitar desde cada chip;
   - impedir duplicados;
   - si se intenta agregar como evitada una actividad favorita, explicar el conflicto y ofrecer quitarla primero, sin modificar datos automáticamente;
   - máximo 200 caracteres por elemento;
   - mostrar progreso/estado vacío sin convertirlo en cuestionario largo.
3. **Seguridad y sesiones**
   - lista de sesiones con fecha de inicio y expiración;
   - nombrarlas de forma neutral, por ejemplo “Sesión iniciada el 9 sep, 14:30”, sin inventar dispositivo;
   - filas informativas sin acción individual para sesiones ajenas, ya que solo se puede revocar un refresh token que el cliente posee;
   - acción “Cerrar todas las sesiones” con confirmación;
   - acción general “Cerrar sesión” para revocar la sesión actual.

### 7. Datos mock para que el prototipo se sienta real

Usa un estado inicial autenticado y emparejado:

- persona actual: **Sofi Martínez**;
- pareja: **Dani Rivera**;
- correo: `sofi@ejemplo.com`;
- código de pareja: `ENLACE-9X2Q7` (12 caracteres contando el guion; úsalo solo como dato de demostración);
- relación activa desde el 14 de febrero de 2025;
- favoritas: “Cine”, “Caminar al atardecer”, “Museos”;
- a evitar: “Karaoke”, “Deportes extremos”;
- comida: “Sushi”, “Comida italiana”, “Opciones vegetarianas”.

Ejemplo de próxima cita programada:

- fecha cercana respecto al contexto del prototipo;
- responsable: Dani;
- punto 1: “Ver el atardecer en Playa Delfines”, 18:00, con coordenadas válidas;
- punto 2: “Cena frente al mar”, 20:00, con coordenadas válidas;
- punto 3: “Helado y caminata”, sin ubicación para demostrar el estado opcional.

Ejemplo de idea generada:

- presupuesto preferido: `$1,500 MXN`;
- presupuesto estimado: `$1,280 MXN`;
- ciudad: Cancún;
- horario: 17:30 a 22:00;
- tres puntos de itinerario con etiquetas como “al aire libre”, “tranquilo” y “vegetariano”.

Incluye un control discreto de demostración, separado del producto final, para cambiar entre estados: sin pareja, sin plan, planeando, programada, juego en curso, idea generada e historial. Esto permite revisar todos los casos sin depender de un backend.

### 8. Comportamiento y validaciones

- Los formularios deben tener labels persistentes, ayuda contextual y errores inline.
- Deshabilita CTAs solo cuando la razón sea evidente; si no, permite pulsar y explica qué falta.
- Usa toasts únicamente para confirmaciones breves; los errores que requieren acción deben permanecer visibles.
- Toda acción destructiva requiere confirmación: cancelar cita, cerrar la sesión actual y cerrar todas las sesiones.
- Agregar y quitar preferencias debe actualizar la vista al instante.
- Agregar un punto debe insertarlo al final con orden consecutivo.
- El itinerario solo es editable en Planeando.
- Programar requiere una fecha y al menos un punto.
- Marcar como realizada solo está disponible en Programada.
- Después de terminar un juego, crea inmediatamente un plan Planeando cuyo responsable sea la persona ganadora.
- En una ronda activa, nunca reveles el código de la jugada rival hasta que la ronda termine.
- Trata las fechas/horas en lenguaje local y legible; no muestres UTC al usuario.
- Incluye skeletons, estados vacíos, errores, offline/reconexión para el juego y confirmaciones de éxito.

### 9. Componentes que debe contener el sistema de diseño

Crea componentes reutilizables y sus variantes:

- logo lockup y símbolo;
- top app bar y navegación móvil/desktop;
- botones primary, secondary, tonal, ghost y destructive;
- inputs de texto, email, contraseña, moneda, fecha, hora, código y búsqueda de lugar;
- chips de preferencia y tags;
- tarjeta de próxima cita;
- tarjeta de cita histórica;
- tarjeta de idea;
- timeline de itinerario;
- status badge para Planeando, Programada, Cancelada y Realizada;
- marcador de juego;
- opción de movimiento seleccionable, oculta y revelada;
- mapa placeholder con pines;
- bottom sheet, modal de confirmación y panel lateral;
- toast, banner inline, skeleton, empty state y error state;
- session row;
- contador o countdown accesible;
- foco, hover, pressed, disabled y loading.

Usa una cuadrícula de 8 px, targets táctiles mínimos de 44 x 44 px y contraste WCAG AA. No dependas solo del color para comunicar estado. Agrega texto o icono además del color. Respeta `prefers-reduced-motion` en la implementación.

### 10. Entregable esperado

Entrega un prototipo frontend funcional y responsive con:

- todas las rutas/pantallas descritas;
- componentes reutilizables, no bloques duplicados;
- navegación conectada;
- interacciones mock funcionales;
- estado local de demostración claramente separado de una futura capa de API;
- diseño mobile-first para aproximadamente 390 x 844 px;
- versión tablet y desktop de las pantallas principales;
- copy final en español, natural, afectuoso y conciso;
- sin lorem ipsum;
- sin revelar GUIDs, enums en inglés, códigos internos de error ni coordenadas crudas en las pantallas normales;
- una pequeña página interna “UI Kit” para revisar tokens y variantes, sin incluirla en la navegación del producto.

Antes de dar por terminado el trabajo, revisa estas historias de extremo a extremo:

1. Crear cuenta -> iniciar sesión -> compartir/ingresar código -> formar pareja -> llegar a Inicio.
2. Elegir juego -> enviar movimientos privados -> resolver empates -> ganar 2 rondas -> crear plan para la persona ganadora.
3. Crear plan -> agregar varios puntos con y sin hora/ubicación -> programar -> consultar mapa e itinerario -> marcar como realizada.
4. Pedir inspiración -> revisar presupuesto e itinerario -> convertir idea en plan -> editar -> programar.
5. Editar preferencias -> resolver conflicto favorita/evitada -> ver preferencias reflejadas en Inspiración.
6. Consultar historial por mes y abrir una cita realizada o cancelada.
7. Revisar sesiones -> cerrar la sesión actual -> cerrar todas.

El resultado final debe sentirse como un espacio privado y querido que dos personas realmente querrían abrir antes de una cita: útil primero, emocional en los detalles y visualmente inconfundible gracias al motivo de los hilos entrelazados.

## FIN DEL PROMPT

---

## Notas de alcance para el equipo (no es necesario pegarlas en Figma Make)

- El frontend actual del repositorio es todavía el starter de Vite/React y no aporta flujos reales ni un sistema visual que deba conservarse.
- Ya existen en backend: registro, login/refresh, consulta y revocación de sesiones, perfil, cambio de nombre, preferencias, vínculo de pareja, consulta de pareja, CRUD parcial y transiciones del plan de cita, y los dos juegos en tiempo real.
- Están documentados o modelados, pero aún no cuentan con una API completa: listado de próxima cita, historial con filtro/paginación y generación/aceptación de ideas mediante IA.
- El prompt incluye esos tres flujos como diseño objetivo y prototipo mock, pero evita presentarlos como integración ya disponible.
