# Auditoría de cierre funcional — Dimensión 3: Fábrica

Fecha: 22 de julio de 2026  
Fuentes: `DIMENSION3_FABRICA_DISENO_CANONICO_COMPLETO.md`, `DIMENSION3_ESTRUCTURA_IMPLEMENTACION.md`, `DIMENSION3_CATALOGO_MAESTRO_AUTOMATIZACIONES.md` y código real del proyecto.

Actualización 22 de julio de 2026: **Bloque 7 completo (7A–7D)**. Las brechas funcionales auditadas quedaron cerradas salvo decisiones expresamente pendientes de diseño (Consola N3 y ampliación de simultaneidad). La regresión completa de Bloques 1–7D produce PASS. Lo siguiente es revisión visual global, jugabilidad completa y balance.

## 1. Veredicto

Los bloques 1–6 forman una base funcional estable y sus validadores pasan, pero Dimensión 3 **todavía no cumple por completo el diseño canónico**. El ciclo interno de Fábrica, el Puerto y el Núcleo están implementados. Las brechas funcionales se concentran en:

1. comportamiento dinámico de las colas;
2. automatizaciones reales de la Consola de Producción;
3. niveles avanzados del Banco de Diagnóstico;
4. alcance completo de perfiles y bonus del Núcleo;
5. controles operativos de UI, localización y presentación.

Por tanto, no corresponde empezar todavía el balance de una partida completa. Primero debe realizarse un **Bloque 7 de cierre e integración**.

## 2. Sistemas confirmados como implementados

| Área canónica | Estado | Evidencia principal |
|---|---|---|
| Desbloqueo, primera entrada y MK1 inicial único | Completo | `Dimension3System`, validación Bloque 1 |
| Estado versionado, guardado, carga y migración básica | Completo | `Dimension3State`, `GameState`, `SaveService` |
| Inventario agregado de piezas y autómatas | Completo | `D3InventorySystem` |
| Piezas V1–V6, costos y tiempos canónicos | Completo en lógica | `Dimension3Catalog`, `D3ProductionSystem` |
| Cuatro colas FIFO, diez entradas y cancelación | Completo en lógica base | `D3JobQueueSystem` |
| Offline interno hasta 12 horas | Completo | `Dimension3System.ApplyOfflineProgress` |
| MK1–MK6 normales y con rasgos | Completo | `D3AssemblySystem` |
| Potencias MK y rasgos por canal | Completo | `Dimension3Catalog`, `D3PowerSystem` |
| Asignaciones, exclusividad y estabilización de 30 s | Completo | `D3FacilitySystem`, `D3InventorySystem` |
| Cinco calibraciones, afinidad, empate y fallo a Normal | Completo en lógica | `D3CalibrationSystem` |
| Automatización gradual de calibración N1–N5 | Completo en lógica y UI básica | `D3CalibrationPanelUI` |
| Investigaciones V4–V6 y equipos reservados | Completo | `D3ResearchSystem` |
| Apoyos no consumibles para MK5/MK6 | Completo | `D3AssemblySystem` |
| Cinco instalaciones y niveles N1–N5 | Completo estructuralmente | `D3FacilitySystem`, `D3FacilitiesPanelUI` |
| Análisis persistente de Máquina | Completo | `MachineManager`, `D3DiagnosticSystem` |
| Autoanálisis y autorreparación segura online | Completo | `D3DiagnosticSystem` |
| Marcas persistentes de recetas ejecutadas manualmente | Completo como dato | `D3DiagnosticSystem`, `Room2PanelUI` |
| Clasificación explícita de destinos D1 | Completo | `D3AutomationCatalog` |
| Puerto: cinco acciones seguras online | Completo | `D3AutomationSystem` |
| Rutinas, prioridad, reservas, parada y perfiles básicos | Completo para Puerto | `D3AutomationSystem`, `D3AutomationPanelUI` |
| Puerto + Núcleo offline en bloques de 60 s y cap 12 h | Completo | validación 6D |
| Exclusión de Ark, cadenas, naves, planetas, reliquias y árbol | Completo | catálogo y validaciones negativas |

Regresión vigente: Bloque 1, Catálogo 6A, Instalaciones 6B, Online 6C y Offline 6D producen `PASS`.

## 3. Brechas funcionales de prioridad alta

### A1. Los modificadores de velocidad de trabajos quedan congelados al confirmar

