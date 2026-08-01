# CONTINUIDAD FINAL — QUANTUM FORGE — IMPLEMENTACIÓN DE UI VERTICAL

Fecha: 2026-07-29  
Proyecto: `C:\Users\nedfla\Quantum Forge`  
Unity: `6000.3.10f1`  
Estado: diseño visual aprobado; implementación todavía no iniciada  
Modelo recomendado para el bloque estructural: Sol High

## 0. Instrucción inicial para el nuevo chat

El nuevo chat debe leer completamente este archivo y después inspeccionar directamente los archivos actuales del proyecto antes de modificar nada. Los archivos reales determinan el estado técnico; este resumen determina las decisiones visuales y de alcance aprobadas.

Antes de implementar, debe explicar brevemente:

- Qué archivos y sistemas tocará.
- Cómo migrará la interfaz horizontal a vertical.
- Cómo separará Mejoras de Generación.
- Cómo creará los dos estados visuales de Generación.
- Cómo preservará lógica, guardados y progresión.
- Cómo conectará la navegación principal y la barra progresiva de sistemas.
- Qué herramientas Editor y validaciones utilizará.

No usar Git salvo petición explícita. No generar APK. No editar manualmente el YAML grande de `Main.unity`. Preservar todo el worktree existente.

## 1. Objetivo aprobado

Implementar el rediseño vertical del juego base siguiendo los mockups finales:

- Orientación vertical para teléfono.
- Interfaz tecnológica oscura 2D con profundidad 2.5D ligera.
- Generación y Mejoras en pantallas separadas.
- Generación sin Triángulo antes de Acople.
- Generación con Triángulo después de comprar Acople.
- Barra de navegación principal fija.
- Barra secundaria progresiva para sistemas posteriores.
- Estructura modular, desplazable y preparada para contenido futuro.
- Conservación completa de lógica, recursos, costes, niveles, requisitos, guardado y progresión existentes.

Los mockups son referencias visuales. No deben importarse como fondos completos ni usarse como una imagen plana interactiva.

## 2. Mockups finales aprobados

El usuario debe adjuntar las cuatro imágenes al nuevo chat. No depender únicamente de las rutas locales.

### A. Generación sin Triángulo

`C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-4b0a1167-2ae3-46f7-874f-013cc44c233c.png`

Estado representado:

- Juego base temprano.
- Higgs adquirido.
- Tetraquark disponible.
- Triángulo completamente ausente.
- Barra secundaria de sistemas ausente.

### B. Generación con Triángulo, antes de sistemas posteriores

`C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-e1f61622-7a55-4da9-9ba6-0befb36dcc90.png`

Estado representado:

- Acople comprado.
- Triángulo revelado.
- Circuitos disponibles según el estado real.
- Ningún sistema posterior desbloqueado todavía.
- Barra secundaria ausente.

### C. Pantalla independiente de Mejoras

`C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-1d0b87f1-665a-4b68-a3ae-91de1cd616c0.png`

Estado representado:

- Catálogo avanzado para mostrar el diseño completo.
- Las filas reales deben aparecer progresivamente; no se muestran todas desde el inicio.
- Mejoras es la pestaña seleccionada.

### D. Generación avanzada con sistemas posteriores

`C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-6c9cfbd1-d027-44a3-8346-3a4809617ea2.png`

Estado representado:

- Triángulo revelado.
- Varios sistemas posteriores desbloqueados.
- Barra secundaria horizontal visible.
- La imagen muestra todos los accesos únicamente para validar la capacidad final; no aparecen todos de golpe durante la partida.

Los valores de LE, Trazas, niveles, costes y sincronización mostrados en los mockups son ilustrativos. La implementación debe mostrar siempre los valores reales del runtime.

## 3. Decisiones visuales definitivas

### 3.1 Orientación, escala y Safe Area

- Cambiar el juego a orientación vertical.
- Usar una referencia vertical adecuada, previsiblemente `1080 x 1920`, validada en varias relaciones de aspecto.
- Mantener elementos aproximadamente 25–30% más compactos que los primeros conceptos.
- Conservar márgenes amplios, jerarquía clara y controles táctiles legibles.
- Usar `ScrollRect` vertical para contenido de cada pantalla.
- Mantener cabecera y navegación dentro del Safe Area.
- El espacio libre de los mockups no es un hueco fijo: es capacidad de continuación del contenido desplazable.
- Las pantallas posteriores deben seguir siendo utilizables en vertical, aunque no se rediseñen visualmente en este primer bloque.

