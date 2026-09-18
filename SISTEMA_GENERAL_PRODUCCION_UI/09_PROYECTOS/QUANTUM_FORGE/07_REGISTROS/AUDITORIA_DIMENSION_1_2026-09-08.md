# Auditoría de Dimensión 1 — 8 de septiembre de 2026

Se inspeccionaron la documentación de producción, los controladores, constructores y Main.unity, y se recorrieron las catorce pantallas en Unity 6000.3.10f1. Se conservaron los cambios previos del usuario. No se usó Git para escribir ni se añadieron animaciones.

## Cambios anteriores que sí estaban implementados

- Centro: cristal detallado, pedestal, emblema y seis iconos.
- Carta: composición orbital V12 y cambios posteriores registrados.
- Explorar: destinos exclusivos y arte sectorial dinámico.
- Sectores: eliminación de destinos duplicados conforme a las decisiones del 22 de agosto.
- Reliquias: tres páginas, veinte piezas y efectos existentes.
- Hangar: conservar la navegación premium posterior; la captura histórica no autoriza restaurar la navegación antigua.

## Fallos encontrados y corregidos

| Área | Problema demostrado | Corrección |
|---|---|---|
| Navegación | Hangar, Reliquias y Árbol tenían vacía la referencia al Centro | Tres enlaces reparados; el constructor del Centro vuelve a conectar y validar sus nueve consumidores |
| Recursos | El helper compartido ocultaba los Text UGUI del Centro y sólo actualizaba TMP | Reconocimiento y actualización de ambos tipos de texto |
| Centro | Dimensiones interceptaba dos puntos interiores de Hangar | Recomendación, progreso y selector recolocados; navegación compartida conservada |
| Selector de dimensiones | Abrirlo reservaba altura y desplazaba toda la composición | El selector superpuesto no cambia la reserva de contenido |
| Sectores | Carta conservaba botones alcanzables detrás del detalle | Un propietario en PanelUI oculta y bloquea Carta mientras hay detalle abierto; la restaura al volver |
| Explorar | Planeta fuera de su marco | Viewport local del arte, conservando escala y referencia dinámica |
| Hangar / Reliquias | NO DISPONIBLE y BLOQUEADA truncados en costes | Ajuste automático localizado de texto dentro de las cajas existentes |
| Resultado | Insignia 150 píxeles a la izquierda | Caja cuadrada centrada sin editar el sprite |
| Registro | Marco recortado por offsets heredados | Lienzo fijo D1 y posición compartida |
| Registro | Separadores sobresalían del resumen; singular incorrecto | Altura de 92 píxeles y «1 REGISTRO RECIENTE» |
| Explorar | «BLOQUEADO EN ÁRBOL» perdía la última letra | Caja de estado de apoyo ampliada de 220 a 250 píxeles; revalidación específica |

Se sincronizaron los constructores y la escena para los ajustes serializados. Los cambios de comportamiento usan sus controladores existentes.

## Evidencia técnica nueva

- Ruta completa: PASS en 1080×1920 y 720×1280, con salida 0 y cierre de Unity.
- 33 capturas por resolución: catorce pantallas, tres páginas de Reliquias, selección de sectores, selector abierto/cerrado y regresos.
- Seis regresos de Hangar, Reliquias y Árbol hacia Centro y Explorar comprobados mediante pulsaciones físicas.
- Hangar desde Centro: 30/30 puntos de pulsación correctos por resolución, incluyendo las zonas antes interceptadas.
- Centro conserva su posición al abrir/cerrar Dimensiones.
- Carta no queda interactiva ni recibe raycasts detrás de los cuatro detalles de sector.
- Cero scripts ausentes en Main durante ambas rutas.
- Compatibilidad de partida nueva, antigua y JSON: PASS.
- Puntos del Árbol: aislamiento D1, acreditación única y compatibilidad de guardado: PASS.
- Escaneo, selección de destino y nave, misión coordinada, desbloqueo y coste real de Hangar: PASS.
- Veinte reliquias, efectos, selección y mejora: PASS.
- Último ajuste de estado de apoyo: PASS específico en ambas resoluciones, con aserción de texto completo y captura nueva. Estas dos imágenes sustituyen únicamente la candidata de Explorar de la ruta anterior.
- Guardado personal: seis archivos `save.json*`, cero diferencias de nombre, tamaño y SHA-256 entre inicio y cierre. Evidencia: `persistent_files_final.json`.

Capturas y logs: [evidencia permanente](../08_PRUEBAS/AUDITORIA_D1_2026-09-08/). Herramienta reproducible: `Assets/Project/Scripts/Editor/Dimension1Audit20260908.cs`, métodos Run1080 y Run720. Las pruebas preparan datos sólo en memoria y suprimen escrituras de la partida.

## Lectura correcta del informe de botones

Las filas OFFSCREEN corresponden a controles legacy fuera del viewport, no a botones modernos cortados. El informe también enumera controles detrás de modales: INTERCEPTED allí puede ser el bloqueo correcto. Las dos esquinas rectangulares transparentes del Centro Galáctico coinciden con planetas que tienen prioridad; no se confirmó un fallo del centro visible ni se alteró la composición por ese dato aislado.

No debe interpretarse cualquier fila del TSV como fallo ni un clic central exitoso como validación de toda la superficie. Las regresiones concretas del Centro y la oclusión de Carta ahora tienen aserciones que detienen la prueba.

## Límites y aprobación

Las comprobaciones se hicieron en el editor de Unity con resoluciones reales registradas. No sustituyen pruebas de rendimiento ni interacción en un dispositivo Android físico, ni cubren cada combinación posible de progreso.

La revisión visual interna confirmó los arreglos contra las referencias y decisiones vigentes. La similitud perceptual mínima del 95 % sigue pendiente de aprobación explícita del usuario. Hangar conserva su aprobación histórica; las nuevas capturas son evidencia de regresión pendiente de revisión, no una nueva aprobación. No se copió nada a Capturas Aprobadas.

## Ajuste aprobado del selector de dimensiones

El usuario aprobó explícitamente el 8 de septiembre de 2026 la maqueta del control que abre la navegación de dimensiones. Se implementó en Centro de Mando con tamaño 360×68, etiqueta 330×48 a 18 px y huecos de 8 px. Los paneles Recomendación y Progreso se compactaron sin alterar su contenido.

La prueba específica PASS cubre los estados cerrado, abierto y cerrado de nuevo a 1080×1920 y 720×1280. Las quince muestras interiores del selector por resolución respondieron correctamente; el Centro permaneció inmóvil y no aparecieron scripts ausentes.

Después de revisar las capturas reales finales en ambas resoluciones, el usuario aprobó explícitamente Centro de Mando completo. Las vistas neutral y selector abierto quedaron archivadas en `06_CAPTURAS_APROBADAS/DIMENSION_1/CENTRO_DE_MANDO`. Centro de Mando queda cerrado; las demás pantallas conservan su estado individual.

Dimensión 2 y Dimensión 3 conservan su estado anterior; no existe Dimensión 4. Las animaciones continúan fuera de alcance.
