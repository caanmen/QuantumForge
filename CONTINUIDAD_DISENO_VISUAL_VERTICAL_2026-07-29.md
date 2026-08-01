# CONTINUIDAD — QUANTUM FORGE — REDISEÑO VISUAL VERTICAL

Fecha: 2026-07-29  
Proyecto: `C:\Users\nedfla\Quantum Forge`  
Unity: `6000.3.10f1`

## 1. Objetivo del siguiente chat

Continuar desde los mockups aprobados y preparar la implementación en Unity del rediseño visual del juego base:

- Cambiar la interfaz principal, actualmente horizontal, a orientación vertical.
- Usar un estilo 2D tecnológico con profundidad 2.5D ligera.
- Separar Generación y Mejoras en pantallas distintas.
- Mantener una versión de Generación sin Triángulo y otra con el Triángulo revelado.
- Conservar la lógica, recursos, costes, niveles, requisitos, guardado y progresión existentes.
- Preparar una estructura modular y desplazable que admita el contenido posterior del juego.

Antes de modificar, el siguiente chat debe leer completamente este resumen y `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`, y después inspeccionar los archivos reales. Los archivos reales determinan el estado técnico; este resumen determina las decisiones visuales aprobadas.

## 2. Estado actual del proyecto

- El juego funciona y existe una APK de QA previa:
  `C:\Users\nedfla\Quantum Forge\Builds\Android\QuantumForge-QA-Development.apk`
- No generar una APK nueva hasta terminar y aprobar los cambios visuales.
- El worktree contiene muchos cambios existentes. Deben preservarse; no restaurar, limpiar ni sobrescribir trabajo ajeno.
- No se ha implementado todavía este rediseño visual en Unity.
- Los mockups son referencias visuales, no recursos para importar como fondos completos.

## 3. Mockups vigentes que el usuario debe adjuntar al nuevo chat

### Generación sin Triángulo

`C:\Users\nedfla\.codex\generated_images\019faaa1-f6b7-7872-8c30-4d77d6b56bd4\exec-e910901c-1674-4239-93ff-e08b81c5bcb8.png`

### Generación con Triángulo

`C:\Users\nedfla\.codex\generated_images\019faaa1-f6b7-7872-8c30-4d77d6b56bd4\exec-1b0f2430-2ce4-4866-bd7e-694a235be6c6.png`

### Pantalla independiente de Mejoras

`C:\Users\nedfla\.codex\generated_images\019faaa1-f6b7-7872-8c30-4d77d6b56bd4\exec-e9d16b8c-6095-4c13-847f-4670310fbdd8.png`

El usuario debe adjuntar estas tres imágenes al nuevo chat; no depender únicamente de las rutas.

## 4. Decisiones visuales aprobadas

### Orientación y escala

- La dirección elegida es vertical, aunque el juego actual esté configurado horizontalmente.
- Los elementos deben ser aproximadamente 25–30% más compactos que los primeros mockups.
- Deben existir márgenes amplios, jerarquía clara y botones táctiles legibles.
- Cada pantalla usará contenido vertical desplazable (`ScrollRect`) y secciones modulares.
- El espacio libre representado en los mockups no será un hueco fijo: será capacidad de continuación dentro del contenido desplazable. Los sistemas futuros ocuparán ese espacio conforme se desbloqueen.
- La barra inferior permanecerá fija dentro del área segura del teléfono.

### Estilo gráfico

- 2D tecnológico oscuro con profundidad 2.5D simulada.
- Fondo azul marino casi negro, paneles índigo, energía cian y acentos controlados púrpura/ámbar/verde.
- Profundidad mediante capas 2D, sombras y halos prehorneados, pequeños desplazamientos, pulsos y partículas sutiles.
- No usar 3D real, bloom de pantalla completa, desenfoque real, luces dinámicas ni sombras en tiempo real para la UI.
- No saturar la pantalla con neón, partículas o transparencias superpuestas.

## 5. Estructura aprobada de pantallas

### A. Generación antes de revelar el Triángulo

