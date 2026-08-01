# Rediseño futuro del Triángulo del juego base

## Estado de esta propuesta

- Documento de continuidad para retomar después de terminar Dimensión 3.
- No implementar todavía.
- No modifica el funcionamiento actual del Triángulo, el guardado ni la escena.
- Es una dirección de diseño acordada, pendiente de definición numérica, revisión técnica e implementación por bloques.

## Contexto

El juego base ya cuenta con una progresión robusta:

1. Producción inicial con el Condensador de Higgs, el Núcleo Tetraquark y el Modulador de Fase.
2. Generación de LE y Trazas, investigaciones y mejoras.
3. Acceso al Cuarto 2 y su sistema de mezclas experimentales.
4. Obtención de resultados y materiales experimentales.
5. Reparación y desarrollo de la Máquina.
6. Prestigio 1 y acceso posterior a las dimensiones.

Por ahora no se considera necesario ampliar todo el juego base. El punto que sí se quiere mejorar después de Dimensión 3 es el Triángulo, porque actualmente aparenta tener más combinaciones de las que realmente posee.

## Problema del sistema actual

El Triángulo permite colocar tres artefactos en tres posiciones, pero el protocolo activo depende principalmente de la posición del Modulador:

- Modulador en Principal: Impulso.
- Modulador en Refuerzo: Sinergia.
- Modulador en Alteración: Persistencia.

Intercambiar Higgs y Tetra entre los otros dos espacios no crea un resultado diferente. Visualmente existen seis permutaciones, pero mecánicamente solo tres resultados.

También existe un segundo selector independiente en el propio Modulador —Expansión, Conservación y Sintonía—, lo que duplica conceptos y hace menos clara la relación entre los tres artefactos.

## Condición fundamental: Quantum Forge es un juego idle

El jugador puede permanecer entre treinta minutos y varias horas, cerrar el juego y regresar después. Inicialmente, el progreso offline tendrá un límite aproximado de doce horas.

Por ello, el Triángulo debe funcionar como un director de producción autónoma, no como un minijuego de atención constante.

Reglas de diseño acordadas:

- Todos sus estados deben continuar funcionando offline.
- No debe exigir pulsaciones repetidas ni recogidas manuales frecuentes.
- No debe obligar al jugador a recordar una configuración especial antes de cerrar.
- Cualquier ciclo, pulso o evento debe resolverse automáticamente durante la ausencia.
- La configuración seleccionada debe persistir entre sesiones.
- Al regresar debe mostrarse claramente qué produjo el Triángulo.
- El límite offline de doce horas es una regla general del juego, no un beneficio exclusivo de una configuración.

## Riesgo detectado: elecciones falsas

No conviene crear varias configuraciones cuya única diferencia sea la manera de producir LE.

Si una configuración de producción directa y otra de almacenamiento entregan finalmente el mismo recurso, el jugador comparará sus resultados después de doce horas y mantendrá siempre la opción matemáticamente superior. En ese caso no existe una decisión real: existe una configuración correcta y las demás son trampas.

Tampoco se recomienda presentar seis protocolos con nombres, porcentajes y condiciones diferentes. En un idle, seis explicaciones extensas pueden crear fatiga y provocar que el jugador ignore por completo el sistema.

La profundidad debe proceder de las consecuencias de unas pocas decisiones claras, no de la cantidad de opciones visibles.

## Dirección recomendada: tres circuitos

Los tres artefactos permanecerían en posiciones fijas y coherentes:

- Condensador de Higgs: energía y potencia disponible.
- Núcleo Tetraquark: estabilidad, Trazas y materia residual.
- Modulador de Fase: ritmo, automatización y control temporal.

En lugar de reorganizar seis permutaciones, el jugador activaría uno de los tres lados del Triángulo. Cada lado representa un circuito con un objetivo fácil de reconocer.

### 1. Circuito de Energía

Conexión principal: Higgs y Modulador. Tetra estabiliza el flujo.