### 3.2 Estilo

- Fondo azul marino casi negro.
- Paneles índigo oscuros.
- Cian para energía y selección principal.
- Púrpura para Trazas/Tetraquark.
- Ámbar para Triángulo/Modulador y QA provisional.
- Verde para estados completados.
- Profundidad mediante capas 2D, bordes, sombras, halos prehorneados y desplazamientos pequeños.
- Animaciones ligeras: pulsos, flujo de circuito, partículas sutiles y cambios de estado.
- No usar 3D real, bloom de pantalla completa, desenfoque real, luces dinámicas ni sombras en tiempo real para UI.
- No saturar la pantalla con neón, partículas o transparencias superpuestas.

### 3.3 Cabecera

- Dos paneles equilibrados: LE y Trazas.
- Cada panel muestra saldo y producción por segundo reales.
- No existe engranaje en la cabecera final.
- Ajustes vive en la barra inferior, evitando duplicación.

## 4. Generación antes del Triángulo

- Cabecera con LE, LE/s, Trazas y Trazas/s.
- Título `GENERACIÓN`.
- Sección `ARTEFACTOS`.
- Mostrar únicamente artefactos conocidos/disponibles según la progresión real.
- En el estado ilustrado: Higgs adquirido y Tetraquark disponible para compra.
- El Modulador no aparece hasta que su gating real lo permita.
- Las acciones de edificios conservan `Comprar`.
- No mostrar mejoras F2 en Generación.
- No mostrar silueta triangular, candado triangular, posiciones vacías, espacio sospechoso, `???`, texto de lore ni la palabra `detectado`.
- La pantalla debe sentirse completa sin el Triángulo.
- La lista debe poder crecer verticalmente sin rediseñar la pantalla.

## 5. Revelación provisional del Triángulo

Decisión final para esta implementación:

- Mantener el comportamiento actual del código.
- Comprar `Acople de Vértices` establece `triangleSystemUnlocked = true`.
- Ese mismo estado controla el cambio visual desde Generación sin Triángulo hacia Generación con Triángulo.
- No añadir ahora un campo narrativo separado como `triangleVisualRevealed`.
- No añadir migración de guardado por lore en este bloque.
- Las partidas antiguas continúan usando su estado actual de Acople/Triángulo.
- Más adelante se hará un cambio separado para introducir la revelación narrativa y su migración correspondiente.
- No inventar lore, evento, misión ni texto narrativo durante este bloque.

## 6. Generación con Triángulo

- Cabecera con LE y Trazas reales.
- Triángulo compacto como foco principal.
- Higgs, Tetraquark y Modulador son vértices fijos.
- Circuitos existentes y reales:
  - Energía: Higgs + Modulador.
  - Experimental: Higgs + Tetra.
  - Fase: Tetra + Modulador.
- Fase permanece bloqueado hasta desbloquear la Máquina.
- Mostrar circuito activo, sincronización, tiempo restante y efecto real.
- El lado activo se ilumina; los otros permanecen contenidos y legibles.
- Debajo aparecen tarjetas compactas de los tres artefactos.
- Higgs y Tetraquark conservan `Comprar` para adquirir niveles.
- El Modulador adquirido muestra `Vértice listo` o `Triángulo activo` y no permite compras repetidas.
- No mostrar mejoras F2 en Generación.
- Eliminar la antigua columna independiente con tarjetas `detectado`.
- No reescribir la lógica de circuitos ni sus fórmulas.

## 7. Pantalla independiente de Mejoras

- Mover `F2Panel` fuera de Generación.
- Crear una pantalla/pestaña independiente `MEJORAS`.
- Lista vertical desplazable y modular.
- Secciones visuales:
  - Producción.
  - Trazas.
  - Triángulo.

Catálogo real que debe conservar nombres, IDs, requisitos, costes, efectos y guardado:

1. Emisión Calibrada — `emission_focus`.
2. Ciclo de Contención — `containment_tuning`.
3. Lectura Tetraquark — `tetraquark_stabilization`.
4. Acople de Vértices — `triangle_unlock_1`.
5. Amplificador de Energía — `triangle_impulse_tuning`.
6. Resonancia Experimental — `triangle_synergy_resonance`.
7. Memoria de Sincronía — `triangle_persistence_anchor`.

