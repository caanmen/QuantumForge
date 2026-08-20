# Dimensión 2 — subpantallas fieles al código

Estas imágenes son referencias de composición y acabado. Los números visibles representan un estado de ejemplo, pero durante la implementación siempre deben proceder de `GameState` y de los métodos públicos de los sistemas indicados.

## 01 Noviciado

- UI: `D2NovitiatePanelUI`
- Sistema: `D2NovitiateSystem`
- Datos: nivel, Seguidores, Acólitos disponibles/totales, Cera, Pan ritual, tanda activa, apoyo y último resultado.
- Acciones reales: `TryStartTraining`, `TryCancelTraining`, `TryUpgrade`, `TryChangeSupportFollowers`.
- Estados: sin formación, formación activa, cancelación habilitada, nivel máximo y bloqueo por Puerta Interior.
- Regla importante: nivel II produce 2 Acólitos; duración base 360 s; formar cuesta 10 Seguidores + 22 Cera + 22 Pan; mejorar a III cuesta 60 Seguidores + 75 Cera + 75 Pan.

## 02 Umbral — Lugar de Vínculo

- UI: `D2VeiledThresholdPanelUI`
- Sistemas: `D2VeiledThresholdSystem`, `D2BondSystem`
- Datos: desbloqueo a 500 Confianza, Incienso, Tela sagrada, Piedra tallada, progreso de vínculo, Acólitos y cinco líneas.
- Acciones reales: `TryPrepare`, `TryAssignAcolytes`, `TryReleaseAcolytes`, `TryUpgrade`.
- Preparación: 100 Incienso + 100 Tela sagrada + 100 Piedra tallada.
- Camino Peregrino I→II: 40 progreso + 50 de cada Ofrenda avanzada.
- `bondProgress` es un número, no un porcentaje.

## 03 Defensa — Represalias

- UI: `D2ReprisalsPanelUI`
- Sistema: `D2Civilization2System`
- Datos: Región seleccionada, Amenaza, Cobertura, pérdida estimada, Espionaje preparado, Fragmentos, total de Represalias, operación debilitada, reglas y último resultado.
- No contiene acciones propias. Las operaciones y la Protección se gestionan en `D2OperationsPanelUI`.
- No existe historial persistente de varias Represalias; solo total y último resultado.
- Reglas: a 100% ocurre Represalia; Amenaza vuelve a 25%; Cobertura conserva la mitad; recompensa 3 Fragmentos o 6 durante Alerta.

## 04 Alerta del Ente

- UI: `D2AlertPanelUI`
- Sistema: `D2Civilization2System`
- Datos: estado de Alerta/Contención, Dominio total, tiempo hasta próxima marca, número de elecciones, efectos, marcas regionales, Protección y último resultado.
- No contiene acciones propias.
- Alerta permanente al llegar a 30% de Dominio total.
- Efectos: Rescate, Espionaje y Sabotaje generan +50% Amenaza; Represalias entregan 6 Fragmentos.
- La próxima Región marcada no se conoce antes de la elección.

## 05 Analizar Restos

- UI: sección arqueológica de `D2Civilization3PanelUI`
- Sistema: `D2Civilization3System`
- Datos: Zona, inventario Baja/Media/Alta, análisis activo, Erudito, investigación, Archivo, Indicios, Anomalía y recursos compartidos de Civilización 1.
- Acciones reales: `TryStartAnalysis` para Baja/Media/Alta, `TryHireScholar`, `TryUpgradeScholar`.
- No hay probabilidad de éxito. La calidad seleccionada determina recompensas.
- Duración base: 30 s. Recompensa base Media: 3 Conocimiento, 2 recursos de Zona, 3% investigación y 8% acumulación de Indicios.
- Erudito de Campo I→II: 30 Conocimiento + 20 Fragmentos Base; requiere Conocimiento del Ente 3.

## 06 Archivo

- UI: `D2ArchivePanelUI`
- Sistema: `D2Civilization3System`
- Acciones reales: desbloquear Cartografía Estratificada, Concordancia Anómala y Exégesis Profunda; volver a Arqueología; volver al Mapa.
- Cartografía Estratificada: Archivo II, umbral 1, 50 Conocimiento + 25 Fragmentos Base; +5% velocidad de Excavación.
- Concordancia Anómala: Archivo III, umbral 3, 75 Conocimiento + 35 Inscripciones; +10% Indicios.
- Exégesis Profunda: Archivo IV, umbral 6, 100 Conocimiento + 50 Sellos; -10% coste de lectura de Anomalías.
- No existe historial de hallazgos ni una barra porcentual del Archivo.

## 07 Investigación del Ente

- UI: `D2EntityResearchPanelUI`
- Sistema: `D2Civilization3System`
- Datos: desbloqueo, estado activo/pausado, progreso, Conocimiento, próximo hito, recursos, Datos Anómalos, Conocimiento del Ente y último resultado.
- Acciones reales antes del 100%: iniciar/pausar y aportar recursos en hitos.
- Velocidad y consumo: 1% cada 30 s y 1 Conocimiento Antiguo por 1%.
- Hitos: 30% = 25 Fragmentos + 1 Dato Básico → +1; 60% = 35 Inscripciones + 1 Dato Simbólico → +2; 85% = 45 Sellos + 1 Dato Profundo → +3; 100% = 50 de cada recurso → preparar Pacto opcional.
- Las cinco líneas del Pacto solo aparecen después de completar el hito 100%.

## Regla de implementación

No se deben codificar los valores de ejemplo de las imágenes. Cada texto, barra, visibilidad e interactuabilidad se refresca desde los componentes y sistemas anteriores. Ningún elemento decorativo debe recibir `Button` si no aparece como acción real en esta ficha.
