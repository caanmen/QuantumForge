# Quantum Forge — instrucciones persistentes

Estas instrucciones aplican a todo el proyecto. Una petición actual y explícita del usuario siempre tiene prioridad.

## Antes de trabajar

1. Lee completamente `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`.
2. Inspecciona los archivos reales implicados; no supongas que un resumen sustituye al proyecto.
3. No uses Git para escribir, crear commits, ramas, staging, reset o push salvo petición explícita.
4. Conserva los cambios del usuario y evita modificaciones ajenas al alcance.

## Trabajo de interfaz obligatorio

Para crear, modificar o revisar cualquier pantalla, lee completamente:

- `SISTEMA_UI_QUANTUM_FORGE/00_INICIO/INSTRUCCIONES_MAESTRAS_UI.md`
- `SISTEMA_UI_QUANTUM_FORGE/01_GUIAS/GUIA_CREACION_CORRECTA_PANTALLAS.txt`
- `SISTEMA_UI_QUANTUM_FORGE/01_GUIAS/GUIA_PREVENCION_ERRORES_PANTALLAS.txt`
- El perfil de estilo de la dimensión correspondiente.
- La ficha, referencia y decisiones registradas de la pantalla afectada.

Reglas no negociables:

- No inventar contenido, mecánicas, nombres, estados, adornos o assets.
- Distinguir referencia, concepto, asset final y captura real de Unity.
- Usar assets canónicos y datos reales antes de crear sustitutos.
- Construir y aprobar primero la composición estática; añadir interacción y animación después.
- Una propiedad visual o funcional debe tener un único propietario.
- Mantener sincronizadas la escena/prefab y cualquier herramienta que los reconstruya.
- Validar estados neutral, seleccionado, bloqueado, vacío y completado cuando correspondan.
- Capturar a 1080×1920 y comprobar también 720×1280 cuando la pantalla sea vertical.
- No declarar una pantalla terminada sin evidencia nueva, pruebas exitosas y similitud visual mínima aprobada del 95 %.
- Archivar referencias permanentes dentro de `SISTEMA_UI_QUANTUM_FORGE/05_REFERENCIAS`; no depender de rutas temporales del chat.
- Archivar capturas aprobadas en `SISTEMA_UI_QUANTUM_FORGE/06_CAPTURAS_APROBADAS`.

## Unity

- No ejecutes Unity batch mientras el editor normal esté abierto.
- Pide cerrar Unity antes de modificar escenas, prefabs, assets serializados o ejecutar reconstrucciones batch.
- Las pruebas que necesitan varios fotogramas deben cerrar Unity por sí mismas al finalizar; no uses un cierre inmediato que aborte el modo de juego.
- Confirma el mensaje PASS específico del método ejecutado y revisa errores o excepciones.

## Sistema visual existente

- Reutiliza `VerticalUiTheme` y los componentes existentes cuando correspondan; no crees un segundo sistema de estilos incompatible.
- Los perfiles de dimensiones no configuradas son marcadores pendientes, no autorización para inventar su apariencia.
- La primera pantalla aprobada de una nueva dimensión debe utilizarse para documentar su perfil antes de generalizar componentes.