Reglas:

- Las mejoras disponibles usan `Mejorar`.
- Acople usa `Activar` antes de comprarse y `Completado` después.
- Las mejoras del Triángulo solo aparecen cuando cumplen sus requisitos actuales.
- No cambiar el gating central de `F2UpgradeManager` sin una razón funcional aprobada.
- No mostrar filas retiradas.
- Los costes, niveles y descripciones deben ser reales y actualizarse sin reasignaciones innecesarias.
- La lista debe admitir mejoras futuras.

## 8. Navegación principal definitiva para este bloque

Barra inferior fija, dentro del Safe Area, en este orden:

1. `GENERACIÓN` — icono átomo.
2. `MEJORAS` — icono doble chevrón.
3. `INVESTIGACIÓN` — icono microscopio.
4. `AJUSTES` — icono engranaje.
5. `QA` — icono de herramientas, acento ámbar.

Decisiones:

- No usar botón `Más`.
- `Logros` no es necesario para las pruebas actuales y se retira de la barra principal de este bloque.
- No eliminar la lógica, datos ni panel de Logros; solo deja de ser un acceso principal por ahora.
- `Ajustes` contiene inicialmente el cambio de idioma.
- Existe una segunda opción futura de Ajustes todavía no definida; no inventarla ni bloquear la implementación por ella.
- `QA` es provisional, separado visualmente y solo debe estar disponible en builds/estados de desarrollo autorizados.
- En una build pública, QA debe permanecer oculto.
- La barra principal no cambia de posición cuando se desbloquean sistemas posteriores.

## 9. Barra secundaria progresiva de sistemas

La barra secundaria aparece inmediatamente encima de la navegación principal únicamente cuando existe por lo menos un sistema posterior desbloqueado.

Comportamiento progresivo:

| Progreso | Contenido visible |
| --- | --- |
| Juego inicial | La barra no existe visualmente |
| Se abre Cuarto 2 | `CUARTO 2` |
| Se desbloquea Dimensión 1 | `CUARTO 2 · DIM. 1` |
| Se desbloquea Dimensión 2 | Se añade `DIM. 2` |
| Se desbloquea Dimensión 3 | Se añade `DIM. 3` |
| Prestigio disponible | Se añade `PRESTIGIO` |

Reglas:

- No mostrar huecos vacíos.
- No mostrar botones bloqueados que adelanten contenido.
- No mostrar siluetas, interrogantes ni nombres antes de tiempo.
- Usar accesos compactos y táctiles.
- Cuando caben, se alinean de forma estable.
- Cuando dejan de caber, la barra se desplaza horizontalmente.
- Mostrar una indicación sutil de desplazamiento, sin convertirla en ruido visual.
- Debe admitir sistemas futuros sin reconstruir la navegación principal.

Condiciones reales existentes que deben reutilizarse:

- Cuarto 2: `GameState.experimentalChamberUnlocked`.
- Dimensión 1: `GameState.dimension01Unlocked`.
- Dimensión 2: `Dimension2System.CanAccessDimension2(GameState.I)`.
- Dimensión 3: `Dimension3System.CanAccessDimension3(GameState.I)`.
- Prestigio/Convergencia: conservar `TabsUI.ShouldShowPrestige1Button(...)` y el etiquetado actual.

La Máquina continúa accediéndose desde Cuarto 2. No crear un acceso principal independiente para la Máquina.

## 10. Alcance del primer bloque

Rediseñar e implementar ahora:

- Orientación vertical y Safe Area base.
- Cabecera de recursos.
- Generación sin Triángulo.
- Generación con Triángulo.
- Pantalla independiente de Mejoras.
- Barra inferior principal.
- Panel mínimo de Ajustes con idioma.
- Acceso QA provisional de desarrollo.
- Barra secundaria progresiva de sistemas.
- Compatibilidad visual/táctil mínima para abrir Investigación, Cuarto 2, Dimensiones y Prestigio en vertical.
- Recursos gráficos modulares necesarios para estos mockups.
- Localización nueva en español e inglés.
- Herramienta Editor idempotente y validadores.

No rediseñar visualmente todavía:

- Investigación completa.
- Logros.
- Cuarto 2/Máquina.
- Dimensión 1.
- Dimensión 2.
- Dimensión 3.
- Prestigio/Convergencia.

Esas pantallas deben seguir funcionales y accesibles, pero recibirán sus rediseños propios cuando existan mockups aprobados.

## 11. Estado técnico real auditado

### Proyecto y escena

- Unity `6000.3.10f1`.
- Única escena activa de build: `Assets/Project/Scenes/Main.unity`.
- El Triángulo y la Máquina viven directamente dentro de `Main.unity`; no son prefabs independientes.
- El Canvas actual usa referencia horizontal `1920 x 1080`, `matchWidthOrHeight = 0.6`.
- La orientación actual permite landscape y no portrait; debe migrarse.
- Existe `MobileSafeAreaRoot` y `MobileQaFriendlyLayout`, pero están configurados para el layout horizontal actual.
- La escena tiene un Canvas muy grande y más de 1.300 elementos gráficos.

### Navegación actual

- Control principal: `Assets/Project/Scripts/UI/TabsUI.cs`.
- Tiene referencias a Generación, Investigación, Cuarto 2, Dimensiones 1–3, Logros y Prestigio.
- No tiene todavía `panelMejoras` ni `btnMejoras`.
- `generationDefaultLayout` está sin conectar en la escena.
- `generationTriangleLayout` está conectado.
- `RefreshGenerationLayoutState()` activa actualmente el layout normal y el Triángulo a la vez; debe cambiarse para usar estados alternativos.
- La barra actual contiene hasta nueve botones dentro de `NavButtonsContainer` y no es adecuada para el diseño vertical final.

### Triángulo

- Lógica canónica: `Assets/Project/Scripts/Core/GameState.cs`.
- UI actual:
  - `Assets/Project/Scripts/UI/TrianglePanelUI.cs`.
  - `Assets/Project/Scripts/UI/TriangleSlotUI.cs`.
  - `Assets/Project/Scripts/UI/TriangleArtifactCardUI.cs`.
  - `Assets/Project/Scripts/UI/TriangleSelectionUI.cs` (legado parcial).
- `F2UpgradeManager.TryBuy("triangle_unlock_1")` activa `triangleSystemUnlocked` y prepara la activación.
- La lógica real de circuitos, sincronización, Fase, offline y guardado ya existe y debe preservarse.
- `TriangleSlotUI.Awake()` reemplaza colores serializados con la paleta actual; debe adaptarse al nuevo skin.
- Las antiguas tarjetas `detectado` son textos fijos de escena y deben retirarse visualmente.

### Edificios

- Lista actual: `Assets/Project/Scripts/Buildings/BuildingListUI.cs`.
- Fila actual: `Assets/Project/Scripts/Buildings/BuildingRowUI.cs`.
- Datos: `Assets/Project/Resources/Data/buildings.json`.
- La lista actual instancia todas las filas y atenúa las bloqueadas; el nuevo diseño debe ocultar artefactos todavía desconocidos cuando la especificación lo exige.
- Higgs y Tetraquark conservan niveles repetibles.
- El Modulador es compra única y ya bloquea compras posteriores.

### Mejoras F2

- Manager: `Assets/Project/Scripts/Systems/F2UpgradeManager.cs`.
- Fila: `Assets/Project/Scripts/UI/F2UpgradeRowUI.cs`.
- Visibilidad: `Assets/Project/Scripts/UI/F2UpgradeVisibilityController.cs`.
- Datos: `Assets/Project/Resources/Data/f2_upgrades.json`.
- El gating central y la migración de siete mejoras ya están implementados.
- `F2Panel` vive actualmente dentro de Generación y debe moverse a su propia raíz/pantalla.
- Separarlo visualmente no debe cambiar fórmulas, IDs ni guardado.

### Tema

- `Assets/Project/Scripts/UI/Dimension1DarkThemeRuntime.cs` recolorea grandes partes del Canvas durante el arranque.
- Puede sobrescribir los colores del nuevo diseño.
- Debe añadirse una exclusión, marcador o estrategia de skin explícita para los nuevos componentes.
- Dimensiones 2 y 3 no deben recolorearse accidentalmente.

### Recursos gráficos

