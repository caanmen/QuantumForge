# Continuidad: Prestigio y selección dimensional animada

Fecha: 2026-08-03  
Proyecto: `C:\Users\nedfla\Quantum Forge`  
Objetivo inmediato: construir la transición de Prestigio 1 y la nueva pantalla vertical animada para escoger una dimensión.

## Referencias visuales preservadas en el proyecto

- Referencia horizontal original:
  `Assets/Project/UI/Vertical/Prestige/Concepts/QF_DimensionalSelection_Landscape_Reference_v1.png`
- Propuesta vertical en formato carrusel:
  `Assets/Project/UI/Vertical/Prestige/Concepts/QF_DimensionalSelection_Portrait_Carousel_v1.png`

La propuesta vertical presenta la Dimensión II grande en el centro y deja visibles parcialmente las dimensiones I y III a los lados. Incluye el título `SELECCIÓN DIMENSIONAL`, tres indicadores inferiores y el botón `SINTONIZAR`.

La propuesta vertical es el punto de partida visual. Antes de integrarla definitivamente conviene enseñársela otra vez al usuario y confirmar si desea conservar exactamente esa composición o hacer un último ajuste.

## Decisión narrativa y de interacción

El flujo acordado es:

1. El jugador cumple los requisitos de Prestigio 1.
2. Pulsa el botón de Prestigio.
3. Se reproduce una animación breve de transición o salto dimensional.
4. La transición desemboca en la pantalla vertical de selección dimensional.
5. El jugador desliza el carrusel y escoge una dimensión disponible.
6. Se reproduce una animación corta de sintonización/confirmación.
7. Sólo entonces se ejecuta el reinicio de Prestigio 1 y se desbloquea la dimensión elegida.
8. Se muestra el objetivo de la dimensión revelada y el jugador puede entrar.

No se debe ejecutar el reinicio antes de que el jugador confirme una dimensión. Esto evita perder o modificar el estado si sale de la pantalla sin elegir.

## Estado actual del código de Prestigio 1

La lógica funcional ya está programada.

- `Assets/Project/Scripts/UI/PrestigeUI.cs`
  - `OnClickPrestige()` comprueba si el Prestigio está disponible.
  - Actualmente abre una selección dimensional básica creada mediante código.
  - `SelectDimensionForPrestige1(int dimensionId)` llama al reinicio después de escoger.
  - Después actualiza las pestañas y muestra el objetivo dimensional.
- `Assets/Project/Scripts/Core/GameState.cs`
  - `CanOpenPrestige1Selection(...)` valida requisitos.
  - `DoPrestige1Reset(int selectedDimensionId, ...)` realiza el reinicio.
  - Conserva el conocimiento/meta-progreso correspondiente.
  - Desbloquea la dimensión elegida.
  - Guarda la partida.

La pantalla visual nueva y las animaciones todavía no se han implementado. Hasta ahora solamente se generó el concepto vertical.

## Animaciones acordadas para la pantalla

Las siluetas principales deben permanecer estables. Se animarán solamente los fondos, luces ambientales y partículas.

### Dimensión I

- Galaxia girando muy lentamente.
- Capas de estrellas con paralaje suave.
- Destellos ocasionales y discretos.
- Movimiento mínimo del campo espacial.
- La nave, los planetas principales y el marco permanecen estables.

### Dimensión II

- Halo del eclipse/sol pulsando lentamente.
- Ligero desplazamiento o rotación de la corona posterior.
- Nubes del fondo moviéndose en distintas profundidades.
- Brillos violetas y naranjas muy suaves.
- La ciudad y sus siluetas permanecen estables.

### Dimensión III

- Humo industrial y vapor ascendiendo.
- Varias capas de niebla con velocidades diferentes.
- Luces lejanas con intermitencia ocasional.
- La megaciudad, las torres y las grúas permanecen estables.

## Dirección visual y técnica

- Mantener el estilo oscuro, metálico, antiguo y tecnológico de Quantum Forge.
- Conservar los números romanos `I`, `II` y `III` y un candado por tarjeta cuando corresponda.
- La pantalla es para móvil vertical, aproximadamente 1080 x 1920.
- Usar un carrusel horizontal: tarjeta central grande y tarjetas vecinas parcialmente visibles.
- Mantener márgenes seguros para dispositivos móviles.
- Las animaciones deben ser lentas y elegantes, no parecer un fondo de vídeo acelerado.
- Evitar mover edificios, nave o siluetas humanas.
- Evitar shaders de pantalla completa demasiado costosos para Android.
- Pausar partículas y actualizaciones cuando la pantalla no esté visible.
- Añadir una opción de movimiento reducido o una versión estática si se implementan ajustes de accesibilidad.

Para obtener profundidad convincente será necesario separar o recrear capas de cada ilustración: fondo, silueta, iluminación, partículas y marco. Puede hacerse en Unity con imágenes enmascaradas, materiales UI y sistemas de partículas; Blender no es necesario para esta pantalla.

## Estado del cubo de la Máquina antes de este bloque

El trabajo anterior del cubo quedó aprobado por el usuario:

- Cuatro caras físicas.
- 35 nodos públicos y 8 secretos, 43 en total.
- Caras 1 y 2 con modelos de Blender.
- Caras 3 y 4 procedurales con el mismo borde industrial PBR de tres capas.
- Sólo los nodos tienen luces.
- Paleta exclusiva por cara: cian, morado, naranja y turquesa.
- Los nodos dañados conservan el color de su cara y parpadean bajando su intensidad.
- Progresión visual dañada/reparada.
- Validación de Unity aprobada y escena `Assets/Project/Scenes/Main.unity` regenerada.

No deshacer ni reemplazar este trabajo al implementar la transición dimensional.

## Primer paso recomendado en el próximo chat

1. Leer este archivo completo.
2. Abrir las dos imágenes guardadas en `Assets/Project/UI/Vertical/Prestige/Concepts`.
3. Confirmar con el usuario que la composición vertical de carrusel queda aprobada.
4. Auditar `PrestigeUI`, la configuración actual del panel y las validaciones existentes.
5. Diseñar el controlador de estados de transición sin llamar todavía a `DoPrestige1Reset`.
6. Preparar primero un prototipo visual animado y enseñarlo antes de sustituir la pantalla funcional.

## Restricción importante del usuario

Antes de efectuar cambios grandes o integrar definitivamente la pantalla, mostrar el resultado visual y permitir que el usuario confirme si se parece suficientemente al concepto aprobado.

## Implementación realizada el 2026-08-03

- `PrestigeUI` inicia ahora la transición dimensional antes de mostrar el carrusel.
- La transición final usa una aproximación ilustrada 2D del cubo y conserva perfiles Alta, Media y Baja para sus efectos.
- El cubo permanece estático, acumula energía y desemboca en la pantalla vertical.
- La selección dimensional utiliza el arte aprobado de las dimensiones I, II y III.
- El carrusel admite pulsaciones laterales y deslizamiento horizontal.
- `SINTONIZAR` requiere exactamente tres pulsaciones: `1/3`, `2/3` y `3/3`.
- La confirmación se reinicia al cambiar de dimensión, volver o superar cinco segundos.
- El reinicio de Prestigio continúa ejecutándose solamente después de la tercera pulsación.
- La capa dimensional se muestra por encima de informes y avisos transitorios sin eliminarlos.
- La configuración visual se carga desde `Assets/Project/Resources/Prestige` para resistir regeneraciones de escena.
- La fase del cubo usa ahora un fondo vertical estático del laboratorio, con tuberías, paneles y luces tenues.
- Se retiraron de esa fase los títulos, subtítulos, estados, líneas, partículas decorativas y marcos alrededor del cubo.
- La cámara del cubo renderiza temporalmente con transparencia para integrarlo directamente sobre el laboratorio, sin rectángulo negro.
- El laboratorio se oscurece gradualmente mientras el cubo concentra energía y el destello abre la selección dimensional.
- Fondo aprobado e integrado: `Assets/Project/UI/Vertical/Prestige/Concepts/QF_PrestigeCube_LabBackground_Portrait_v1.png`.
- La cinemática previa fue reemplazada por una composición completamente 2D para evitar mezclar la silueta ilustrada con el cubo 3D.
- Duración actual: aproximadamente `8.6` segundos.
- Secuencia: personaje anónimo de espaldas, contacto con la máquina, aparición del cubo ilustrado, energía emitida por la máquina, apertura escalonada de tres portales, oscurecimiento y destello hacia dimensiones.
- El cubo 2D conserva la forma industrial, las luces rojas frontales y las luces violetas laterales del cubo real, pero no intenta girar entre caras.
- Los tres portales reutilizan un único aro 2D transparente y se diferencian mediante color cian, violeta y naranja.
- Alta usa 18 partículas y dos capas de aro; Media usa 12 partículas y dos capas; Baja usa 6 partículas y una capa.
- Los portales tienen doble borde orgánico, núcleo y halo procedurales ligeros; las estelas salen de la máquina y desaparecen al abrir cada portal, sin elementos orbitando alrededor.
- Los recursos 2D se importan sin mipmaps, con compresión y límites de tamaño para reducir memoria móvil.
- Recursos: `Assets/Project/UI/Vertical/Prestige/Cinematic/QF_PrestigeCube2D_v1.png`, `QF_PrestigeCharacterBackRaised_v1.png` y `QF_PrestigePortalRing2D_v1.png`.

Validaciones:

- Compilación de `Assembly-CSharp` y `Assembly-CSharp-Editor`: sin errores.
- Unity: `Prestige Dimension Validation PASS`.
- Captura en Play Mode: `Prestige Dimension Capture PASS` a 1080 x 1920.
- El guardado del usuario fue respaldado y restaurado durante la prueba.

Evidencias:

- `Logs/VisualQA/PrestigeDimension/01_interaccion_personaje_maquina_2d.png`
- `Logs/VisualQA/PrestigeDimension/02_energia_maquina_hacia_portales_2d.png`
- `Logs/VisualQA/PrestigeDimension/03_apertura_tres_portales_2d.png`
- `Logs/VisualQA/PrestigeDimension/04_seleccion_dimensional_vertical.png`
- `Logs/VisualQA/PrestigeDimension/05_confirmacion_sintonizar_2_de_3.png`
