# Dimensión 3 — Catálogo Maestro de Automatizaciones

Estado: catálogo técnico cerrado al finalizar el Bloque 7D, contrastado con el código real.

Este documento decide qué puede controlar la Fábrica. Una acción no incluida como `AUTORIZADA` no puede ejecutarse automáticamente aunque exista una API pública capaz de hacerlo.

## 1. Estados del catálogo

- `AUTORIZADA`: diseño y API segura suficientes para implementarla.
- `PREPARAR API`: autorizada por diseño, pero falta una API por ID, persistencia manual o separación de la UI.
- `PENDIENTE DE DISEÑO`: no se programa hasta definir su comportamiento exacto.
- `PROHIBIDA`: decisión única, narrativa, de Prestigio o gasto especial.
- `YA INTERNA`: pertenece a la Fábrica y no consume una rutina externa del Núcleo.

## 2. Reglas obligatorias para todas las rutinas

1. ID estable y sistema propietario.
2. Ejecución manual previa, salvo tutorial repetitivo expresamente autorizado.
3. Una transacción como máximo por evaluación.
4. Reservas mínimas absolutas antes y después de pagar.
5. Pausar no elimina; eliminar no devuelve gastos ya ejecutados.
6. Si faltan recursos, la rutina espera y vuelve a evaluar.
7. Condición de parada explícita: cantidad, nivel, destino o repeticiones.
8. Orden estable como desempate.
9. Offline solo con Núcleo N5, en bloques de 60 segundos y con máximo de 12 horas.
10. Nunca confirmar Prestigios, misiones de cadena, decisiones únicas ni gastos de blueprints especiales.

## 3. Acciones internas de la Fábrica

| ID estable | Acción | Estado | Instalación/nivel | Manual previo | Reservas | Offline | API real |
|---|---|---|---|---|---|---|---|
| `d3.factory.produce_part` | Producir pieza V1–V6 conocida | YA INTERNA | Banco según versión | No | LE, Trazas | Sí, cola D3 | `D3ProductionSystem.TryQueuePartProduction` |
| `d3.factory.assemble_normal` | Ensamblar MK normal desbloqueado | YA INTERNA | Banco según MK | No | LE, Trazas, piezas, apoyos | Sí, cola D3 | `D3AssemblySystem.TryQueueNormalAssembly` |
| `d3.factory.assemble_trait` | Ensamblar con calibración conocida | YA INTERNA | Banco N1–N5 | Perfil/lecturas descubiertas | LE, Trazas, piezas, apoyos | Sí, cola D3 | `D3AssemblySystem.TryQueueTraitAssembly` |
| `d3.factory.research_part` | Investigar V4–V6 | YA INTERNA | Banco N4–N5 / Núcleo N4 para V6 | No | LE, Trazas, investigadores | Sí, cola D3 | `D3ResearchSystem.TryQueueResearch` |
| `d3.factory.upgrade_facility` | Construir/ampliar instalación | YA INTERNA | Cola de instalaciones | No | LE, Trazas | Sí, cola D3 | `D3FacilitySystem.TryQueueFacilityUpgrade` |

Estas acciones usan colas propias y no cuentan como decisiones externas simultáneas del Núcleo.

## 4. Consola de Producción — Cuarto 1

| ID estable | Acción | Estado | Nivel | Manual previo | Parada | Reservas | Offline | API real o brecha |
|---|---|---|---:|---|---|---|---|---|
| `d3.console.buy_higgs` | Comprar niveles del Condensador de Higgs | AUTORIZADA | 1 | Haber comprado un nivel manualmente | Nivel objetivo o número de compras | LE | N5 | `D3ConsoleSystem.TryPurchaseRepeatableBuilding` |
| `d3.console.buy_tetraquark` | Comprar niveles del Núcleo Tetraquark | AUTORIZADA | 1 | Haber comprado un nivel manualmente | Nivel objetivo o compras | LE | N5 | `D3ConsoleSystem.TryPurchaseRepeatableBuilding` |
| `d3.console.set_purchase_policy` | Prioridad LE/Trazas/equilibrio y reservas | YA INTERNA | 2 | No | Configuración persistente | LE, Trazas | N5 | `D3ConsoleSystem.TrySetPolicyAndReserves` |
| `d3.console.repeat_basic_upgrade` | Repetir mejoras básicas auditadas | PENDIENTE DE DISEÑO | 3 | Mejora usada manualmente | Nivel/cantidad | Según mejora | No hasta clasificar | No existe lista cerrada de mejoras básicas |
| `d3.console.set_triangle_circuit` | Mantener circuito preferido del Triángulo | AUTORIZADA | 5 | El circuito debe haberse elegido manualmente | Un circuito: Energía, Experimental o Fase | Ninguna | N5 | `D3ConsoleSystem.TryApplyPreferredCircuit` |
| `d3.console.buy_phase_modulator` | Comprar Modulador de Fase | PROHIBIDA | — | — | — | — | No | Artefacto único; no es compra repetible |