- El proyecto no contiene todavía un paquete de sprites ni una tipografía específica para reproducir los mockups.
- Crear/importar los sprites, marcos, iconos, halos y fuente SDF necesarios forma parte de la implementación.
- Usar atlas y compresión Android.
- No usar los mockups como fondos completos.

### Estado de validación previo

La base existente estaba estable en la auditoría del 2026-07-29:

- Unity normal estaba cerrado en el momento de la auditoría; el nuevo chat debe volver a verificarlo antes de modificar escena/assets.
- La validación integral anterior pasó lógica del Triángulo, migración F2, guardado, Safe Area, botones móviles, escena y español/inglés.
- Log relevante: `Logs/codex_sol_medium_corrections_validation.log`.
- Build QA previa existente: `Builds/Android/QuantumForge-QA-Development.apk`.
- No generar otra APK hasta aprobación visual explícita.

## 12. Riesgos técnicos y rendimiento Android

- El riesgo principal es el Canvas grande, no los nuevos gráficos.
- Separar fondo estático, datos dinámicos y efectos por frecuencia de actualización.
- Desactivar `RaycastTarget` en decoración y textos no interactivos.
- No actualizar textos, colores o layouts cuando el valor no cambió.
- Reducir `Update()` por fila cuando sea posible; usar eventos o intervalos compartidos.
- Evitar máscaras stencil anidadas y transparencias grandes superpuestas.
- Usar atlas, fuente SDF compartida y compresión Android.
- Usar pocos emisores y aproximadamente 30–80 partículas simples simultáneas.
- Brillos mediante sprites/gradientes o shader UI simple.
- No usar postprocesado de pantalla completa.
- Validar con Profiler y Frame Debugger en Android de gama baja/media antes del cierre final.

## 13. Implementación recomendada por bloques

### Bloque 1 — Base vertical y skin

- Verificar Unity cerrado.
- Auditar de nuevo el worktree sin limpiar nada.
- Configurar portrait.
- Migrar CanvasScaler y Safe Area.
- Crear componentes de skin y recursos modulares.
- Aislar el nuevo skin de `Dimension1DarkThemeRuntime`.
- Preparar un root vertical común y navegación fija.

### Bloque 2 — Navegación

- Ampliar `TabsUI` o crear un controlador compatible.
- Crear `btnMejoras` y `panelMejoras`.
- Crear `btnAjustes` y panel mínimo de idioma.
- Integrar `btnQA` provisional con visibilidad de desarrollo.
- Retirar Logros de la barra sin borrar su sistema.
- Crear barra secundaria dinámica y horizontal.
- Reutilizar los gates reales de Cuarto 2, dimensiones y Prestigio.

### Bloque 3 — Generación sin Triángulo

- Crear `GenerationBeforeTriangleRoot`.
- Crear lista modular de artefactos conocidos.
- Conectar compras y estados reales.
- Garantizar ausencia total de pistas triangulares.

### Bloque 4 — Generación con Triángulo

- Crear `GenerationTriangleRoot`.
- Conectar su visibilidad a `triangleSystemUnlocked`.
- Reutilizar lógica de circuitos y sincronización.
- Conectar tarjetas de artefactos a niveles reales.
- Retirar presentación `detectado`.

### Bloque 5 — Mejoras independiente

- Mover/recrear visualmente F2 en `panelMejoras`.
- Mantener IDs y llamadas de compra.
- Crear secciones Producción, Trazas y Triángulo.
- Conservar visibilidad progresiva.
- Hacer la lista desplazable y extensible.

### Bloque 6 — Herramienta Editor y conexiones

- No editar manualmente el YAML grande de `Main.unity`.
- Crear una herramienta Editor idempotente para construir/actualizar jerarquía, sprites, referencias y eventos.
- La herramienta debe poder ejecutarse varias veces sin duplicar objetos ni listeners.
- Crear un validador de jerarquía y referencias específico para la UI vertical.

### Bloque 7 — Validación y revisión visual

- Compilar C#.
- Ejecutar validadores existentes y nuevos.
- Probar guardado nuevo y antiguo.
- Capturar Game View vertical en varios aspectos.
- Entregar capturas para aprobación del usuario.
- No generar APK.

## 14. Validaciones mínimas obligatorias