Objetivo visible: producir más LE.

Dirección mecánica:

- Aumenta la producción de LE.
- Puede acelerar los ticks energéticos.
- Reduce ligeramente el rendimiento de Trazas para que exista una especialización real.
- Continúa produciendo durante las doce horas offline.

Debe ser la elección evidentemente correcta cuando el jugador necesita LE; no debe competir con otras maneras disfrazadas de producir el mismo recurso.

### 2. Circuito Experimental

Conexión principal: Higgs y Tetra. El Modulador controla la extracción.

Objetivo visible: producir Trazas y materiales experimentales.

Dirección mecánica:

- Reduce parcialmente la producción de LE.
- Aumenta la producción de Trazas.
- Después de abrir el Cuarto 2, mejora la generación de fragmentos.
- En etapas posteriores puede influir en la estabilidad, calidad o resultados de las mezclas.
- Todos sus resultados se calculan también offline.

Este circuito debe evolucionar con el juego: antes del Cuarto 2 se centra en Trazas y después obtiene funciones experimentales adicionales.

### 3. Circuito de Fase

Conexión principal: Tetra y Modulador. Higgs alimenta el proceso.

Objetivo visible: acelerar procesos automáticos.

Dirección mecánica:

- Reduce ligeramente la producción directa.
- Acelera ticks y ciclos compatibles.
- Después del Cuarto 2 puede acelerar generación de fragmentos u otros procesos temporizados.
- Después de acceder a la Máquina puede acelerar procesos internos compatibles.
- La aceleración se aplica igualmente a la simulación offline.

Este circuito no debe ser solamente otro multiplicador de LE. Su valor debe proceder de reducir tiempos y completar más ciclos.

## Participación de los tres artefactos

Aunque se active un lado concreto, los tres artefactos deben intervenir siempre:

- Higgs determina la energía disponible para el circuito.
- Tetra determina estabilidad, eficiencia residual y producción secundaria.
- Modulador determina velocidad, sincronización y comportamiento autónomo.

El lado seleccionado define la prioridad, pero el tercer vértice continúa aportando una función calculable y visible. De esta manera el resultado ya no depende únicamente del Modulador.

## Sincronización

La calibración actual del Modulador puede reutilizarse como Sincronización del Circuito.

Propuesta inicial:

- La primera activación comienza en 0%; los cambios posteriores comienzan en 50%.
- Recupera el 100% en pocos minutos.
- Continúa aumentando offline.
- No se pierde al cerrar el juego.
- Su finalidad es evitar cambios abusivos instantáneos, no castigar la experimentación.

No deben existir descuentos de compra que puedan explotarse cambiando de circuito justo antes de pulsar Comprar. Los efectos deben depender principalmente del tiempo durante el cual el circuito estuvo trabajando.

## Presentación y comprensión

La interfaz principal debe usar tres objetivos, tres iconos y textos muy cortos:

- Energía: más LE.
- Experimental: más Trazas y fragmentos.
- Fase: procesos más rápidos.

El panel central debería mostrar como máximo:

- Circuito activo.
- Beneficio principal.
- Sacrificio principal.
- Sincronización.
- Producción o progreso estimado por hora.

Ejemplo:

> EXPERIMENTAL  
> Prioriza Trazas y fragmentos  
> +45% Trazas · -20% LE

Los detalles técnicos completos pueden aparecer en un botón de información, pero no deben ser necesarios para tomar la decisión básica.

Dirección visual:

- El lado activo se ilumina.
- Una animación muestra el flujo entre los dos vértices principales.
- El tercer artefacto presenta una animación secundaria que comunica su función de soporte.
- El centro cambia de icono y color según el circuito.
- Deben mostrarse previsiones comprensibles por hora y para el periodo offline.

## Progreso offline

Todos los circuitos funcionan durante el límite general de ausencia:

| Circuito | Resultado principal durante la ausencia |
| --- | --- |
| Energía | Acumulación de LE |
| Experimental | Acumulación de Trazas y, cuando corresponda, fragmentos |
| Fase | Mayor cantidad de ciclos y procesos completados |

No debe existir un modo Persistencia obligatorio para obtener progreso offline. Si el juego se cierra inesperadamente, el jugador conserva el comportamiento normal del circuito que ya estaba usando.

Al regresar, el informe offline debería indicar:

- Tiempo simulado.
- Circuito utilizado.
- LE y Trazas obtenidas.
- Fragmentos o resultados experimentales obtenidos.
- Ciclos o procesos completados.
- Si se alcanzó algún límite de almacenamiento.

## Evolución futura sin aumentar la complejidad visible

Los mismos tres circuitos pueden ganar profundidad conforme avanza el juego:

- Las mejoras tempranas aumentan la contribución de cada artefacto.
- El Cuarto 2 añade funciones experimentales al Circuito Experimental.
- La Máquina añade procesos compatibles al Circuito de Fase.
- Las dimensiones pueden aportar módulos o modificadores para los tres circuitos.

El jugador sigue viendo tres decisiones, pero sus consecuencias crecen con la progresión general.

## Elementos actuales que conviene reutilizar

- La representación visual triangular.
- Los tres artefactos existentes.
- El estado de calibración.
- Las mejoras existentes del Triángulo, reinterpretadas si es necesario.
- Los bonus de la Máquina relacionados con el Triángulo.
- Los bonus dimensionales relacionados con el Triángulo.
- El guardado del circuito activo y su sincronización.
- La infraestructura actual de progreso offline, adaptándola a los tres circuitos.

Las mejoras actuales podrían reinterpretarse por función, procurando conservar sus IDs internos para no romper partidas guardadas:

- Impulso Dirigido: potencia energética o de alimentación.
- Enlace Resonante: estabilidad y producción residual.
- Anclaje Persistente: sincronización y funcionamiento autónomo.

## Elementos que probablemente deben retirarse o transformarse

- La dependencia exclusiva de la posición del Modulador para determinar el protocolo.
- Las seis permutaciones visuales que solo producen tres resultados.
- El selector separado de Expansión, Conservación y Sintonía.
- Persistencia como requisito para recibir progreso offline.
- Bonificaciones que solo sean maneras distintas de producir más LE.
- Descuentos explotables mediante cambios instantáneos de configuración.
- Acciones manuales repetitivas incompatibles con el carácter idle.

Sintonía puede reservarse para una evolución posterior al Prestigio 1, por ejemplo como control avanzado de intensidad, pero no debe mantenerse como una opción sin efecto.

## Orden recomendado cuando se retome

1. Auditar por completo el cálculo offline y las fórmulas reales de producción.
2. Definir los tres circuitos con efectos cualitativos, todavía sin balance final.
3. Decidir qué sistemas temporizados pueden ser acelerados de forma segura por el Circuito de Fase.
4. Diseñar la migración de partidas guardadas y de las configuraciones actuales.
5. Reinterpretar las tres mejoras existentes sin cambiar sus IDs si es posible.
6. Implementar la lógica central y pruebas unitarias o de validación.
7. Actualizar la interfaz y la visualización del flujo.
8. Integrar el cálculo offline y crear el informe de regreso.
9. Balancear sesiones de 30 minutos, 2 horas y 12 horas offline.
10. Validar que ningún circuito sea universalmente superior y que cada uno resuelva un cuello de botella diferente.

## Criterios de aceptación del futuro rediseño

- Un jugador puede explicar las tres opciones como LE, experimentos y velocidad sin leer documentación extensa.
- Los tres artefactos participan mecánicamente en todos los circuitos.
- Ninguna opción es simplemente una versión peor de otra que produce el mismo recurso.
- Los tres circuitos funcionan online y offline.
- Cerrar inesperadamente el juego no causa una penalización por no haber elegido un modo especial.
- La decisión correcta depende del cuello de botella actual, no de una fórmula universal.
- El sistema gana funciones con Cuarto 2, Máquina y dimensiones sin añadir nuevas opciones principales.
- Los datos mostrados por la interfaz coinciden con la producción real y la simulación offline.

