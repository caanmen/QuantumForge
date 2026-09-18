# Sistema general de producción de proyectos, juegos y UI

Este es el único sistema documental común del estudio. Su núcleo permite seleccionar,
planear, implementar, validar y documentar distintos tipos de proyecto. La especialización
más desarrollada actualmente es videojuegos, Unity y UI. Cuando otro tipo todavía no tenga
un módulo detallado, se usa el núcleo común y se declara la carencia sin inventar cobertura.

Las decisiones y evidencias de cada proyecto permanecen aisladas dentro de `09_PROYECTOS`
y nunca se convierten automáticamente en reglas generales.

## Entrada obligatoria

La petición «revisa la carpeta de producción» no ordena leer todo. Activa esta secuencia:

1. `00_INICIO/INVENTARIO_DE_CARPETAS.md` para saber qué existe.
2. `00_INICIO/ENRUTADOR_DE_PROYECTOS_Y_TAREAS.md` para seleccionar la ruta.
3. `00_INICIO/MAPA_DE_USO.md` para ejecutar sólo el flujo aplicable.

El usuario describe el proyecto y el objetivo con lenguaje natural. El asistente elige los
documentos, completa las plantillas necesarias y comunica brevemente su selección.

## Qué se puede reutilizar

- Principios de autoridad, trazabilidad, fidelidad y propietarios únicos.
- Flujo de trabajo adaptable al riesgo principal de cada pantalla.
- Plantillas de proyecto, familia visual, pantalla y aprobación.
- Pruebas de estructura, estados, interacción, navegación, accesibilidad y rendimiento.
- Módulos técnicos opcionales para Unity UGUI y móvil Android.
- Método universal para auditar, modificar, probar y evolucionar código heredado sin
  exigir que el usuario conozca la implementación.
- Catálogo normalizado de aprendizajes y casos históricos resumidos.
- Investigación verificada, fuentes y advertencias para evitar repetir errores.
- Método de producción orientado a riesgos, rebanadas verticales y reducción de retrabajo.
- Reglas de colaboración con un coordinador, especialistas y un único escritor por sistema.
- Método para producir familias visuales modulares con geometría, zonas reservadas y
  contratos de capas consistentes.

## Qué debe definir cada proyecto

- Diseño canónico, contenido y mecánicas.
- Motor, plataforma, orientación y resoluciones objetivo.
- Familias visuales y reglas de estilo.
- Umbral y método de fidelidad visual.
- Assets canónicos, referencias y capturas aprobadas.
- Herramientas, rutas, perfiles de build, firma y persistencia.

## Organización

- `00_INICIO`: autoridad, inventario, enrutador, continuidad y mapa de uso.
- `01_PRINCIPIOS`: reglas transferibles.
- `02_FLUJO_DE_TRABAJO`: procesos de producción y reducción de iteraciones.
- `03_GUIAS_TECNICAS`: módulos por código, tecnología o plataforma.
- `04_PLANTILLAS`: contratos y fichas reutilizables.
- `05_QA`: planes y puertas de calidad.
- `06_CATALOGO_APRENDIZAJES`: reglas consolidadas y antipatrones.
- `07_CASOS_HISTORICOS`: resúmenes transferibles de problemas reales.
- `08_INVESTIGACION_Y_FUENTES`: fuentes externas verificadas y fecha de revisión.
- `09_PROYECTOS`: reglas, mapas técnicos, assets, capturas y decisiones propias de cada juego.
- `10_SKILL`: puerta de aceptación reutilizable, su criterio canónico y validador.

La responsabilidad de cada capa y el orden de autoridad están definidos en
`00_INICIO/FUENTES_DE_INSTRUCCIONES.md`. No se debe copiar un procedimiento completo en
varios archivos: se conserva una fuente canónica y las demás capas sólo la enlazan.

## Regla de adopción

Cada proyecto debe aplicar el núcleo de `05_QA/PUERTA_OBLIGATORIA_ACEPTACION.md`, completar
la ficha que corresponda, elegir los módulos técnicos compatibles y registrar cualquier
regla más estricta en su propio sistema documental. Las comprobaciones especializadas no se
suponen aplicables: se evalúan y toda exclusión se justifica.

Un proyecto puede exigir composición estática antes de toda interacción, un porcentaje
concreto de fidelidad o resoluciones fijas. Esas políticas prevalecen dentro de ese
proyecto, pero no se convierten automáticamente en universales.

## Inicio rápido

1. Leer inventario, enrutador y mapa.
2. Clasificar el tipo de proyecto, la tarea y el riesgo principal.
3. Inspeccionar el proyecto real y sus instrucciones específicas.
4. Seleccionar únicamente principios, guías, aprendizajes y pruebas aplicables.
5. Crear el registro y pasar `preflight` cuando el entregable sea sustancial.
6. Para videojuegos o UI nuevos, continuar con `00_INICIO/COMO_INICIAR_UN_JUEGO_NUEVO.md`
   e `00_INICIO/INSTRUCCIONES_MAESTRAS_UI_GENERAL.md`.

## Mantenimiento de la skill

La fuente canónica de `puerta-aceptacion` vive en `10_SKILL/puerta-aceptacion`. La copia de
`C:\Users\nedfla\.codex\skills\puerta-aceptacion` es un despliegue y no se edita a mano.
Después de cambiar la fuente, ejecutar:

```powershell
python .\10_SKILL\puerta-aceptacion\scripts\sync_skill.py --destination "C:\Users\nedfla\.codex\skills\puerta-aceptacion"
```

Usar `--check` para detectar divergencias sin escribir.

## Relación con los proyectos

Cada proyecto registrado tiene una sola carpeta dentro de `09_PROYECTOS`. Para iniciar uno
nuevo se reutiliza el núcleo aplicable y se crea su carpeta; no se copian perfiles, valores,
assets ni decisiones de otro proyecto.