- Cabecera con LE, LE/s, Trazas y Trazas/s.
- Sección `ARTEFACTOS`.
- Mostrar únicamente artefactos conocidos/disponibles según la progresión real.
- En el estado ilustrado aparece Higgs adquirido y Tetraquark disponible para compra; el Modulador todavía no aparece.
- Los edificios usan la acción real `Comprar`.
- No mostrar mejoras F2 en esta pantalla.
- No mostrar silueta triangular, candado triangular, tres posiciones, espacio reservado sospechoso, `???`, texto de lore ni la palabra `detectado`.
- La pantalla debe sentirse completa aun sin el Triángulo.

### B. Generación después de revelar el Triángulo

- Cabecera con LE y Trazas.
- Triángulo compacto como foco principal.
- Higgs, Tetraquark y Modulador son vértices fijos.
- Circuitos existentes y reales:
  - Energía: Higgs + Modulador.
  - Experimental: Higgs + Tetra.
  - Fase: Tetra + Modulador; bloqueado hasta desbloquear la Máquina.
- Mostrar circuito activo, sincronización, tiempo restante y efecto real.
- Debajo, tarjetas compactas de los tres artefactos.
- Higgs y Tetraquark conservan `Comprar` para adquirir niveles.
- El Modulador adquirido muestra su estado (`Vértice listo` o `Triángulo activo`) y no permite compras repetidas.
- No mostrar mejoras F2 en esta pantalla.
- Eliminar completamente la antigua columna independiente con Higgs/Tetra/Modulador `detectado`.

### C. Pantalla independiente `Mejoras`

- Mover el `F2Panel` fuera de Generación.
- Añadir una pantalla/pestaña de navegación `Mejoras`.
- Usar secciones compactas:
  - Producción.
  - Trazas.
  - Triángulo.
- Catálogo F2 real que debe conservar nombres, IDs, requisitos, costes y efectos:
  - Emisión Calibrada.
  - Ciclo de Contención.
  - Lectura Tetraquark.
  - Acople de Vértices.
  - Amplificador de Energía.
  - Resonancia Experimental.
  - Memoria de Sincronía.
- Las mejoras disponibles usan `Mejorar`.
- Acople usa `Activar` antes de comprarse y `Completado` después.
- Las mejoras del Triángulo solo aparecen cuando sus requisitos actuales se cumplen; no cambiar el gating de `F2UpgradeManager` sin una razón funcional aprobada.
- La lista debe ser desplazable y admitir mejoras futuras.

## 6. Navegación: decisión todavía pendiente

Los mockups vigentes muestran cuatro botones inferiores:

- Generación.
- Mejoras.
- Investigación.
- Logros.

Sin embargo, el juego posteriormente añade Cuarto 2, Dimensión 1, Dimensión 2, Dimensión 3 y Prestigio. Todos esos accesos no caben simultáneamente en una barra vertical de cuatro botones.

Se propuso sustituir el cuarto botón `Logros` por `Más`, que abriría un menú o cajón con los accesos posteriores conforme se desbloqueen. Esta propuesta NO está aprobada todavía y debe confirmarse con el usuario antes de implementarse.

`Más` no significa `Máquina`. La Máquina debe continuar accediéndose desde Cuarto 2, como funciona actualmente, salvo una decisión futura explícita del usuario.

No inventar la navegación definitiva sin confirmar este punto.

## 7. Lore y revelación del Triángulo: pendiente obligatorio

- No crear lore en este bloque.
- El usuario explicó únicamente que el Triángulo estará oculto al comienzo y será revelado tras cierto progreso.
- No se ha decidido qué evento, misión, texto narrativo, compra o hito provoca la revelación.
- No vincular automáticamente la revelación a Acople, Modulador, Máquina u otro estado sin aprobación.
- Se pueden preparar dos raíces visuales separadas, por ejemplo:
  - `GenerationBeforeTriangleRoot`.
  - `GenerationTriangleRoot`.
- La condición definitiva de cambio debe quedar centralizada y pendiente.
- Si la implementación necesita un interruptor temporal de QA/editor para revisar ambos estados, debe identificarse como herramienta de prueba y no como diseño canónico.

## 8. Hallazgos técnicos de la auditoría

### Escena y jerarquía

