# Mapa de código y validación de Quantum Forge

Fecha de línea base: 2026-09-18  
Alcance: enrutamiento técnico, riesgos y evolución segura; no sustituye el código real.

## Cómo usar este mapa

Consultar este archivo cuando una tarea cree, corrija, integre o refactorice código de
Quantum Forge. Leer además la guía universal
`03_GUIAS_TECNICAS/CODIGO/GUIA_DESARROLLO_Y_CALIDAD_CODIGO.md` y sólo los documentos del
sistema afectado.

El usuario no necesita identificar archivos ni llenar formatos. El asistente inspecciona
las fuentes actuales, completa la matriz de impacto y comunica únicamente decisiones,
riesgos y pruebas relevantes.

## Línea base reproducible

El inventario archivado en `Logs/GeneralUiDocs/code_audit_inventory_2026-09-18.txt`
registró:

- 448 scripts C# propios y 192285 líneas.
- 87 archivos con más de 500 líneas y 33 con más de 1000.
- 109 archivos `*Validation.cs`.
- Unity Test Framework instalado, sin archivos `.asmdef` ni atributos `[Test]`,
  `[UnityTest]` o `[TestCase]` encontrados bajo `Assets/Project/Scripts`.
- 10 scripts con marcadores de posible codificación dañada; tres pertenecen al runtime.

Estas cifras son una fotografía fechada, no contratos permanentes. Volver a ejecutar el
inventario antes de usarlas para una decisión futura.

## Propietarios técnicos actuales

| Área | Fuentes principales | Riesgo que obliga a ampliar la inspección |
|---|---|---|
| Estado global y producción base | `Assets/Project/Scripts/Core/GameState.cs` | Nuevos campos, recursos, resets, offline o lógica añadida al estado central |
| Guardado, carga y recuperación | `Assets/Project/Scripts/Systems/SaveService.cs` | Schema, migraciones, pausa, reanudación, históricos o bloqueo de escrituras |
| Dimensión 1 | `Assets/Project/Scripts/Systems/Dimension1System.cs` y estado D1 en `GameState.cs` | Economía, exploración, naves, sectores, árbol, reliquias u offline |
| Dimensión 2 | `Assets/Project/Scripts/Systems/Dimension2*.cs` y `D2*.cs` | Civilizaciones, pactos, progreso offline o textos de dominio |
| Dimensión 3 | `Assets/Project/Scripts/Systems/Dimension3*.cs` y `D3*.cs` | Automatización, colas, instalaciones, diagnósticos u offline |
| Convergencia | `Assets/Project/Scripts/Systems/Convergence*.cs` | Transacciones, recuperación, sincronización, telemetría o resets |
| Edificios | `Assets/Project/Scripts/Buildings` | Registro, niveles, costes, producción y reconstrucción tras carga |
| Presentación | `Assets/Project/Scripts/Presentation` | Informes, rutas y textos derivados de varios sistemas |
| Interfaz runtime | `Assets/Project/Scripts/UI` | Navegación, asignaciones por fotograma, localización y estados visibles |
| QA runtime | `Assets/Project/Scripts/QA` | Checkpoints, aceleración, preparación y restauración de estado |
| Setup, captura y validación | `Assets/Project/Scripts/Editor` | Escenas, datos ficticios, archivos temporales, entrada batch y limpieza |

Esta tabla orienta la búsqueda. Antes de modificar hay que verificar el propietario real en
el código vigente; una ruta histórica no concede autoridad si el comportamiento cambió.

## Fortalezas que deben conservarse

- `SaveService` escribe primero un temporal, lo valida y reemplaza el archivo principal con
  respaldo.
- Existen copia `.bak`, históricos, recuperación y rechazo de schema futuro.
- La persistencia dispone de inyección de fallos para probar etapas críticas.
- Las capturas visuales pueden suprimir escrituras para proteger partidas reales.
- Varios validadores usan carpetas aisladas y bloques `finally` para restaurar estado.
- Existen validadores amplios para guardado, Convergencia, dimensiones, QA y release.

Una refactorización no puede degradar estas protecciones aunque reduzca líneas o parezca
más moderna.

## Concentraciones y política de cambio

La línea base encontró estos puntos principales:

