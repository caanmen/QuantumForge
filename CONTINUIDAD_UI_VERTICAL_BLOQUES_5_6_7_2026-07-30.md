# CONTINUIDAD — QUANTUM FORGE — UI VERTICAL — BLOQUES 5, 6 Y 7

Fecha: 2026-07-30  
Proyecto: `C:\Users\nedfla\Quantum Forge`  
Unity: `6000.3.10f1`  
Escena principal: `Assets/Project/Scenes/Main.unity`

## 1. Instrucción para el nuevo chat

Leer completamente este archivo y después inspeccionar los archivos reales antes de modificar nada. Este documento describe el estado confirmado al cerrar el Bloque 4; el código y la escena determinan el estado técnico final si hubiera alguna diferencia.

Continuar directamente con el **Bloque 5 — Mejoras independiente**. No repetir ni reconstruir desde cero los Bloques 1–4.

Reglas obligatorias:

- No usar Git salvo petición explícita del usuario.
- No crear branch, commit, stage, push, reset ni checkout.
- No generar APK antes de aprobación visual explícita.
- No editar manualmente el YAML grande de `Main.unity`.
- Configurar la escena mediante herramientas Editor idempotentes.
- Preservar el worktree completo y los cambios existentes.
- No cambiar balance, IDs, costes, requisitos, fórmulas, guardado ni progresión.
- No inventar lore, eventos narrativos ni una segunda opción de Ajustes.
- No rediseñar Investigación, Cuarto 2, Máquina, Dimensiones o Prestigio sin mockups propios.
- Trabajar por bloques y cerrar cada bloque con compilación y regresiones.

## 2. Objetivo general aprobado

Completar la primera implementación vertical del juego base siguiendo los mockups aprobados:

- Portrait `1080 x 1920` con Safe Area.
- Cabecera común con LE, LE/s, Trazas y Trazas/s reales.
- Generación inicial completamente libre de pistas del Triángulo.
- Generación avanzada con Triángulo real.
- Mejoras F2 en una pantalla independiente.
- Navegación principal fija y barra secundaria progresiva.
- Ajustes mínimo con idioma y QA solo en estados autorizados.
- Interfaz modular, desplazable y preparada para contenido futuro.

Los mockups son referencias visuales, no fondos planos para importar.

## 3. Mockups aprobados

- Generación sin Triángulo:  
  `C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-4b0a1167-2ae3-46f7-874f-013cc44c233c.png`
- Generación con Triángulo:  
  `C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-e1f61622-7a55-4da9-9ba6-0befb36dcc90.png`
- Mejoras independiente:  
  `C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-1d0b87f1-665a-4b68-a3ae-91de1cd616c0.png`
- Generación avanzada con sistemas posteriores:  
  `C:\Users\nedfla\.codex\generated_images\019fae73-cd44-7861-a9b7-d3f332488f7d\exec-6c9cfbd1-d027-44a3-8346-3a4809617ea2.png`

## 4. Estado real al cerrar el Bloque 4

### Bloque 1 — Base vertical y skin — TERMINADO

- Orientación portrait configurada.
- CanvasScaler de referencia `1080 x 1920`.
- Safe Area para cabecera, contenido y navegación.
- Skin modular protegido de `Dimension1DarkThemeRuntime`.
- Fondo, paneles, botones, selección, glow y grid generados como sprites modulares.
- Adaptación mínima de pantallas heredadas a portrait.

Archivos principales:

- `Assets/Project/Scripts/UI/Vertical/VerticalUiTheme.cs`
- `Assets/Project/Scripts/UI/Vertical/VerticalUiSkinRoot.cs`
- `Assets/Project/Scripts/UI/Vertical/VerticalSafeAreaLayout.cs`
- `Assets/Project/Scripts/Editor/VerticalUiBlock1Setup.cs`
- `Assets/Project/Scripts/Editor/VerticalUiBlock1Validation.cs`

### Bloque 2 — Navegación — TERMINADO

- Barra principal fija en este orden:
  1. Generación.
  2. Mejoras.
  3. Investigación.
  4. Ajustes.
  5. QA.
- Barra secundaria horizontal y progresiva:
  - Cuarto 2: `GameState.experimentalChamberUnlocked`.
  - Dimensión 1: `GameState.dimension01Unlocked`.
  - Dimensión 2: `Dimension2System.CanAccessDimension2`.
  - Dimensión 3: `Dimension3System.CanAccessDimension3`.
  - Prestigio: `TabsUI.ShouldShowPrestige1Button`.
- Barra secundaria visualmente ausente cuando no hay ningún sistema desbloqueado.
- `F2Panel` movido bajo `Panel_Mejoras` sin cambiar su lógica.
- Investigación mínima funcional con el prefab existente.
- Ajustes contiene Español/Inglés.
- QA reutiliza `QaRuntimeService.IsAvailable`.
- Navegación horizontal heredada desactivada, no eliminada.