La reducción de costo de la Consola solo podrá aplicarse dentro del nuevo servicio de compras repetibles. Nunca alterará el costo mostrado o pagado por compras manuales, el Modulador, investigaciones, Prestigios ni otros sistemas.

Las acciones heredadas `d3.console.set_modulator_phase` y `d3.console.apply_basic_triangle` se migran al selector único de circuitos. Si una partida tenía ambas rutinas, la antigua configuración del Triángulo conserva prioridad y la rutina de fase duplicada queda pausada.

## 5. Banco de Diagnóstico — Cuarto 2

| ID estable | Acción | Estado | Nivel | Manual previo | Parada | Reservas | Offline | API real o brecha |
|---|---|---|---:|---|---|---|---|---|
| `d3.diagnostic.analyze_line_node` | Analizar nodo dañado visible con `tierGroup` | AUTORIZADA | 1 | No, tutorial repetitivo autorizado | Sin nodos válidos | Ninguna | Solo rutina N5 + Núcleo N5 | `MachineManager.TryStartNodeAnalysis` |
| `d3.diagnostic.repair_line_node` | Reparar nodo de línea válido | AUTORIZADA | 2 | Nodo analizado cuando corresponda | Sin nodos válidos | LE, Trazas y materiales reales | Solo N5 + Núcleo N5 | `CanRepairNode` + `TryRepairNode` |
| `d3.diagnostic.set_zone_priority` | Prioridad por zona y orden | AUTORIZADA | 3 | No | Configuración persistente | No gasta | Sí como parte de perfil | `D3DiagnosticSettingsState.priorityZone` |
| `d3.diagnostic.repeat_marked_fusion` | Repetir receta marcada | AUTORIZADA | 4 | Receta ejecutada manualmente | Repeticiones/recursos | Fragmentos y materiales de receta | N5 + Núcleo N5 | `D3FusionService.TryExecuteMarkedSafeFusion` |
| `d3.diagnostic.collect_fragments` | Recolectar fragmentos | PROHIBIDA | — | — | — | — | No | Los artefactos ya los generan automáticamente |
| `d3.diagnostic.unique_or_hidden_node` | Analizar/reparar nodo oculto, clave o sin `tierGroup` | PROHIBIDA | — | — | — | — | No | Decisión única o no repetible |

El Banco no modifica costos ni recompensas del Cuarto 2.

## 6. Puerto de Expedición — Dimensión 1

### 6.1 Clasificación formal de destinos actuales

El código confirma que la Parte 1 no posee una variable real de riesgo o fallo (`GetD1TreeUnstableZoneRiskReduction` devuelve 0). Por ello los diez destinos normales actuales se clasifican explícitamente como riesgo nulo para automatización simple. Esto no se infiere de sus nombres.

| Destino | Riesgo de catálogo | Repetible | Cadena | Blueprint especial obligatorio | Decisión única |
|---|---|---:|---:|---:|---:|
| `destination_mineral_belt` | Nulo | Sí | No | No | No |
| `destination_ship_graveyard` | Nulo | Sí | No | No | No |
| `destination_drifting_probes` | Nulo | Sí | No | No | No |
| `destination_abandoned_ship` | Nulo | Sí | No | No | No |
| `destination_orbital_ruin` | Nulo | Sí | No | No | No |
| `destination_laboratory` | Nulo | Sí | No | No | No |
| `destination_abandoned_station` | Nulo | Sí | No | No | No |
| `destination_minor_anomaly` | Nulo en la lógica actual | Sí | No | No | No |
| `destination_ancient_structure` | Nulo en la lógica actual | Sí | No | No | No |
| `destination_unstable_zone` | Nulo en la lógica actual | Sí | No | No | No |

Los cuatro IDs antiguos/provisionales no se generan en escaneos nuevos y quedan fuera del Puerto. Los puntos especiales son bonificaciones pasivas de una exploración simple; no convierten la ruta en una decisión única. Si en el futuro se añade fallo o elección, la clasificación deberá cambiar antes de automatizarla.

### 6.2 Acciones del Puerto

