# Instrucciones maestras UI generales

## Propósito

Producir interfaces fieles, utilizables, estables y reproducibles sin depender de la
memoria de una conversación ni de reglas implícitas de un proyecto anterior.

## Clasificación obligatoria

Toda afirmación documental debe pertenecer a una categoría:

- PRINCIPIO GENERAL: transferible entre proyectos, sin imponer motor o estilo.
- MÓDULO TÉCNICO: aplicable sólo a un motor, framework, plataforma o dispositivo.
- POLÍTICA DE PROYECTO: exigencia elegida para un juego concreto.
- PERFIL VISUAL: lenguaje autorizado para una familia, facción, zona, modo o capítulo.
- DECISIÓN DE PANTALLA: contenido o excepción de una pantalla específica.
- CASO HISTÓRICO: evidencia de origen; enseña, pero no se hereda automáticamente.

Si una regla no indica su alcance, todavía no está lista para reutilizarse.

## Autoridad

1. Petición actual y explícita del responsable del proyecto.
2. Diseño canónico y decisiones aprobadas del proyecto actual.
3. Reglas persistentes del proyecto actual.
4. Perfil visual y contrato de la pantalla actual.
5. Principios generales y módulos técnicos adoptados.
6. Casos históricos, únicamente como evidencia o diagnóstico.

El estado técnico real debe inspeccionarse siempre. Cuando contradiga la documentación,
se registra la divergencia y se resuelve antes de realizar una decisión difícil de
revertir. Un caso histórico nunca tiene autoridad creativa sobre otro juego.

## Puerta de adopción del sistema

Antes de usar este método en un proyecto:

1. Identificar motor, sistema UI, plataformas y métodos de entrada.
2. Definir orientación, relaciones de aspecto y dispositivos objetivo.
3. Definir autoridad creativa y fuentes canónicas.
4. Elegir un umbral y método de aprobación visual apropiados.
5. Seleccionar sólo los módulos técnicos aplicables.
6. Definir dónde vivirán referencias, assets, capturas, decisiones y pruebas.
7. Registrar qué políticas particulares endurecen o sustituyen el método general.

## Reglas no negociables del núcleo

- No inventar contenido, mecánicas, nombres, estados o estética.
- Distinguir referencia, concepto, asset final y captura real del motor.
- Buscar y reutilizar recursos canónicos antes de producir sustitutos.
- Cada dato y propiedad dinámica debe tener un propietario identificable.
- Los estados relevantes se diseñan y prueban, no se deducen sólo del estado feliz.
- La evidencia debe ser posterior al último cambio que pueda alterar el resultado.
- Una prueba técnica correcta no equivale por sí sola a aprobación visual o jugable.
- Las limitaciones, sustituciones y simplificaciones se registran y aprueban.
- Los aprendizajes transferibles se separan de las decisiones del caso que los originó.

## Elección del flujo según el riesgo

### Ruta A — Fidelidad dirigida por referencia

Usar cuando existe una composición visual aprobada y el principal riesgo es perder su
proporción, identidad o acabado. Aprobar primero la composición estática; después conectar
datos, interacción y animación.

### Ruta B — Interacción desconocida

Usar cuando el principal riesgo es que el control, flujo o modelo mental no funcione. Se
permite un prototipo funcional deliberadamente no final para validar interacción. Después
se crea y aprueba la composición estática final antes del pulido, arte final y animación.

### Ruta C — Sistema existente que recibirá un skin nuevo

Inventariar primero toda la capacidad del controlador actual. El skin opera la misma
autoridad funcional mediante adaptadores; no duplica reglas ni elimina ramas silenciosamente.

El proyecto puede declarar una ruta obligatoria más estricta.

## Colaboración con agentes

Cuando varios agentes puedan aportar una ventaja real, aplicar
`02_FLUJO_DE_TRABAJO/REGLAS_USO_AGENTES.md`. Un coordinador conserva autoridad sobre el
plan, la integración y la validación final. Cada archivo, sistema o propiedad tiene un solo
escritor; los agentes auxiliares trabajan en lectura salvo asignación exclusiva y disjunta.
No se usan agentes por rutina en cambios pequeños ni se ejecutan instancias concurrentes de
Unity sobre la misma ruta.

## Cierre

Una pantalla sólo se considera terminada cuando cumple el contrato del proyecto, presenta
datos y estados reales, conserva navegación y persistencia, supera las pruebas aplicables y
dispone de evidencia vigente. Los porcentajes, resoluciones y dispositivos son parámetros
del proyecto, no constantes del núcleo general.