Archivos principales:

- `Assets/Project/Scripts/UI/Vertical/VerticalNavigationUI.cs`
- `Assets/Project/Scripts/UI/Vertical/VerticalSettingsPanelUI.cs`
- `Assets/Project/Scripts/Editor/VerticalUiBlock2Setup.cs`
- `Assets/Project/Scripts/Editor/VerticalUiBlock2Validation.cs`
- `Assets/Project/Scripts/UI/TabsUI.cs`

### Bloque 3 — Generación sin Triángulo — TERMINADO

- Cabecera común nueva conectada al `HUD` real.
- `GenerationBeforeTriangleRoot` vertical.
- Título Generación y sección Artefactos.
- `ScrollRect` vertical con lista extensible.
- `BuildingListUI` reutilizado; no existe un segundo registro de edificios.
- Artefactos todavía desconocidos se ocultan por completo mediante su gate real.
- Prefab `VerticalBuildingRow.prefab` para filas verticales.
- Ausencia validada de componentes, textos, candados, siluetas o pistas triangulares.
- Estado inicial y avanzado alternan por `GameState.triangleSystemUnlocked`.

Archivos principales:

- `Assets/Project/Scripts/UI/Vertical/VerticalGenerationBeforeTriangleUI.cs`
- `Assets/Project/UI/Vertical/Generated/VerticalBuildingRow.prefab`
- `Assets/Project/Scripts/Editor/VerticalUiBlock3Setup.cs`
- `Assets/Project/Scripts/Editor/VerticalUiBlock3Validation.cs`
- `Assets/Project/Scripts/Buildings/BuildingListUI.cs`
- `Assets/Project/Scripts/Buildings/BuildingRowUI.cs`
- `Assets/Project/Scripts/UI/HUD.cs`

### Bloque 4 — Generación con Triángulo — TERMINADO

- `GenerationTriangleRoot` vertical y desplazable.
- Triángulo compacto con Higgs, Tetraquark y Modulador.
- Líneas dinámicas para Energía, Experimental y Fase.
- Iluminación sigue `GameState.triangleActiveCircuit`.
- Fase respeta `GameState.IsTrianglePhaseUnlocked()` y permanece bloqueado hasta la Máquina.
- Selectores reutilizan `TriangleSlotUI` y llaman `GameState.SetTriangleCircuit`.
- `TrianglePanelUI` conserva sincronización, tiempo restante y efectos reales.
- Tres tarjetas compactas de artefactos.
- Higgs y Tetra siguen siendo repetibles.
- Modulador sigue siendo compra única.
- Ambas presentaciones llaman el mismo `BuildingPurchaseService`.
- El registro de edificios es idempotente mediante `BuildingListUI.EnsureInitialized()`.
- La presentación horizontal anterior se conserva como `LegacyGenerationTriangleLayout`, inactiva.
- No hay tarjetas visibles con texto `detectado`.

Archivos principales:

- `Assets/Project/Scripts/Buildings/BuildingPurchaseService.cs`
- `Assets/Project/Scripts/UI/Vertical/VerticalTrianglePresentationUI.cs`
- `Assets/Project/Scripts/UI/Vertical/VerticalTriangleArtifactCardUI.cs`
- `Assets/Project/Scripts/Editor/VerticalUiBlock4Setup.cs`
- `Assets/Project/Scripts/Editor/VerticalUiBlock4Validation.cs`
- `Assets/Project/Scripts/UI/TrianglePanelUI.cs`
- `Assets/Project/Scripts/UI/TriangleSlotUI.cs`

## 5. Última validación confirmada

Log final:

`Logs/vertical_ui_block4_final.log`

Resultado confirmado:

- `Vertical UI Block 1`: PASS.
- `Vertical UI Block 2`: PASS.
- `Vertical UI Block 3`: PASS.
- `Vertical UI Block 4`: PASS.
- `Triangle Redesign`: PASS.
- `Mobile QA Friendly Layout`: PASS.
- `Mobile Button Legibility`: PASS.
- `QA Main Scene Integrity`: PASS.
- `Missing Scripts = 0`.
- Referencias locales rotas = 0.
- Guardado y reapertura de escena correctos.
- Herramienta ejecutada dos veces: idempotencia PASS.
- Log final sin errores C#, excepciones o fallos relacionados.
- Unity estaba cerrado al finalizar.
- No quedaron escenas temporales `RoundTrip`.
- No se generó APK.
- No se ejecutaron operaciones Git.

Método batch probado:

`VerticalUiBlock4Setup.ConfigureAndValidateBatch`

## 6. Bloque 5 — Mejoras independiente — SIGUIENTE

### Estado actual