## Decisión actual

El rediseño fue retomado después de completar la mayor parte funcional de Dimensión 3. El usuario aprobó iniciar su implementación manteniendo el Triángulo como identidad visual y sustituyendo las ranuras móviles por tres circuitos de vértices fijos.

## Decisiones aprobadas de progresión y especialización (2026-07-28)

- La progresión obligatoria es Higgs → Tetraquark → Modulador → Acople → elección de circuito → especialización.
- Acople requiere los tres artefactos y no selecciona un circuito automáticamente. La primera elección parte de 0%; los cambios posteriores parten de 50%, 65% o 80% según Memoria de Sincronía.
- El catálogo público F2 queda en siete mejoras: Emisión Calibrada, Ciclo de Contención, Lectura Tetraquark, Acople de Vértices, Amplificador de Energía, Resonancia Experimental y Memoria de Sincronía.
- `residual_analysis` se retira visualmente y migra a `tetraquark_stabilization`; `pattern_mapping` se retira y migra a `emission_focus`.
- `triangle_persistence_anchor` conserva su ID, pero sustituye definitivamente el concepto de reserva offline: ahora es Memoria de Sincronía y solo determina el porcentaje inicial de cambios entre circuitos ya activados.
- El tema visual de Dimensión 1 pasa a una paleta oscura de alto contraste sin alterar layout, jerarquía, navegación ni geometría.

## Decisiones aprobadas para la primera implementación

- El Triángulo conserva los tres artefactos y su representación triangular.
- Higgs, Tetraquark y Modulador permanecen en vértices fijos; el jugador selecciona uno de los tres lados.
- Energía gobierna el avance de LE del juego base.
- Experimental gobierna Trazas y, después de abrir el Cuarto 2, fragmentos experimentales.
- Fase gobierna procesos temporizados compatibles de la Máquina y la respuesta de rutinas autorizadas de Dimensión 3.
- Fase no acelera los ticks normales de LE o Trazas, las fusiones instantáneas del Cuarto 2 ni los temporizadores de Dimensión 1 o Dimensión 2.
- Fase se muestra bloqueado hasta que la Máquina esté desbloqueada.
- Todos los circuitos conservan su comportamiento durante un máximo general de doce horas offline.
- La penalización de un circuito se aplica inmediatamente; su beneficio positivo escala con la sincronización.
- La primera activación comienza en 0% y tarda 90 segundos en llegar al 100%.
- Los cambios posteriores comienzan en 50% y también tardan 90 segundos en llegar al 100%, incluso offline.

Valores iniciales aprobados:

| Circuito | Beneficio | Sacrificio |
| --- | --- | --- |
| Energía | +12% LE | -10% Trazas |
| Experimental | +10% Trazas y +6% fragmentos | -10% LE |
| Fase | +15% velocidad de análisis de la Máquina y +10% velocidad de respuesta de rutinas N3 | -10% LE y Trazas |

Compatibilidad aprobada:

- Se conservan los IDs `triangle_impulse_tuning`, `triangle_synergy_resonance` y `triangle_persistence_anchor`.
- Impulso antiguo migra a Amplificador de Energía, Sinergia a Resonancia Experimental
  y `triangle_persistence_anchor` a Memoria de Sincronía.
- La configuración triangular antigua tiene prioridad sobre el modo separado del Modulador al migrar una partida.
- La Consola N3 pasará a mantener un circuito preferido en lugar de controlar por separado el modo del Modulador y una permutación triangular.
- El regreso offline mostrará una ventana sencilla con tiempo aplicado, circuito utilizado y resultados obtenidos.