El diseño establece que el costo se fija al confirmar, pero la velocidad debe recalcularse durante el progreso según las asignaciones actuales.

Actualmente `D3ProductionSystem`, `D3AssemblySystem`, `D3ResearchSystem` y `D3FacilitySystem` guardan una duración ya modificada. `D3JobQueueSystem` después resta segundos sin consultar nuevamente la potencia activa.

Consecuencia: mover autómatas, perder capacidad o mejorar el Banco no altera un trabajo ya iniciado. Esto contradice las reglas canónicas de colas.

Corrección requerida: guardar duración base y avanzar cada trabajo mediante un multiplicador dinámico; conservar el costo pagado como valor fijo.

### A2. Consola de Producción construible, pero sin automatizaciones reales

Los canales, niveles y capacidad existen, pero sus funciones N1–N5 son únicamente texto.

Falta:

- N1: servicios seguros de compra repetible para Condensador de Higgs y Núcleo Tetraquark, con historial manual;
- N2: política LE/Trazas/equilibrio y reservas persistentes;
- N3: lista cerrada de mejoras básicas repetibles; sigue `PENDIENTE DE DISEÑO`;
- N4: historial manual y mantenimiento de una fase preferida válida;
- N5: captura y aplicación de una configuración básica del Triángulo.

No debe automatizarse el Modulador como compra única, Prestigio ni configuraciones avanzadas.

### A3. Banco de Diagnóstico incompleto desde N3

N1 y N2 funcionan online. Queda pendiente:

- controles de UI para reservas y prioridad de zona/orden de N3;
- requisito real de `NodeAnalysisUnlocked` al construir N2;
- servicio de fusión separado de `Room2PanelUI`;
- ejecución N4 de recetas marcadas manualmente;
- rutina diagnóstica guardable de N5;
- autoanálisis, autorreparación y fusión offline únicamente con Diagnóstico N5 + Núcleo N5.

Las marcas de recetas existen, pero todavía ninguna rutina consume esas marcas.

### A4. Bonus de coordinación y eficiencia del Núcleo aplicado de forma parcial

El multiplicador +5 %/+10 % del Núcleo se usa en la respuesta del Puerto. Aún falta aplicarlo de forma coherente a los bonus autorizados de otras instalaciones.

También falta aplicar la Coordinación del Puerto al bonus de Respuesta de sus rutinas. El Diagnóstico usa su coordinación, pero no recibe el multiplicador general del Núcleo.

Debe conservarse la exclusión canónica: el Núcleo no multiplica su propia capacidad, potencia base, requisitos ni recompensas de D1.

### A5. Perfiles del Núcleo incompletos

Los perfiles actuales guardan copias de rutinas del Puerto con prioridades, reservas y destinos. El diseño completo también exige conservar, cuando existan:

- ajustes diagnósticos y recetas marcadas;
- fase preferida del Modulador;
- configuración básica del Triángulo;
- calibraciones autorizadas;
- políticas de compra de la Consola.

Estas secciones deben incorporarse después de implementar sus conectores; no deben guardarse referencias frágiles a objetos de UI.

### A6. El catálogo programático no representa todavía todo el Catálogo Maestro

`DIMENSION3_CATALOGO_MAESTRO_AUTOMATIZACIONES.md` contiene más acciones que `D3AutomationCatalog.cs`.

Faltan definiciones programáticas, entre otras, para:

- acciones internas de Fábrica;
- política de compra de Consola;
- mejoras básicas pendientes;
- prioridad diagnóstica;
- segunda rutina de Puerto como capacidad formal;
- acciones pendientes de nave y escáner;
- prohibiciones adicionales de Consola y Diagnóstico.

Aunque varias no sean ejecutables, deben existir con estado `Internal`, `PrepareApi`, `PendingDesign` o `Prohibited` para que el catálogo sea realmente la fuente única de seguridad.

### A7. «Repetir última ruta» no usa automáticamente la última ruta

La rutina actual recibe un destino elegido. Es segura, pero semánticamente equivale a «repetir ruta conocida», no a «último destino simple válido».

Debe decidirse si se renombra la función o si se conserva y consulta un historial estable de última ruta manual por nave/global.

### A8. Simultaneidad del Núcleo necesita confirmación final de diseño

La implementación permite varias rutinas activas, pero ejecuta una sola transacción externa por evaluación global. Esto coincide con la regla de seguridad adoptada durante 6C.