- `Panel_Mejoras` ya existe.
- `F2Panel` ya está separado de Generación y vive dentro de `Panel_Mejoras`.
- La navegación principal ya abre Mejoras.
- La lógica y las filas F2 existentes siguen funcionando.
- Falta reconstruir visualmente la pantalla para el mockup vertical aprobado.

### Objetivo

Crear la pantalla vertical independiente `MEJORAS` con un `ScrollRect` modular y tres secciones:

1. Producción.
2. Trazas.
3. Triángulo.

### Catálogo canónico que debe conservarse

1. Emisión Calibrada — `emission_focus`.
2. Ciclo de Contención — `containment_tuning`.
3. Lectura Tetraquark — `tetraquark_stabilization`.
4. Acople de Vértices — `triangle_unlock_1`.
5. Amplificador de Energía — `triangle_impulse_tuning`.
6. Resonancia Experimental — `triangle_synergy_resonance`.
7. Memoria de Sincronía — `triangle_persistence_anchor`.

### Reglas funcionales

- No cambiar `F2UpgradeManager`, IDs, requisitos, costes, efectos, niveles ni migración salvo una corrección demostrablemente necesaria.
- Reutilizar `F2UpgradeRowUI` y `F2UpgradeVisibilityController` o una capa visual compatible.
- Las filas aparecen progresivamente según los gates actuales.
- Producción contiene `emission_focus` y `containment_tuning`.
- Trazas contiene `tetraquark_stabilization`.
- Triángulo contiene las cuatro mejoras triangulares.
- Acople debe mostrar `Activar` antes de comprarse y `Completado` después.
- Las demás mejoras disponibles usan `Mejorar`.
- No mostrar filas retiradas.
- Costes, niveles, descripciones y moneda deben venir del runtime real.
- No crear compras alternativas ni duplicar listeners.
- La lista debe crecer sin rediseño cuando existan mejoras futuras.
- Generación debe continuar libre de filas F2.

### Archivos que deben auditarse antes de implementar

- `Assets/Project/Scripts/Systems/F2UpgradeManager.cs`
- `Assets/Project/Scripts/UI/F2UpgradeRowUI.cs`
- `Assets/Project/Scripts/UI/F2UpgradeVisibilityController.cs`
- `Assets/Project/Resources/Data/f2_upgrades.json`
- `Assets/Project/Scripts/Editor/VerticalUiBlock2Setup.cs`
- `Assets/Project/Scenes/Main.unity`, solo mediante Unity/Editor API.

### Implementación recomendada

- Crear `VerticalUiBlock5Setup.cs` y `VerticalUiBlock5Validation.cs`.
- Construir un shell vertical dentro de `Panel_Mejoras`.
- Reubicar o adaptar las siete filas existentes; no crear estados F2 duplicados.
- Añadir encabezados de sección localizados ES/EN.
- Usar paneles, botones, tipografía y paleta de `VerticalUiTheme`.
- Ejecutar el setup dos veces y comprobar nombres/referencias únicas.
- Ejecutar después los validadores de los Bloques 1–5 y las regresiones F2 existentes.

### Criterio de cierre

- Compra real desde la pantalla nueva.
- Visibilidad progresiva idéntica a la actual.
- Acople cambia correctamente a Generación con Triángulo.
- Guardado/carga conserva mejoras y estado visual.
- Ninguna fila F2 dentro de Generación.
- Compilación y regresiones completas aprobadas.

## 7. Bloque 6 — Herramienta Editor y conexiones — PENDIENTE

Los Bloques 1–5 ya tendrán herramientas Editor incrementales. No volver a crear toda la UI desde cero.

### Objetivo actualizado

Consolidar y cerrar la automatización final:

- Crear una entrada maestra, por ejemplo `VerticalUiFinalSetup.ConfigureAll`.
- Invocar en orden los setups idempotentes de los Bloques 1–5.
- Crear un validador integral final de jerarquía, referencias y eventos.
- Verificar que ejecutar la herramienta completa varias veces no duplica:
  - Roots.
  - Botones.
  - Filas.
  - Componentes.
  - Listeners.
  - Recursos generados.
- Confirmar que todos los paneles gestionados viven bajo el `ContentSlot` seguro.
- Confirmar conexiones de `TabsUI`, `HUD`, navegación, Ajustes, QA y barras progresivas.
- Confirmar que los sprites y prefabs generados usan importación/compresión Android adecuada.
- Mantener herramientas por bloque para diagnóstico; no eliminarlas.
- No añadir alcance funcional nuevo durante esta consolidación.

### Validación mínima del Bloque 6

- Setup completo ejecutado dos veces.
- Una sola instancia de cada root aprobado.
- Cero listeners duplicados.
- Cero Missing Scripts.
- Cero referencias locales rotas.
- `Main.unity` guarda, cierra y reabre correctamente.
- Todos los validadores de Bloques 1–5 continúan en PASS.