- Compilación C# sin errores relacionados.
- `Main.unity` abre sin Missing Scripts ni referencias rotas.
- Herramienta Editor idempotente.
- Generación sin Triángulo no contiene pistas triangulares.
- Comprar Acople cambia a Generación con Triángulo.
- Guardar/cargar conserva correctamente Acople y el estado visual.
- Generación con Triángulo refleja `GameState` real.
- Higgs y Tetraquark siguen comprándose.
- Modulador continúa siendo compra única.
- Circuitos Energía y Experimental funcionan.
- Fase sigue bloqueado hasta la Máquina.
- Sincronización, tiempo restante y efectos coinciden con la lógica.
- Mejoras F2 compran desde la pantalla nueva.
- Visibilidad y requisitos F2 no cambian.
- Barra principal navega correctamente.
- Ajustes cambia idioma sin desincronizar textos.
- QA está visible solo donde corresponde.
- Barra secundaria está ausente al inicio.
- Cada sistema aparece únicamente al cumplir su gate real.
- La barra secundaria se desplaza cuando no caben sus elementos.
- Investigación, Cuarto 2, Dimensiones y Prestigio continúan accesibles en vertical.
- Español e inglés permanecen sincronizados.
- Safe Area correcta con notch y barra de gestos.
- Probar al menos relaciones aproximadas 9:16, 9:19.5 y una relación vertical estrecha adicional.
- Guardados anteriores cargan sin perder edificios, mejoras, Acople, circuito o sincronización.
- `git diff --check` únicamente como consulta de formato, sin stage ni commit.
- No crear APK durante este bloque.

## 15. Pendientes no bloqueantes

- La segunda opción del panel Ajustes todavía no está definida. Implementar solo idioma y dejar estructura extensible; no inventar la segunda opción.
- El lore y el futuro evento narrativo de revelación del Triángulo se diseñarán después. No anticiparlos.
- Investigación, Logros, Cuarto 2, Máquina, Dimensiones y Prestigio necesitarán mockups propios para un rediseño visual completo posterior.
- Balance visual fino, tiempos de animación y densidad exacta de partículas se ajustarán después de la primera captura vertical en Unity.

## 16. Reglas completas de trabajo y continuidad

### 16.1 Prioridad y autoridad

- Una instrucción explícita y actual del usuario tiene prioridad.
- El diseño aprobado por el usuario determina el contenido creativo y mecánico.
- El asistente no puede completar vacíos de diseño por iniciativa propia.
- El código y los archivos reales determinan el estado técnico actual.
- Este resumen determina intención, alcance y decisiones visuales aprobadas.
- Si código, resumen o diseño se contradicen, informar antes de cambios importantes.
- El objetivo es avanzar con rapidez sin sacrificar calidad, estabilidad, compatibilidad ni claridad.

### 16.2 Inicio de un chat nuevo

- Leer completamente este resumen.
- Revisar directamente los archivos actuales antes de proponer o realizar cambios.
- No asumir que el resumen sustituye al código.
- Continuar desde el primer bloque pendiente.
- No repetir bloques ya terminados y probados.
- Resolver contradicciones antes de implementar la parte afectada.

### 16.3 Revisión previa a un bloque importante

Antes de iniciar un bloque funcional importante, indicar brevemente:

- Archivos o sistemas que se modificarán.
- Motivo de cada modificación.
- Comportamiento esperado.
- Conexiones, automatización y pruebas posteriores necesarias en Unity.

Una vez solicitado el bloque, avanzar por sus subdivisiones sin pedir autorización repetidamente. Detenerse solo ante una decisión creativa pendiente, contradicción importante, riesgo material, acción destructiva o alcance nuevo.

### 16.4 Forma de trabajo

- Trabajar en el bloque funcional completo más grande que pueda implementarse y verificarse de forma segura.
- Dividir internamente en estado/guardado, lógica, interfaz, conexiones, validaciones y pruebas.
- No modificar simultáneamente varios bloques grandes independientes.
- No comenzar el siguiente bloque si el actual conserva conexiones o pruebas automatizables indispensables pendientes.
- Mantener compatibilidad con partidas antiguas.
- Preservar cambios existentes y evitar refactorizaciones generales sin necesidad concreta.
- Priorizar funcionalidad correcta antes del pulido fino.
- Si aparece un error de compilación o consola relacionado, detener el avance, corregirlo y validar de nuevo.

### 16.5 Unity, jerarquía e Inspector