- La única escena activa de build es `Assets/Project/Scenes/Main.unity`.
- El Triángulo y la Máquina están construidos directamente dentro de `Main.unity`; no existen como prefabs independientes.
- No editar manualmente el YAML grande de la escena. Crear o actualizar la jerarquía mediante una herramienta Editor idempotente, conectarla y validarla.

### Triángulo actual

- Jerarquía principal actual:
  `Canvas/HUD/Panel_Generacion/TopArea/GenerationTriangleLayout`.
- Componentes relevantes:
  - `Assets/Project/Scripts/UI/TrianglePanelUI.cs`.
  - `Assets/Project/Scripts/UI/TriangleSlotUI.cs`.
  - `Assets/Project/Scripts/UI/TriangleArtifactCardUI.cs`.
  - `Assets/Project/Scripts/UI/TriangleSelectionUI.cs` (legado parcialmente serializado).
  - `Assets/Project/Scripts/Core/GameState.cs` (lógica canónica).
- `TabsUI.RefreshGenerationLayoutState()` activa actualmente tanto el layout normal como el del Triángulo. Esto debe rediseñarse para soportar estados visuales alternativos.
- Las tres tarjetas `detectado` son textos fijos de escena y aparecen activas siempre; no reflejan correctamente la propiedad real de los artefactos.
- `TriangleArtifactCardUI` es principalmente decorativo; sus acciones de clic/arrastre son no-op.
- La lógica de circuitos, sincronización, Fase y requisitos ya existe en `GameState` y debe preservarse.

### Mejoras actuales

- El catálogo se gestiona mediante:
  - `Assets/Project/Scripts/Systems/F2UpgradeManager.cs`.
  - `Assets/Project/Scripts/UI/F2UpgradeRowUI.cs`.
  - `Assets/Project/Scripts/UI/F2UpgradeVisibilityController.cs`.
  - `Assets/Project/Resources/Data/f2_upgrades.json`.
- El `F2Panel` está actualmente dentro de Generación.
- Separarlo visualmente no requiere cambiar las fórmulas ni el guardado; requiere nueva raíz/panel, navegación y conexiones serializadas.

### Máquina

- Presentación: `Assets/Project/Scripts/UI/MachinePanelUI.cs`.
- Lógica: `Assets/Project/Scripts/Systems/MachineManager.cs`.
- La Máquina puede rediseñarse posteriormente como mapa gráfico sin cambiar su lógica.
- No forma parte de estos tres mockups ni de la primera implementación visual, salvo que el usuario amplíe el alcance.

### Tema oscuro

- `Assets/Project/Scripts/UI/Dimension1DarkThemeRuntime.cs` recolorea gran parte del Canvas al iniciar.
- Puede sobrescribir colores de los nuevos elementos.
- La implementación debe añadir una exclusión, marcador o estrategia de skin controlada para que el nuevo diseño conserve sus colores.
- `TriangleSlotUI.Awake()` también sustituye colores serializados por la paleta actual.

## 9. Rendimiento Android

- El proyecto usa Unity 6 y URP 17.3 con una configuración móvil razonable.
- Android utiliza el perfil Mobile, render scale aproximado 0.8, SRP Batcher y ajustes simplificados.
- El estilo aprobado es viable si se simula con UI 2D.
- El riesgo actual mayor no son los nuevos gráficos: la escena posee un Canvas muy grande, más de 1.300 elementos gráficos, muchos raycasts, máscaras y scripts con actualizaciones frecuentes.
- Recomendaciones:
  - Separar fondo estático, información dinámica y efectos por frecuencia de actualización.
  - Desactivar `RaycastTarget` en decoración y textos no interactivos.
  - No reasignar textos, colores o layouts cuando el valor no cambió.
  - Evitar transparencias grandes superpuestas y máscaras stencil anidadas.
  - Usar atlas de sprites, compresión Android y una fuente SDF compartida.
  - Usar pocos emisores y aproximadamente 30–80 partículas simples simultáneas.
  - Brillos mediante sprites/gradientes o shader UI simple, no postprocesado de pantalla completa.
  - Probar después con Profiler y Frame Debugger en Android de gama baja/media.

## 10. Implementación sugerida por bloques