El documento canónico original puede interpretarse como varias transacciones simultáneas al existir Núcleo. Antes de cambiarlo debe confirmarse expresamente si:

- el Núcleo solo amplía la cantidad de rutinas activas, manteniendo una transacción global; o
- permite que cada rutina activa ejecute una transacción por evaluación.

Hasta esa decisión, se conserva el comportamiento seguro actual.

## 4. Brechas operativas y de UI

### U1. Lotes de piezas no accesibles desde la pantalla principal

La API acepta 1, 5, 10, 25 y 50 con Banco N5, pero la UI siempre solicita una pieza. Debe añadirse selector de lote.

### U2. Previsión de costos incompleta

El diseño exige mostrar por separado:

- costo de piezas faltantes;
- costo de ensamblaje;
- costo total estimado.

La UI actual no ofrece esa previsión completa antes de confirmar.

### U3. Ensamblaje normal por cantidad no expuesto

La API admite cantidades, pero la UI principal ensambla una unidad. Debe añadirse cantidad y validar apoyos multiplicados para MK5/MK6.

### U4. Cancelación limitada al último trabajo

La lógica puede cancelar cualquier trabajo por ID, pero la UI solo apunta al último de Producción o Ensamblaje. Falta seleccionar una entrada concreta y acceso equivalente a las colas de Investigación e Instalaciones.

### U5. Calibración de Módulo de Control valida después, pero no impide conexiones duplicadas

El sistema rechaza una permutación inválida al registrar. El diseño pide que la UI impida conexiones inválidas y muestre el circuito completo antes de aceptar.

### U6. Localización incompleta

La nueva UI de Dimensión 3 usa textos directos en español. El botón EN/ES no traduce la mayoría de sus etiquetas, estados ni errores.

### U7. Presentación visual progresiva pendiente

No se han implementado crecimiento visual de instalaciones, actividad de autómatas, expansión N1–N5, arte o animaciones. Esto estaba aplazado y no bloquea la lógica, pero sí el cierre visual definitivo.

## 5. Pruebas transversales que todavía deben añadirse

- velocidad dinámica al cambiar asignaciones durante un trabajo activo;
- potencia extrema y reducción asintótica sin cero, NaN ni infinito;
- todas las líneas V4→V5→V6, no solo muestras representativas;
- lote 50 bloqueado/desbloqueado y selector de UI;
- cancelación individual de cada posición en las cuatro colas;
- requisitos específicos de todos los niveles de todas las instalaciones;
- perfiles completos de Consola, Diagnóstico y Núcleo;
- equivalencia lógica online/offline para rutinas diagnósticas;
- traducción EN/ES de toda la interfaz D3;
- carga de una partida antigua real sin ningún campo nuevo de automatización.

## 6. Elementos aplazados que no son defectos

Continúan fuera de la primera implementación por decisión de diseño:

- calidad individual de piezas;
- autómatas individuales con nombres;
- automatización de Dimensión 2 hasta cerrar su diseño completo;
- instalación de Prestigio;
- decisiones únicas, narrativas o de cadena;
- autorrecolección de fragmentos;
- balance final;
- arte y animaciones definitivas.

## 7. Bloque 7 recomendado

### 7A — Conformidad del núcleo y UI operativa

- velocidad dinámica de trabajos;
- selector de lotes y previsión de costos;
- cantidades de ensamblaje;
- selección/cancelación de trabajos en las cuatro colas;
- requisito faltante de Diagnóstico N2;
- validadores transversales de potencia y colas.

### 7B — Consola de Producción

- preparar APIs seguras e historial manual;
- N1, N2, N4 y N5;
- mantener N3 bloqueado hasta definir lista cerrada;
- integrar catálogo, rutinas, perfiles y offline.

### 7C — Banco de Diagnóstico completo

- controles N3;
- servicio y ejecución de fusiones N4;
- rutina/perfil/offline N5;
- pruebas negativas de nodos únicos y recetas no manuales.

### 7D — Núcleo, perfiles y cierre

- propagación correcta de coordinación y +5 %/+10 %;
- perfiles completos;
- catálogo programático exhaustivo;
- resolver semántica de última ruta y simultaneidad;
- localización funcional EN/ES;
- regresión final de Dimensión 3.

Después de 7D podrán comenzar las pruebas completas de jugabilidad y balance sin ocultar brechas estructurales.