- `Dimension1System.cs`: 12149 líneas.
- `Dimension2Block1UISetup.cs`: 10817 líneas.
- `GameState.cs`: 8793 líneas; incluye un bloque editorial amplio dentro de
  `#if UNITY_EDITOR`.
- `Dimension1PanelUI.cs`: 8132 líneas.
- `SaveData`: 134 campos públicos serializados en la medición actual.

Estas cifras no autorizan una división masiva. Aplicar esta política:

1. No añadir una responsabilidad nueva a esos concentradores si puede tener propietario
   delimitado.
2. No extraer código estable sólo para reducir el número de líneas.
3. Para una extracción necesaria, conservar una fachada compatible y migrar consumidores
   por grupos comprobables.
4. Separar primero herramientas editoriales, textos de presentación o lógica nueva con
   límites claros; dejar migraciones transversales para un bloque propio.
5. Probar antes y después los invariantes del sistema y sus vecinos.

## Reglas específicas para todo cambio

Antes de implementar, registrar de forma breve:

- objetivo observable y archivos propietarios;
- consumidores y conexiones Unity afectadas;
- impacto en save, schema, migración, reset y progreso offline;
- textos visibles y claves de localización;
- coste por fotograma, búsquedas globales, asignaciones y E/S si aplica;
- validador dirigido, regresión vecina y prueba humana pendiente.

Durante la implementación:

- no mezclar el bloque solicitado con una limpieza general;
- no duplicar estado entre sistema, UI y herramienta de setup;
- mantener DEBUG fuera del runtime de producción o claramente protegido;
- no usar datos DEBUG como evidencia de progresión o balance;
- no crear otro framework de estilo, guardado o QA si el existente puede ampliarse;
- no modificar escenas, prefabs o assets serializados sin comprobar la instancia exacta de
  Unity que tiene abierto este proyecto.

## Estrategia de validación vigente

Los validadores personalizados son cobertura real, no deben descartarse por no usar NUnit.
Su arquitectura todavía es heterogénea: algunos tienen entrada batch, otros sólo menú;
varios repiten `Require`, listas de fallos, salida y limpieza, y algunos validan código
leyendo texto fuente.

Reglas de evolución:

1. No convertir los 109 validadores de una vez.
2. Crear primero un contrato o utilidad compartida para resultado, salida, aislamiento y
   limpieza.
3. Probarla con un bloque representativo de persistencia y recuperación.
4. Migrar validadores sólo cuando se toque su sistema o exista beneficio de automatización.
5. Usar lectura de fuente únicamente para contratos estructurales; preferir ejecución real
   para comportamiento.
6. Considerar `.asmdef` y Unity Test Framework en un piloto aislado; no envolver de golpe
   todo `Assembly-CSharp`, porque cambiaría límites y referencias del proyecto.

El índice para seleccionar validadores está en `INDICE_VALIDADORES_CODIGO.md`.

## Prioridades técnicas registradas

### Corregir en un bloque próximo y dirigido

- Auditar las 42 coincidencias de texto potencialmente dañado encontradas en 10 scripts,
  distinguiendo detectores intencionales, comentarios y textos visibles antes de reemplazar.
- Añadir una comprobación central que impida reintroducir mojibake en archivos de código y
  localización.
- Definir el contrato común de validadores y aplicarlo primero a guardado/recuperación.

### Mejorar al tocar el sistema relacionado

- Evitar nuevas herramientas DEBUG dentro de `GameState`.
- Separar textos visibles de sistemas de dominio cuando la tarea ya afecte esas cadenas.
- Cachear referencias runtime cuando una búsqueda global esté dentro de una ruta frecuente.
- Reducir duplicación de preparación, captura, restauración y salida entre validadores.

### Planificar como proyecto propio

- Dividir gradualmente `Dimension1System`, `GameState`, `Dimension1PanelUI` y los setup
  gigantes mediante fachadas compatibles y pruebas por rebanada.
- Evaluar ensamblajes y pruebas Unity estándar después de aislar un primer módulo, no antes.
- Automatizar una selección de regresiones de release con un único resumen compacto.

## Actualización del mapa

Actualizar este documento cuando cambie un propietario, se cree una nueva familia de
validadores, se cierre una prioridad o una cifra fechada se vuelva relevante. No registrar
cada método ni cada archivo: el código sigue siendo la fuente definitiva.

