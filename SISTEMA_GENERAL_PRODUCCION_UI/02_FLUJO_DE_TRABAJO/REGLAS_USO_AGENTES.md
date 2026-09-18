# Reglas para usar agentes de forma eficiente

Alcance: sistema general reutilizable para cualquier videojuego o proyecto asistido por
varios agentes que comparten archivos o resultados. No depende de Quantum Forge, Unity ni
de un estilo concreto. El objetivo es obtener más revisión y paralelismo sin perder
autoridad, causalidad ni seguridad.

## Principio central

Un coordinador conserva el contexto completo, decide el plan, integra los resultados,
ejecuta la verificación final y responde por el cierre. Los demás agentes investigan o
revisan subtareas independientes. Cada archivo, sistema o propiedad tiene un solo escritor
durante un bloque de trabajo.

La regla resumida es:

> Un responsable escribe; varios especialistas pueden inspeccionar y cuestionar.

## Cuándo usar agentes

Usar el menor número de agentes que produzca una ventaja real cuando ocurra al menos una de
estas condiciones:

- Existen dos o más auditorías independientes que pueden avanzar en paralelo.
- El cambio tiene riesgo visual, funcional, de rendimiento, persistencia o compatibilidad.
- Conviene separar revisión de diseño, revisión técnica y QA.
- Hace falta investigar fuentes, postmortems, documentación o alternativas amplias.
- Una pantalla importante necesita una segunda opinión antes de solicitar aprobación.
- El diagnóstico admite hipótesis independientes que luego pueden compararse.

Aplicaciones especialmente útiles:

- auditoría de composición, jerarquía, legibilidad y consistencia visual;
- revisión de transiciones, tiempos, capas, alfa y fotogramas intermedios;
- validación de estados, resoluciones, toque, navegación y regreso;
- búsqueda de propietarios duplicados, rutas rotas y riesgos de reconstrucción;
- investigación externa y extracción de antipatrones;
- revisión final contra contrato, referencia y evidencia vigente.

## Cuándo trabajar con un solo agente

No delegar por rutina cuando:

- la modificación es pequeña, local y verificable en pocos minutos;
- las subtareas dependen estrictamente una de otra;
- coordinar y reconciliar resultados cuesta más que realizar el trabajo;
- varios agentes tendrían que editar los mismos archivos o valores;
- la tarea exige una única sesión de Unity sobre el mismo proyecto;
- todavía falta una decisión del usuario que cambiaría todas las alternativas.

## Propiedad y escritura

- El coordinador declara el propietario de cada archivo o sistema antes de editar.
- Por defecto, los agentes auxiliares trabajan en modo lectura y entregan evidencia,
  hipótesis, riesgos y recomendaciones.
- Dos agentes no modifican simultáneamente el mismo archivo, escena, prefab, asset
  serializado, generador, manifiesto o documentación canónica.
- Si un agente recibe permiso de escritura, su alcance debe ser disjunto, concreto y
  verificable. El coordinador revisa e integra el resultado antes de continuar.
- Las observaciones de un agente no cambian el diseño canónico ni autorizan assets,
  mecánicas, nombres o valores nuevos.
- La escena, prefab y reconstructor continúan teniendo un único responsable coordinado.

## Motores y recursos compartidos

- No ejecutar dos instancias de un editor o motor sobre la misma ruta de proyecto cuando
  puedan escribir estado, aunque pertenezcan a agentes distintos.
- Sólo el coordinador autoriza y realiza escrituras serializadas o ejecuciones automatizadas
  sobre el proyecto activo, salvo asignación explícita y exclusiva.
- Los agentes pueden inspeccionar código, documentación, capturas y logs en paralelo cuando
  no interfieran con una escritura activa.
- Una captura o PASS producido por un agente es evidencia candidata hasta que el coordinador
  confirme método, fecha, resolución, escenario y ausencia de errores.

## Contrato mínimo de una delegación

Toda tarea entregada a un agente debe indicar:

1. Objetivo concreto y pregunta que debe resolver.
2. Archivos, sistema o evidencia permitidos.
3. Si trabaja sólo en lectura o tiene escritura exclusiva.
4. Restricciones creativas y técnicas que debe respetar.
5. Resultado esperado: hallazgos, rutas, pruebas, comparación o parche aislado.
6. Criterio de cierre y asuntos que debe devolver como incertidumbre.

No delegar instrucciones vagas como “mejora la pantalla” o “arregla lo que veas”.

## Integración obligatoria

El coordinador debe:

1. Comparar los hallazgos con los archivos reales.
2. Separar hechos, inferencias, preferencias y decisiones pendientes.
3. Resolver contradicciones entre agentes mediante evidencia reproducible.
4. Elegir una única solución coherente con las autoridades del proyecto.
5. Implementar o integrar sin mezclar cambios ajenos al bloque.
6. Ejecutar las pruebas finales aplicables.
7. Informar qué aportaron los agentes sin presentar sus opiniones como aprobación del usuario.

## Patrón recomendado para una pantalla importante

- Coordinador: contexto, implementación, integración y cierre.
- Revisor visual: proporción, encuadre, jerarquía, estados y semejanza.
- Revisor técnico: propietarios, layout, reconstrucción, transición y rendimiento.
- Revisor QA: recorrido real, resoluciones, casos extremos, logs y evidencia.

No es obligatorio usar los tres revisores. Se eligen sólo los que cubren riesgos reales.

## Criterio de terminado

El uso de agentes termina cuando sus preguntas están resueltas, el coordinador verificó los
hallazgos relevantes y no queda una auditoría necesaria en ejecución. Que varios agentes
coincidan no sustituye una prueba nueva ni la aprobación visual o creativa del usuario.