1. Auditar configuración actual de orientación, CanvasScaler, Safe Area, anchors y ScrollRects.
2. Preparar orientación vertical sin romper paneles posteriores.
3. Crear la navegación nueva y la pantalla independiente de Mejoras, dejando pendiente el cuarto botón definitivo.
4. Crear Generación sin Triángulo y Generación con Triángulo como raíces separadas.
5. Retirar visualmente las tarjetas `detectado` y conectar los artefactos a sus niveles reales.
6. Reutilizar las llamadas públicas y estados existentes; no reescribir la lógica del Triángulo ni de F2.
7. Crear/importar sprites optimizados y animaciones ligeras. No usar los mockups como fondos planos.
8. Ajustar `Dimension1DarkThemeRuntime` para respetar el nuevo skin.
9. Crear una herramienta Editor idempotente para modificar `Main.unity` y un validador específico de jerarquía/referencias.
10. Validar compilación, navegación, estados visuales, guardado/carga y compatibilidad con partidas antiguas.
11. Capturar Game View vertical para revisión del usuario.
12. No generar APK hasta la aprobación visual.

## 11. Validaciones mínimas esperadas

- Compilación C# sin errores relacionados.
- Las referencias de `TabsUI`, paneles y botones quedan conectadas.
- Generación sin Triángulo no contiene pistas triangulares.
- Generación con Triángulo refleja los estados reales de `GameState`.
- Comprar edificios sigue funcionando.
- Comprar/mejorar F2 sigue funcionando desde su nueva pantalla.
- Visibilidad y requisitos F2 continúan correctos.
- Fase sigue bloqueada hasta la Máquina.
- Español e inglés permanecen sincronizados para cualquier clave nueva.
- Safe Area y navegación correctas en varias relaciones verticales.
- Ninguna pantalla posterior queda inutilizable por el cambio de orientación.
- Guardados existentes cargan sin perder niveles, Acople, circuito ni mejoras.
- `git diff --check` sin errores de formato.
- No se crea APK durante este bloque.

## 12. Reglas de trabajo que deben conservarse

- Una instrucción explícita actual del usuario tiene prioridad.
- El usuario conserva la autoridad creativa. No inventar mecánicas, lore, costes, recompensas, requisitos, desbloqueos ni textos narrativos determinantes.
- Revisar documentos y archivos reales antes de modificar.
- Antes de un bloque importante, indicar brevemente archivos/sistemas a tocar, motivo, comportamiento esperado y pruebas/conexiones necesarias.
- Avanzar autónomamente dentro del bloque aprobado; detenerse ante una decisión creativa pendiente, contradicción importante, acción destructiva o riesgo material.
- Preservar el worktree y los cambios existentes. No limpiar, restaurar ni sobrescribir trabajo ajeno.
- No hacer stage, commit, push, reset, checkout ni crear ramas salvo petición explícita.
- Los cambios manuales de archivos se realizan con `apply_patch`.
- No editar directamente el YAML grande de `Main.unity`; usar una herramienta Editor idempotente y validada.
- Si se modificarán escenas, prefabs, assets, metas o conexiones serializadas, Unity normal debe estar cerrado para evitar sobrescrituras.
- No ejecutar Unity batch mientras el Editor normal esté abierto.
- Mantener compatibilidad con guardados existentes.
- Mantener español e inglés sincronizados.
- Separar claramente código terminado, conexiones realizadas, pruebas ejecutadas y pendientes.
- No afirmar que algo está completamente jugable si faltan conexiones o validación visual indispensable.
- No generar una APK hasta que el usuario apruebe el rediseño visual.

## 13. Primer mensaje recomendado para el nuevo chat

> Lee completamente `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md` y `CONTINUIDAD_DISENO_VISUAL_VERTICAL_2026-07-29.md`. Revisa directamente los archivos actuales del proyecto y los tres mockups adjuntos. Antes de modificar, explícame brevemente qué archivos o sistemas tocarás, cómo migrarás la UI de horizontal a vertical, cómo separarás Mejoras de Generación, cómo preservarás la lógica y qué validaciones ejecutarás. No inventes el disparador de revelación del Triángulo ni la navegación definitiva del cuarto botón: ambos siguen pendientes de mi aprobación. Preserva el worktree, no uses Git y no generes APK.