## 8. Bloque 7 — Validación y revisión visual — PENDIENTE

Este bloque debe producir evidencia visual y jugable. Las validaciones Editor actuales no sustituyen las capturas de Game View.

### Pruebas funcionales obligatorias

- Partida nueva: Generación sin ninguna pista del Triángulo.
- Comprar Higgs revela Tetra según el gate real.
- Comprar Tetra revela Modulador según el gate real.
- Comprar Acople cambia al estado avanzado.
- Higgs y Tetra se compran desde las tarjetas avanzadas.
- Modulador no admite compra repetida.
- Circuitos Energía y Experimental funcionan.
- Fase permanece bloqueado hasta la Máquina.
- Sincronización, tiempo restante y efecto coinciden con `GameState`.
- Mejoras F2 compran desde `Panel_Mejoras`.
- Guardar/cargar conserva Acople, circuito, sincronización, edificios y mejoras.
- Probar un guardado anterior además de una partida nueva.
- Español/Inglés actualizan textos sin desincronización.
- QA solo aparece cuando `QaRuntimeService.IsAvailable` lo permite.
- Barra secundaria ausente al inicio y progresiva con gates reales.

### Capturas mínimas de Game View

Capturar y entregar al usuario, como mínimo:

- Generación inicial sin Triángulo.
- Generación con Triángulo y Energía activa.
- Estado con Fase bloqueada.
- Pantalla Mejoras en estado temprano.
- Pantalla Mejoras con catálogo triangular visible.
- Barra secundaria ausente.
- Barra secundaria con varios sistemas y desplazamiento horizontal.
- Una muestra en español y otra en inglés.

Probar relaciones aproximadas:

- `9:16`.
- `9:19.5`.
- Una relación vertical estrecha adicional.
- Safe Area con notch superior y barra de gestos inferior.

### Revisión visual

- Comparar jerarquía, densidad, color y ritmo vertical con los mockups.
- Ajustar tamaños, márgenes, scroll y contraste sin alterar lógica.
- Revisar que el espacio libre sea capacidad de scroll, no un hueco fijo.
- Confirmar que navegación y cabecera no saltan de posición.
- Entregar capturas al usuario para aprobación.
- No generar APK en este bloque.
- El APK solo se considera después de aprobación visual explícita.

## 9. Validadores y regresiones relevantes

Herramientas verticales existentes:

- `VerticalUiBlock1Validation.Validate()`.
- `VerticalUiBlock2Validation.Validate()`.
- `VerticalUiBlock3Validation.Validate()`.
- `VerticalUiBlock4Validation.Validate()`.

Regresiones confirmadas:

- `TriangleRedesignValidation.Validate()`.
- `MobileQaFriendlyLayoutValidation.Validate()`.
- `MobileButtonLegibilityValidation.Validate()`.
- `QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity()`.

Buscar además validadores específicos de F2 y guardado antes de cerrar el Bloque 5. Usar los nombres reales encontrados en `Assets/Project/Scripts/Editor`; no inventar llamadas.

## 10. Riesgos y observaciones técnicas

- El Canvas original es grande; evitar efectos caros y reconstrucciones innecesarias.
- Mantener intervalos de refresco y caché de textos.
- Decoración y textos no interactivos deben usar `raycastTarget = false`.
- Evitar máscaras stencil anidadas, bloom global, blur real y luces dinámicas.
- `Dimension1DarkThemeRuntime` no debe recolorear la nueva UI vertical.
- `TriangleSlotUI` ya detecta `VerticalUiSkinRoot` y usa la paleta vertical.
- `BuildingPurchaseService` es ahora la única operación compartida para comprar desde las dos presentaciones. No duplicar esa lógica.
- `BuildingListUI.EnsureInitialized()` evita registros duplicados y permite inicializar aunque la vista temprana vaya a quedar oculta por un guardado avanzado.
- El layout horizontal del Triángulo es respaldo inactivo; no borrarlo durante los Bloques 5–7.
- La segunda opción de Ajustes sigue sin estar definida.
- El evento narrativo futuro del Triángulo sigue fuera de alcance.

## 11. Forma recomendada de iniciar el siguiente chat

Mensaje sugerido del usuario:

> Lee completamente `CONTINUIDAD_UI_VERTICAL_BLOQUES_5_6_7_2026-07-30.md`, inspecciona el estado real del proyecto y continúa directamente con el Bloque 5. Trabaja por bloques, usa herramientas Editor idempotentes, ejecuta las regresiones y no uses Git ni generes APK.

El nuevo chat debe comenzar por auditar F2, explicar brevemente qué archivos tocará y después implementar el Bloque 5 completo sin pedir confirmaciones repetidas.
