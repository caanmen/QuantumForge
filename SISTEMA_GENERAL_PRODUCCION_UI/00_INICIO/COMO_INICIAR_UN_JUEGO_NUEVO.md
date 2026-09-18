# Cómo iniciar un juego nuevo con este sistema

Este documento sirve como punto de entrada para una conversación nueva. Permite adoptar el
método general sin heredar contenido, estética, valores ni decisiones de otros juegos.

## Lectura mínima obligatoria

1. `README.md` en la raíz de `SISTEMA_GENERAL_PRODUCCION_UI`.
2. `00_INICIO/INSTRUCCIONES_MAESTRAS_UI_GENERAL.md`.
3. `00_INICIO/MAPA_DE_USO.md`.
4. `02_FLUJO_DE_TRABAJO/GUIA_REDUCIR_ITERACIONES.md`.
5. `06_CATALOGO_APRENDIZAJES/ANTIPATRONES_Y_ERRORES.md`.
6. Los módulos de `03_GUIAS_TECNICAS` compatibles con el motor y plataforma elegidos.

Si el juego utiliza colecciones de elementos repetidos o temas visuales, leer también
`03_GUIAS_TECNICAS/COMUN/GUIA_FAMILIAS_VISUALES_MODULARES.md`.

## Qué debe hacer el chat antes de implementar

1. Inspeccionar los archivos reales del proyecto nuevo.
2. Identificar motor, plataforma, orientación, resoluciones y métodos de entrada.
3. Localizar el diseño canónico y aclarar quién tiene autoridad creativa.
4. Completar `04_PLANTILLAS/FICHA_PROYECTO_UI.txt`.
5. Crear una carpeta exclusiva en `09_PROYECTOS/NOMBRE_DEL_JUEGO`.
6. Elegir únicamente los principios y módulos que sí correspondan.
7. Indicar qué archivos piensa crear o modificar y cómo verificará el resultado.

## Qué no debe copiar

- Assets, capturas, nombres, textos o paletas de otro proyecto.
- Costes, fórmulas, mecánicas, resoluciones o umbrales sin adopción explícita.
- Perfiles visuales y decisiones guardadas dentro de otra carpeta de `09_PROYECTOS`.
- Casos históricos como si fueran autoridad creativa.

Las carpetas `00_INICIO` a `08_INVESTIGACION_Y_FUENTES` forman la biblioteca general.
`09_PROYECTOS` contiene evidencia y decisiones aisladas por juego.

## Instrucción recomendada para copiar en un chat nuevo

> Revisa `C:\Users\nedfla\Quantum Forge\SISTEMA_GENERAL_PRODUCCION_UI`. Empieza por su
> `README.md` y por `00_INICIO/COMO_INICIAR_UN_JUEGO_NUEVO.md`. Quiero aplicar al proyecto
> actual únicamente las reglas generales y los módulos técnicos compatibles. No copies
> contenido, estética, valores ni decisiones de las carpetas de otros juegos dentro de
> `09_PROYECTOS`. Inspecciona primero los archivos reales del proyecto y dime qué documentos
> adoptarías, qué falta definir y qué archivos crearías antes de implementar.

Después de esa revisión, el responsable del proyecto decide qué reglas particulares adopta
o endurece. Una instrucción actual y explícita siempre tiene prioridad.

