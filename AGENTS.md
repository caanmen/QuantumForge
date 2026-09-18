# Quantum Forge — instrucciones persistentes

Estas instrucciones aplican a todo el proyecto. Una petición actual y explícita del usuario siempre tiene prioridad.

## Antes de trabajar

1. En un chat nuevo de Quantum Forge o antes de un bloque amplio y transversal, lee `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`. Para una corrección pequeña y localizada, consulta sólo las reglas, el estado y los archivos directamente relacionados.
2. Si el usuario pide revisar la carpeta de producción, aplica `SISTEMA_GENERAL_PRODUCCION_UI/00_INICIO/ENRUTADOR_DE_PROYECTOS_Y_TAREAS.md`: conoce el inventario y lee sólo lo aplicable, nunca todo el árbol.
3. Inspecciona los archivos reales implicados; no supongas que un resumen sustituye al proyecto.
4. No uses Git para escribir, crear commits, ramas, staging, reset o push salvo petición explícita.
5. Conserva los cambios del usuario y evita modificaciones ajenas al alcance.

## Puerta de aceptación

- Para lotes, integraciones, cierres o entregables sustanciales, aplica la skill `puerta-aceptacion`.
- La skill contiene el procedimiento y los criterios vigentes; no los dupliques aquí.
- Las pruebas específicas de Quantum Forge y la aprobación visual o creativa del usuario siguen siendo obligatorias cuando correspondan.

## Calidad de código y evolución segura

- Para crear, corregir, integrar o refactorizar código, aplica
  `SISTEMA_GENERAL_PRODUCCION_UI/03_GUIAS_TECNICAS/CODIGO/GUIA_DESARROLLO_Y_CALIDAD_CODIGO.md`
  y consulta el mapa específico
  `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/11_CODIGO/MAPA_CODIGO_Y_VALIDACION.md`.
- El usuario puede describir objetivos y fallos con lenguaje natural; el asistente localiza
  propietarios, dependencias, riesgos y pruebas sin exigir nombres de archivos ni tablas.
- Antes de cambiar código, inspecciona la implementación y sus consumidores; evalúa impacto
  en persistencia, migraciones, ciclo de vida, progreso offline, UI, localización y
  rendimiento sólo cuando corresponda.
- No mezcles el bloque solicitado con una refactorización general. No amplíes una clase
  concentradora si la responsabilidad nueva puede tener un propietario delimitado; en
  código heredado, extrae por rebanadas compatibles y verificables.
- Todo cambio persistente requiere pruebas proporcionales de guardado, carga, compatibilidad
  y recuperación. Un fallo repetido debe convertirse en aserción, prueba o validador.
- Para seleccionar validadores existentes, usa
  `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/11_CODIGO/INDICE_VALIDADORES_CODIGO.md`;
  no ejecutes los 109 validadores por rutina.

## Uso de agentes

- Aplica `SISTEMA_GENERAL_PRODUCCION_UI/02_FLUJO_DE_TRABAJO/REGLAS_USO_AGENTES.md`.
- El agente principal coordina, conserva el contexto, integra y realiza la validación final.
- Usa agentes auxiliares cuando existan auditorías independientes, investigación amplia o
  riesgos visuales, técnicos o de QA que se beneficien de revisión paralela.
- Para cambios pequeños, locales o estrictamente secuenciales, trabaja sin delegación.
- Los agentes auxiliares trabajan en modo lectura por defecto.
- Cada archivo, escena, prefab, asset serializado, herramienta o sistema tiene un solo
  escritor durante el bloque. No permitas ediciones solapadas.
- Ningún agente ejecuta otra instancia de Unity sobre la misma ruta del proyecto.
- Verifica los hallazgos contra los archivos reales; el consenso de agentes no reemplaza
  pruebas nuevas ni la aprobación visual o creativa del usuario.

## Trabajo de interfaz obligatorio

Para crear o rediseñar una pantalla, lee completamente:

- `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/00_INICIO/INSTRUCCIONES_MAESTRAS_UI.md`
- `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/01_GUIAS/GUIA_CREACION_CORRECTA_PANTALLAS.txt`
- `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/01_GUIAS/GUIA_PREVENCION_ERRORES_PANTALLAS.txt`
- El perfil de estilo de la dimensión correspondiente.
- La ficha, referencia y decisiones registradas de la pantalla afectada.

Para una corrección o auditoría localizada, usa el enrutador y consulta únicamente la ficha, las decisiones, el perfil de estilo, los aprendizajes coincidentes y las secciones técnicas relacionadas con el fallo. Amplía la lectura si el cambio altera la composición, el contrato compartido o el flujo completo.

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
- Archivar referencias permanentes dentro de `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS`; no depender de rutas temporales del chat.
- Archivar capturas aprobadas en `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/06_CAPTURAS_APROBADAS`.

## Unity

- Se permiten varias instancias de Unity cuando cada una usa una carpeta de proyecto
  diferente. No confundas proyectos visibles en Unity Hub con editores realmente abiertos.
- Nunca abras dos instancias, normales o batch, sobre la misma ruta de proyecto.
- Antes de modificar escenas, prefabs, assets serializados o ejecutar una reconstrucción
  batch, resuelve la ruta absoluta del proyecto objetivo y comprueba específicamente que
  esa carpeta no esté abierta en otra instancia. Las instancias de otros proyectos no
  obligan a cerrarlas.
- Si `Quantum Forge` está abierto, pide cerrar únicamente esa instancia antes de una
  escritura serializada o un batch dirigido a `Quantum Forge`.
- Las pruebas que necesitan varios fotogramas deben cerrar Unity por sí mismas al finalizar; no uses un cierre inmediato que aborte el modo de juego.
- Confirma el mensaje PASS específico del método ejecutado y revisa errores o excepciones.

## Sistema visual existente

- Reutiliza `VerticalUiTheme` y los componentes existentes cuando correspondan; no crees un segundo sistema de estilos incompatible.
- Los perfiles de dimensiones no configuradas son marcadores pendientes, no autorización para inventar su apariencia.
- La primera pantalla aprobada de una nueva dimensión debe utilizarse para documentar su perfil antes de generalizar componentes.
