# Scorpion: el marco respeta la geometría universal

Fecha: 2026-09-12  
Estado: consolidado

## Problema observado

Aunque el patrón A–10 tenía conteos y topología correctos, la especificación permitía que
cada marco suministrara centro y escala. En Scorpion esto dejó símbolos centrales casi del
mismo tamaño que los palos de los índices y obligó a ajustar el contenido después de haber
diseñado el marco.

## Regla consolidada

El maestro A–10 es una interfaz geométrica inmutable. Incluye posiciones, orientación,
centros de índices, tamaños visibles y zonas reservadas. El marco se diseña después y debe
contener esas zonas. Si no puede hacerlo, se modifica el marco; nunca se traslada ni se
escala la geometría universal.

El estilo tipográfico puede pertenecer al tema solamente cuando conserva la misma huella
visible bloqueada. Ningún marco puede validarse sin estado `LOCKED` y hash aprobado del
maestro. Modificar cualquier campo geométrico invalida la aprobación.

## Defensa automática

`Tools/validate_universal_frame_contract.py --self-test` demuestra una fixture válida y
obliga a fallar estas variantes: patrón trasladado, patrón escalado, centro de índice
movido, símbolos reducidos, zona reservada estrecha, arte intruso, maestro pendiente y
geometría modificada sin nueva aprobación.

El contrato canónico vive en
`04_PLANTILLAS/CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json`.