| ID estable | Acción | Estado | Nivel | Manual previo | Parada | Reservas | Offline | API real o brecha |
|---|---|---|---:|---|---|---|---|---|
| `d3.port.scan_simple_destinations` | Ejecutar barrido simple cuando haga falta una ruta | AUTORIZADA | 1 | Un barrido manual | Destino prioritario encontrado o rutina pausada | No gasta | N5 + Núcleo N5 | `TryScanSimpleDestinationAutomated`; historial manual persistente |
| `d3.port.repeat_last_simple_route` | Reenviar nave al último destino simple válido disponible | AUTORIZADA | 1 | Destino completado manualmente por esa nave o de forma global | Repeticiones/destino | Ninguna | N5 + Núcleo N5 | `TryStartExplorationByDestinationId` |
| `d3.port.priority_simple_routes` | Elegir por lista de destinos repetibles conocidos | AUTORIZADA | 2 | Cada destino completado manualmente | Cantidad/destino | Ninguna | N5 + Núcleo N5 | Lista persistente `dimension1ManualSimpleDestinationIds` |
| `d3.port.repeat_safe_route` | Repetir rutas normales de riesgo nulo/bajo | AUTORIZADA | 3 | Ruta completada manualmente | Repeticiones | Ninguna | N5 + Núcleo N5 | Catálogo formal + API por ID |
| `d3.port.upgrade_extractor` | Mejorar extractor planetario ya desbloqueado | AUTORIZADA | 4 | Ese extractor mejorado manualmente al menos una vez | Tier objetivo/compras | Metal principal del planeta | N5 + Núcleo N5 | `TryUpgradeExtractorAutomated`; historial manual persistente |
| `d3.port.second_expedition_routine` | Segunda rutina simultánea del Puerto | AUTORIZADA | 5 | Acciones internas ya autorizadas | Según cada rutina | Según cada rutina | N5 + Núcleo N5 | Motor de simultaneidad D3 |
| `d3.port.unlock_planet` | Desbloquear planeta | PROHIBIDA | — | — | — | — | No | Decisión importante no incluida en N4 |
| `d3.port.build_or_unlock_ship` | Construir/desbloquear nave | PROHIBIDA | — | — | — | — | No | Excluida expresamente |
| `d3.port.upgrade_ship` | Mejorar partes de nave | PENDIENTE DE DISEÑO | — | — | — | — | No | No incluida en el alcance actual |
| `d3.port.coordinated_mission` | Iniciar misión coordinada | PROHIBIDA | — | — | — | — | No | Excluida expresamente |
| `d3.port.ark_or_chain_mission` | Ark, sincronizaciones o cadenas | PROHIBIDA | — | — | — | — | No | Narrativa/progreso clave |
| `d3.port.upgrade_relic` | Mejorar reliquia | PROHIBIDA | — | — | — | — | No | Excluida expresamente |
| `d3.port.buy_tree_node` | Comprar nodo del árbol D1 | PROHIBIDA | — | — | — | — | No | Decisión estratégica |
| `d3.port.upgrade_scanner` | Mejorar escáner | PENDIENTE DE DISEÑO | — | — | — | — | No | El diseño autoriza barridos, no sus mejoras |

Las recompensas se entregan al completar la exploración; no existe una acción separada de recolección.

## 7. Núcleo de Automatización

| Nivel | Capacidad requerida | Efecto autorizado |
|---:|---:|---|
| 1 | 2 | +5 % a bonus de otras instalaciones; máximo 2 rutinas externas |
| 2 | 5 | 1 perfil; máximo 3 rutinas |
| 3 | 10 | +10 % total reemplaza +5 %; máximo 4 rutinas |
| 4 | 20 | 2 perfiles; máximo 5 rutinas; requisito para investigaciones V6 |
| 5 | 35 | Offline externo hasta 12 h, evaluado en bloques de 60 s |

El Núcleo nunca multiplica potencia base, requisitos, recompensas, recursos externos ni su propia capacidad. Sin Núcleo, cada instalación externa puede conservar una rutina online, pero solo una acción externa puede ejecutarse por evaluación global.

## 8. Sistemas todavía fuera del catálogo ejecutable

Permanecen `PENDIENTE DE DISEÑO` o `PROHIBIDOS` hasta una decisión posterior:

- producción base y edificios fuera de los dos artefactos repetibles del Cuarto 1;
- investigaciones generales de `ResearchManager`;
- mejoras anteriores a Prestigio 1 no enumeradas;
- Prestigio 1, Meta/Prestigio 2 y rutas de reinicio;
- decisiones adicionales de Cuarto 1 y Cuarto 2;
- Dimensión 2 / Pactos;
- dimensiones futuras.

## 9. Brechas técnicas que deben cerrarse antes de activar rutinas

1. Estado manual de Cuarto 1 y recetas de fusión; el historial manual de D1 ya está implementado.
2. Servicio de compra repetible de edificios fuera de `BuildingRowUI`.
3. Servicio de fusión fuera de `Room2PanelUI`.
4. Estado de rutinas con reservas, prioridad, parada, repeticiones y último resultado.
5. Instalaciones Puerto y Núcleo, asignaciones, capacidad, perfiles y simultaneidad.

## 10. Orden de implementación aprobado

1. Datos de catálogo, riesgo y ejecución manual previa.
2. APIs seguras por ID para D1, edificios repetibles y fusiones.
3. Puerto y Núcleo con niveles/capacidad.
4. Motor de rutinas online y límites de simultaneidad.
5. Perfiles.
6. Offline externo N5 en bloques de 60 segundos.
7. Validadores negativos que demuestren que no se tocan decisiones únicas.
