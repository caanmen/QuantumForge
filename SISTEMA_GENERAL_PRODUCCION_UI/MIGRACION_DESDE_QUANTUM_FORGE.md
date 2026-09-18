# Estructura unificada adoptada desde Quantum Forge

Esta guía explica cómo conviven el núcleo reutilizable y los proyectos sin mantener dos
sistemas raíz que puedan confundirse.

## Una sola raíz

`SISTEMA_GENERAL_PRODUCCION_UI` es la única raíz. Quantum Forge conserva su autoridad
documental específica dentro de `09_PROYECTOS/QUANTUM_FORGE`: perfiles por dimensión,
catálogo real, referencias, capturas, registros, pruebas, pantallas y herramientas.

## Qué se extrajo conceptualmente

| En el sistema general | Permanece específico del proyecto |
|---|---|
| Principios sin estética | Perfiles de Dimensión 1, 2, 3 y 4 |
| Flujo adaptable por riesgo | Orden obligatorio decidido para Quantum Forge |
| Plantillas parametrizadas | Fichas y decisiones de pantallas reales |
| QA por capacidades | Resoluciones, Android y umbral concretos del juego |
| Módulos Unity y Android opcionales | Menús, scripts, paquete, firma y builds reales |
| Casos resumidos | Evidencia completa, imágenes, logs y capturas |

## Regla de autoridad

1. Leer primero las reglas persistentes del proyecto.
2. Usar el núcleo general como método base.
3. Aplicar después el perfil y contrato específicos de Quantum Forge.
4. Si el proyecto exige una regla más estricta, esa regla prevalece.
5. No editar ni sustituir evidencia específica al actualizar el núcleo general.
6. Una fuente externa informa una decisión, pero no sustituye la inspección del proyecto.

## Portabilidad

Para otro juego se reutiliza el núcleo `00_INICIO` a `08_INVESTIGACION_Y_FUENTES` y se crea
`09_PROYECTOS/NOMBRE_DEL_JUEGO`. La carpeta de Quantum Forge no se usa como plantilla y no
se copian sus imágenes, estilos, porcentajes, resoluciones ni mecánicas.