- El asistente está autorizado a crear/modificar escenas, jerarquías, paneles, botones, textos, componentes, referencias y eventos necesarios para el bloque solicitado.
- El usuario no debe realizar conexiones manuales salvo que lo prefiera expresamente.
- Limitar cambios de escenas, prefabs, assets y metas al alcance.
- Unity puede permanecer abierto solo para documentación o cambios pequeños aislados en scripts, fuera de Play Mode.
- Pedir cerrar Unity antes de modificar externamente escenas, prefabs, assets, metas o conexiones serializadas.
- Pedir cerrar Unity antes de crear/modificar varios scripts interdependientes si Auto Refresh puede compilar estados parciales.
- No ejecutar Unity batch mientras el Editor normal esté abierto.
- No editar manualmente el YAML grande de `Main.unity`; usar una herramienta Editor idempotente y validada.
- Hacer conexiones cuando el bloque alcance un punto comprobable; no acumularlas hasta el final.
- Preparar validadores y accesos de desarrollo seguros para evitar exigir partidas completas.
- No afirmar que una función está completamente jugable si solo funciona mediante DEBUG o carece de UI indispensable.

### 16.6 Compilación y pruebas

- Ejecutar comprobaciones ligeras en hitos internos útiles.
- Completar conjuntos interdependientes antes de la compilación externa final del conjunto.
- Realizar compilación y validación integral al finalizar cada bloque funcional.
- Agrupar pruebas de Unity por hitos, no por cambios diminutos.
- Pedir al usuario solo revisiones visuales/interacciones breves que no puedan automatizarse.
- Separar claramente código terminado, conexiones realizadas, conexiones pendientes, pruebas ejecutadas y pruebas pendientes.
- Probar guardado/carga cuando se modifique estado persistente.
- Probar partida nueva y antigua cuando haya migración o compatibilidad involucrada.
- Un estado DEBUG sirve para pruebas técnicas aisladas, no como evidencia de balance, progresión natural o migración normal.
- La prueba jugable integral y el balance definitivo ocurren después de la estructura funcional.

### 16.7 Git y protección del trabajo

- No crear commits, ramas, staging, push, reset, checkout ni otras operaciones Git salvo petición explícita.
- Se permiten consultas de solo lectura cuando sean necesarias.
- No eliminar, reemplazar ni descartar cambios existentes del usuario.
- No modificar archivos ajenos al alcance.
- No ejecutar builds pesados cuando una comprobación ligera sea suficiente.
- Recomendar puntos de guardado antes/después de cambios delicados, dejando su ejecución al usuario.

### 16.8 Autonomía y detenciones

Avanzar autónomamente dentro del bloque solicitado. Detenerse únicamente cuando:

- Falte una decisión de diseño que cambie materialmente el resultado.
- Exista una contradicción importante entre reglas, resumen y código.
- Aparezca un error que deba resolverse antes de continuar.
- Sea necesaria una prueba visual o jugable del usuario.
- La acción sea destructiva, externa al alcance o pueda afectar trabajo no relacionado.
- El bloque esté listo para cerrarse y deba decidirse el siguiente paso.

### 16.9 Cierre de bloques

Antes de declarar un bloque terminado, confirmar:

- Código implementado.
- Compilación sin errores relacionados.
- Guardado/carga comprobados cuando corresponda.
- Compatibilidad con partidas antiguas preservada y probada.
- Herramientas DEBUG y validaciones relevantes disponibles.
- Conexiones visuales indispensables completadas.
- Prueba funcional en Unity realizada cuando corresponda.
- Errores corregidos o registrados claramente.
- Decisiones provisionales y asuntos de balance documentados.

Si falta una conexión o prueba indispensable, describir el estado como `código terminado, validación pendiente` y no como completamente terminado.

### 16.10 Chats largos y continuidad

- Avisar antes de que el chat pierda claridad por longitud.
- Recomendar chat nuevo al terminar un bloque o alcanzar un punto seguro.
- No iniciar un bloque grande nuevo si conviene preparar continuidad primero.
- Entregar un resumen autosuficiente antes del cambio.
- El nuevo chat no debe depender de conversaciones anteriores.

### 16.11 Contenido obligatorio de futuros resúmenes

Todo resumen futuro debe incluir:

- Objetivo general y trabajo actual.
- Estas reglas completas y actualizadas dentro del propio resumen.
- Orden de autoridad entre instrucciones actuales, reglas, resumen y archivos reales.
- Bloques principales terminados y futuros.
- Estado exacto de cada bloque.
- Funciones implementadas.
- Archivos modificados.
- Conexiones realizadas y pendientes.
- Pruebas ejecutadas y resultados.
- Errores encontrados y correcciones.
- Contradicciones o decisiones provisionales.
- Estado de cualquier partida alterada mediante DEBUG.
- Siguiente bloque recomendado y subdivisión interna.
- Instrucción explícita de inspeccionar archivos reales.
- Indicación de no usar Git salvo petición explícita.

### 16.12 Autosuficiencia

- Las reglas deben viajar completas en cada resumen de continuidad.
- Un resumen corto puede condensar el estado, pero no omitir reglas esenciales.
- Incluir bloques futuros y lo necesario para cerrar el alcance acordado.
- No depender de frases como `según lo hablado antes`.
- No depender únicamente de enlaces a documentos anteriores.
- Registrar cualquier contradicción todavía sin resolver.

### 16.13 Modelos y herramientas

- Avisar si un trabajo requiere razonamiento amplio, migraciones delicadas o cambios que afecten muchos sistemas.
- No detener tareas normales únicamente por este motivo.
- La decisión de modelo corresponde al usuario.
- Usar automatización y herramientas cuando reduzcan trabajo manual sin disminuir seguridad o calidad.
- Para este bloque estructural se recomienda Sol High; Medium puede usarse después para ajustes pequeños y pulido.

### 16.14 Fidelidad al diseño y autoridad creativa

- El juego y su dirección creativa pertenecen al usuario.
- Implementar el diseño aprobado; no sustituirlo por preferencias propias.
- Revisar el diseño canónico aplicable antes de implementar mecánicas.
- No inventar recursos, costes, fórmulas, tasas, cantidades, niveles, límites, requisitos, recompensas, penalizaciones, desbloqueos, relaciones, simultaneidad, textos narrativos determinantes ni comportamientos no aprobados.
- Una inferencia razonable no se convierte automáticamente en diseño canónico.
- Si una decisión necesaria permite varias interpretaciones, señalarla y esperar aprobación antes de implementar la parte afectada.
- Los valores temporales de prueba también requieren autorización previa, deben estar centralizados y ser fáciles de retirar.
- No presentar una decisión técnica o provisional como parte del diseño original.
- Registrar decisiones nuevas aprobadas en el diseño canónico antes o al mismo tiempo que se implementan.
- Si se descubre una implementación sin respaldo de diseño, informar, detener dependencias y pedir decidir si se conserva, modifica o elimina.

### 16.15 Alcance completo de los resúmenes

- Un resumen no puede limitarse al subbloque activo.
- Debe transferir el contexto del alcance completo hasta su cierre.
- Incluir bloques terminados, bloque activo, siguientes subdivisiones, integraciones futuras, decisiones, contradicciones y pendientes.
- Cuando el trabajo toque Dimensión 2 o Dimensión 3, conservar también sus documentos canónicos y reglas específicas; este rediseño no autoriza cambios mecánicos en esas dimensiones.

### 16.16 Regla final de entrega

- No generar APK hasta que el usuario apruebe visualmente la implementación vertical en capturas de Game View.
- No afirmar que la interfaz está terminada sin conexiones, compilación y validación visual.
- Al cerrar el bloque, entregar archivos modificados, conexiones, pruebas, resultados y pendientes con precisión.

## 17. Mensaje recomendado para iniciar el nuevo chat

> Lee completamente `CONTINUIDAD_IMPLEMENTACION_UI_VERTICAL_FINAL_2026-07-29.md` y revisa directamente los archivos actuales del proyecto y los cuatro mockups adjuntos. Implementa el rediseño vertical aprobado preservando lógica, guardados y progresión. Acople revela provisionalmente el Triángulo usando `triangleSystemUnlocked`. La barra fija es Generación, Mejoras, Investigación, Ajustes y QA; no existe botón Más. La barra secundaria aparece progresivamente con Cuarto 2, Dimensiones y Prestigio usando sus gates reales. Antes de modificar, explica brevemente archivos/sistemas, migración vertical, conexiones Editor y validaciones. Preserva el worktree, no uses Git, no edites manualmente `Main.unity` y no generes APK.
