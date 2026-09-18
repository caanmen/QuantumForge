# Módulo técnico: móvil y Android

Este módulo es opcional. Cada proyecto define orientación, densidades, relaciones de
aspecto, métodos de entrada, ABI, SDK, paquete, firma y dispositivos objetivo.

## Diseño móvil

- Trabajar desde áreas físicas tocables y no sólo desde el tamaño visual del icono.
- Probar Safe Areas extremas, barras del sistema, cámara y gestos.
- Revisar la resolución y relación de aspecto menor sin ampliar la captura.
- Mantener controles indispensables visibles, legibles y descubribles.
- Definir botón Atrás, prioridad de modales y restauración del estado anterior.

## Rendimiento

- Medir en el dispositivo objetivo y en el estado visual más costoso.
- Establecer presupuestos de CPU, GPU, memoria, texturas, overdraw y refresco.
- Detener animaciones, partículas y actualizaciones cuando la pantalla no está visible.
- Comparar toda optimización contra una línea base equivalente.
- Definir FPS y tiempo por fotograma, memoria, tamaño de build, carga y duración mínima de
  prueba sostenida antes de optimizar.
- Medir al menos un dispositivo representativo bajo, medio y alto cuando el alcance lo exija.
- No perseguir FPS máximos si aumentan calor, batería o throttling; buscar cadencia estable.

## Build y continuidad

- Registrar paquete, versionName, versionCode, firma pública, ABI, Min SDK y Target SDK.
- Mantener secretos y claves privadas fuera del proyecto y de su documentación compartida.
- Probar instalación nueva en un objetivo limpio independiente.
- Probar actualización sobre progreso respaldado sin desinstalar ni borrar datos.
- Verificar migración y compatibilidad del guardado antes y después de actualizar.
- Comparar tamaño y hash de la build con la entrega anterior del mismo perfil.
- Usar Build Report o equivalente para explicar crecimientos relevantes.

## Evidencia mínima

- Dispositivo y versión del sistema.
- Resolución y orientación.
- Perfil de build y arquitectura.
- Resultado de instalación nueva o actualización.
- Rendimiento medido en escenario identificado.
- Estado del guardado y respaldos.
- Hash y tamaño de la entrega.

Los valores concretos no se guardan en este módulo general; pertenecen al contrato Android
específico de cada proyecto.
