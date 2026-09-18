# Guía base: juegos de automatización, fábricas y robots

Consulta inicial: 24 de agosto de 2026.

Esta guía reúne principios reutilizables para proyectos donde el jugador organiza
máquinas, rutas, robots, defensas o flujos de producción. No sustituye el diseño
propio de cada juego: es un punto de partida para decidir qué probar primero.

## 1. La pregunta central del jugador

El juego debe permitir que el jugador responda con sus decisiones a una pregunta
clara: **¿cómo consigo que este flujo funcione mejor?**

La respuesta puede pasar por colocar, mejorar, separar, priorizar o redistribuir,
pero la consecuencia debe verse en el mundo: más producción, menos espera, más
capacidad o un cuello de botella distinto.

## 2. Enseñar mediante consecuencias

- Presentar pocas piezas al inicio y desbloquear complejidad gradualmente.
- Dejar que la primera solución sencilla funcione, aunque no sea óptima.
- Introducir presión después: más flujo, piezas resistentes, capacidad limitada o
  rutas que compiten por una salida.
- Mostrar el estado observable (cola, carga, saturación, piezas/minuto), no una
  orden que resuelva el problema por el jugador.
- Explicar una máquina cuando se selecciona, en una frase factual: qué transforma,
  qué capacidad tiene y qué mejora cambia.

La ayuda explícita se reserva para un bloqueo real: una interacción que el jugador
no puede descubrir o un error que no puede entender desde la pantalla.

## 3. Decisiones que sí crean estrategia

Una nueva herramienta aporta estrategia cuando cambia un intercambio, no solo
cuando sube un número.

Ejemplos:

- **Divisor:** reparte el flujo y evita un cuello, pero obliga a financiar y usar
  dos rutas que pueden quedar desbalanceadas.
- **Rotor o arma rápida:** procesa más, pero puede crear demasiadas piezas para el
  transporte posterior.
- **Rotor o arma potente:** resuelve objetivos resistentes, pero puede ser más lento
  o más caro y conviene colocarlo en el punto correcto.
- **Ruta corta:** llega antes, pero concentra la carga.
- **Ruta larga o alternativa:** ocupa espacio, pero permite separar funciones.

Evitar mejoras que eliminen todas las decisiones. Si una sola máquina resuelve todo
sin coste ni nueva presión, las otras ubicaciones dejan de importar.

## 4. Estructura recomendada para una primera vertical slice

1. Un flujo claro: entrada o aparición de objetivos, transformación y salida.
2. Una capacidad inicial que soporte casi todo el flujo.
3. Una presión gradual que ocasione cola visible, no fracaso instantáneo.
4. Dos respuestas viables y con diferencias reales.
5. Una métrica de resultado fácil de leer: entregas/minuto, créditos/minuto,
   tiempo de supervivencia o eficiencia.
6. Una mejora comprable cuyo efecto se pueda observar inmediatamente.

Antes de añadir más sistemas, probar con personas si entienden la relación entre:
acción del jugador → cambio en el flujo → resultado medible.

## 5. Información en pantalla

Prioridad de lectura:

1. Lo que está pasando en el mundo: piezas, robots, filas, impacto, atascos.
2. Estado breve junto al elemento: capacidad, nivel, resistencia o saturación.
3. Resumen estable: producción, recursos y carga general.
4. Detalle al seleccionar: nivel actual, coste y cambio exacto de la mejora.

No hacer que una frase crítica cambie demasiadas veces por segundo. Si hay varios
estados, separarlos en campos estables (por ejemplo, estado, carga y último evento).

## 6. Rendimiento Unity / Android

- Medir primero en un teléfono Android de prueba; el Editor sirve para iterar, no
  para concluir que el juego funciona fluido en el dispositivo final.
- Capturar una línea base del Profiler antes de optimizar y comparar el mismo
  escenario de estrés después de cada cambio.
- Reutilizar objetos de aparición frecuente (robots, enemigos, proyectiles,
  productos, fragmentos y efectos) con `ObjectPool<T>` o un pool propio.
- Evitar crear y destruir objetos continuamente durante el juego.
- No actualizar toda la interfaz cada fotograma si sus datos cambian pocas veces
  por segundo. Actualizar por evento o con una frecuencia limitada.
- Separar el Canvas estático de los contadores y mensajes dinámicos. Unity indica
  que un cambio puede obligar a reconstruir el Canvas completo.
- Reducir Raycast Targets y Graphic Raycasters donde no haya interacción.

Orden de diagnóstico práctico:

1. Reproducir el tirón en un teléfono.
2. Marcar si coincide con aparición, destrucción, corte, disparo, oleada o UI.
3. Revisar CPU, GC Alloc, Rendering y memoria en el Profiler.
4. Corregir la causa dominante.
5. Medir de nuevo antes de hacer otra optimización.

## 7. Checklist antes de añadir una mecánica

- ¿Qué nueva decisión obliga a tomar?
- ¿Qué problema observable resuelve?
- ¿Qué coste, límite o nuevo problema introduce?
- ¿Cómo se verá su efecto sin explicarlo con un tutorial largo?
- ¿Qué métrica cambiará?
- ¿Puede probarse en una escena pequeña antes de integrarse al juego completo?
- ¿Qué coste tiene en Android si aparecen muchas instancias?

## Fuentes de referencia

- Unity, ObjectPool<T> (Unity 6):
  https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Pool.ObjectPool_1.html
- Unity, UI optimization tips:
  https://unity.com/how-to/unity-ui-optimization-tips
- Unity, profiling on the target platform:
  https://docs.unity3d.com/2022.2/Documentation/Manual/profiler-profiling-applications.html
- Factorio Friday Facts #284, aprendizaje orgánico y presión de producción:
  https://www.factorio.com/blog/post/fff-284
- Factorio Friday Facts #261, feedback de interacción y rendimiento:
  https://www.factorio.com/blog/post/fff-261
- Factorio Friday Facts #421, medir y optimizar sistemas que crecen en cantidad:
  https://www.factorio.com/blog/post/fff-421

