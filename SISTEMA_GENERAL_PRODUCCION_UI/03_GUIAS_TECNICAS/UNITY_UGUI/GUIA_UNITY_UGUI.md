# Módulo técnico: Unity UGUI

Aplicar únicamente en proyectos que utilicen Canvas, RectTransform, GraphicRaycaster y
componentes UGUI. Las versiones y paquetes objetivo se registran en la ficha del proyecto.

## Seguridad del proyecto

- Resolver la ruta absoluta del proyecto antes de automatizar.
- No abrir dos instancias normales o batch sobre la misma carpeta.
- Cerrar únicamente el Editor que usa el proyecto objetivo antes de escribir escenas,
  prefabs, assets serializados o metadatos externamente.
- Confirmar compilación, Package Manager, licencia, espacio de escritura y log nuevo.
- Una herramienta debe ser idempotente y existir un solo constructor canónico por raíz.

## Ejecución batch y licenciamiento

- Ejecutar un preflight antes de cada proceso batch: bloquear si existe cualquier
  `Unity.exe`, un `Unity Hub.exe` con `-runTests` o el `Temp/UnityLockfile` del proyecto.
  La exclusividad es global porque licencia, cachés y servicios auxiliares pueden ser
  compartidos aun cuando los proyectos sean distintos.
- No terminar una instancia ajena para liberar el preflight. Esperar a que finalice o pedir
  al propietario que la cierre; la defensa sólo inspecciona y bloquea.
- Invocar directamente el `Unity.exe` de la versión registrada. Unity Hub no es el corredor
  canónico de pruebas automatizadas.
- Ejecutar con permisos normales de escritura sobre licencia, cachés, `Library`, `Temp`,
  logs y resultados. Un fallo de acceso se informa como bloqueo, no como error funcional.
- No usar `-noUpm` cuando el proyecto dependa de paquetes: puede dejar sin resolver tipos de
  UI o pruebas y producir errores engañosos.
- No combinar `-quit` con `-runTests`: el Test Runner es quien debe cerrar el proceso al
  completar y escribir el XML. El cierre anticipado puede devolver una ejecución incompleta.
- No usar `-nographics` cuando se produzca evidencia visual. Una captura creada sin render
  válido puede ser gris, vacía o de baja información aunque el archivo exista.
- Esperar el proceso real, conservar su código de salida y exigir log nuevo, resultado XML y
  conteo de pruebas coherentes antes de declarar PASS.

La defensa reutilizable es `05_QA/Test-UnityBatchPreflight.ps1`. Debe ejecutarse antes de
abrir Unity y nunca incluye `Stop-Process`, `taskkill` ni otra terminación automática.

## Jerarquía y layout

- Aplicar Safe Area mediante un único propietario.
- Usar anclas y pivotes que expresen la intención del layout.
- Separar el contenedor estable de la capa animada.
- Evitar Layout Groups anidados en composiciones fijas; medir antes de retirarlos.
- No usar activeSelf simultáneamente como visibilidad y señal de geometría.
- Medir la cadena de padres cuando un hijo se desplaza sin cambiar su posición local.
- En raster, elegir explícitamente contener o cubrir; nunca deformar ancho y alto de forma
  independiente. Un fondo que cubre necesita recorte local y un pivote consciente.
- El orden de jerarquía debe expresar el dibujo; documentar todo Canvas adicional y
  `overrideSorting` que pueda alterar capas.

## Interacción

- Un Canvas interactivo necesita GraphicRaycaster y un EventSystem válido.
- Mantener raycastTarget sólo en superficies que reciben interacción.
- Button.targetGraphic debe existir, estar activo y aceptar raycast.
- Los fondos, textos e iconos decorativos no interceptan eventos.
- Validar mediante GraphicRaycaster y el Button visible; onClick.Invoke o una llamada
  directa no demuestran la ruta física del jugador.
- Los controles ocultos no reciben eventos y los modales bloquean sólo el ámbito previsto.

## Datos y refresco

- No reconstruir jerarquía, anclas o layout durante un refresco de datos.
- Actualizar por evento o a frecuencia limitada y evitar reasignar valores idénticos.
- Separar datos, animación y layout.
- Los MonoBehaviour serializados viven en archivos cuyo nombre coincide con su clase.

## Render y rendimiento

- Separar Canvas por frecuencia de cambio sólo cuando el perfilado lo justifique.
- Medir Canvas.BuildBatch, reconstrucciones, batches, draw calls, overdraw, memoria,
  objetos activos, raycasts y asignaciones por fotograma.
- Usar presets de importación por familia y Sprite Atlas sólo para sprites finales que
  aparecen juntos.
- No incluir referencias, conceptos o capturas en atlas o dependencias de producción.
- Un gráfico con alfa cero puede seguir generando coste o recibir raycast. Desactivarlo al
  terminar una transición y retirar interacción de la capa saliente.

## Transiciones

- Preparar y activar el destino antes de reducir el origen.
- Usar una única función temporal para fondos, objeto, cara, velo y paralaje.
- Mantener al menos una capa ambiental visible durante todo el cruce.
- Recordar que el alfa de CanvasGroup anidados se multiplica.
- Usar tiempo no escalado cuando la transición deba funcionar con `timeScale` alterado.
- Fijar explícitamente el estado final antes de desactivar la fuente.
- Validar capturas a 0 %, 25 %, 50 %, 75 % y 100 %.

## Pruebas

- Edit Mode: estructura, referencias, medidas y propietarios.
- Play Mode: varios fotogramas, interacción, navegación, animación y ciclo de vida.
- Device Simulator: relación de aspecto, Safe Area y orientación.
- Dos resoluciones con la misma relación sólo validan escala; añadir al menos un aspecto o
  Safe Area extremo como diagnóstico cuando el producto soporte dispositivos variados.
- Cuando una captura automatizada cambie resolución, RenderTexture o destino de Canvas,
  recalcular `CanvasScaler` y el layout antes de capturar. Usar `Canvas.ForceUpdateCanvases`
  y, cuando corresponda, `LayoutRebuilder.ForceRebuildLayoutImmediate` sobre la raíz
  propietaria; no reutilizar la geometría calculada para la resolución anterior.
- Comprobar que el orden de las capturas no altere el resultado: la segunda resolución debe
  conservar encuadre, scroll, anclas y contenido. Al finalizar, restaurar resolución,
  RenderTexture, CanvasScaler y geometría temporal antes de devolver el control o cerrar.
- Dispositivo real: legibilidad, tacto, memoria, rendimiento y artefactos de escalado.
- Confirmar PASS específico, salida correcta, log sin excepciones y evidencia reciente.
