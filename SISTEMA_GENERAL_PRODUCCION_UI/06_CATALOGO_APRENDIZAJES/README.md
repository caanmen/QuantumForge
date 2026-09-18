# Catálogo de aprendizajes

El catálogo impide que el manual crezca como una bitácora sin depurar. Cada hallazgo entra
primero como caso del proyecto y sólo después se convierte en regla general si es repetible.

## Cuándo se consulta

El catálogo **no se lee completo en cada proyecto ni en cada chat**. El enrutador lo busca
por palabras clave cuando la tarea, el síntoma, la tecnología o el riesgo puedan coincidir
con un aprendizaje anterior.

- Al iniciar un proyecto: sólo para riesgos y tecnologías seleccionados.
- Antes de una corrección: buscar por síntoma, sistema y propietario probable.
- Antes de un lote o fase de pulido: consultar los antipatrones de esa categoría.
- Después de resolver un problema nuevo: registrar primero el caso específico.
- Cuando un fallo se repite: promover una prevención ejecutable o un validador bloqueante.

El usuario puede entregar una lista numerada normal. El asistente realiza la búsqueda,
clasificación y posible promoción; no obliga al usuario a etiquetar cada hallazgo ni llenar
el catálogo.

## Criterios de promoción

Un aprendizaje se promueve cuando:

- Tiene causa identificada, no sólo síntoma.
- Puede redactarse sin depender del contenido o estética del proyecto de origen.
- Indica alcance y condiciones de aplicación.
- Incluye prevención y prueba reproducible.
- No duplica una regla existente.

## Estados

- PROPUESTO: caso observado, todavía sin generalizar.
- VALIDADO: reproducido o respaldado por evidencia suficiente.
- CONSOLIDADO: integrado en una guía temática.
- SUSTITUIDO: reemplazado por una regla más precisa.
- NO_GENERALIZABLE: decisión o incidente exclusivo del proyecto.

## Mantenimiento

1. Buscar por tema e ID antes de crear una fila.
2. Ampliar una regla existente cuando la causa sea la misma.
3. Mantener la explicación extensa en el caso histórico.
4. Conservar en la guía sólo la forma accionable.
5. Revisar periódicamente duplicados, excepciones y vigencia técnica.
6. No cargar al contexto reglas no relacionadas sólo porque existen en el catálogo.

El archivo CATALOGO_APRENDIZAJES.csv contiene el esquema y una base inicial de principios
extraídos sin valores, nombres o estética de un juego concreto.
