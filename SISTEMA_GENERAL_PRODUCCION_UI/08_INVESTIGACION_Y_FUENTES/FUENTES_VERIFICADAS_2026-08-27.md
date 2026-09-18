# Fuentes verificadas — 2026-08-27

Las siguientes fuentes sustentan las guías generales. “Comunidad” indica experiencia útil
para formular una prueba, pero no autoridad normativa.

## Producción, alcance e iteración

| Fuente | Tipo | Aplicación |
|---|---|---|
| [Unity Learn: ciclo de producción](https://learn.unity.com/tutorial/explore-the-production-cycle?version=2022.3) | Oficial | Preproducción, prototipo y evaluación antes de escalar producción |
| [Análisis de postmortems de Game Developer](https://www.gamedeveloper.com/audio/dissecting-the-postmortem-lessons-learned-from-two-years-of-game-development-self-reportage) | Análisis de casos | Alcance, recursos e iteración como fuentes frecuentes de riesgo |
| [Postmortem de Knights of the Old Republic](https://www.gamedeveloper.com/design/classic-postmortem-bioware-s-i-star-wars-knights-of-the-old-republic-i-) | Fuente de equipo | La falta de prototipo interactivo de UI prolongó drásticamente la iteración |
| [Scrum Guide](https://scrumguides.org/scrum-guide.html) | Guía oficial | Definición compartida y observable de terminado |

## Unity UI y composición adaptable

| Fuente | Tipo | Aplicación |
|---|---|---|
| [Diseño UI para múltiples resoluciones](https://docs.unity3d.com/2020.1/Documentation/Manual/HOWTO-UIMultiResolution.html) | Oficial | Canvas Scaler, anclas y adaptación |
| [Canvas Scaler](https://docs.unity3d.com/2022.2/Documentation/Manual/script-CanvasScaler.html) | Oficial | Escala de referencia y comportamiento por pantalla |
| [RectTransform](https://docs.unity3d.com/6000.0/ScriptReference/RectTransform.html) | Oficial | Geometría, anclas, pivote y actualizaciones de layout |
| [Aspect Ratio Fitter](https://docs.unity3d.com/cn/2018.3/Manual/script-AspectRatioFitter.html) | Oficial | Diferencia entre contener y cubrir sin deformar |
| [CanvasGroup](https://docs.unity3d.com/cn/6000.0/ScriptReference/CanvasGroup.html) | Oficial | Alfa, interacción y grupos anidados |
| [Alfa heredado de CanvasRenderer](https://docs.unity3d.com/6000.0/ScriptReference/CanvasRenderer.GetInheritedAlpha.html) | Oficial | Multiplicación de alfa en jerarquías |
| [Optimizing Unity UI](https://learn.unity.com/tutorial/optimizing-unity-ui?language=en) | Oficial | Rebuilds, fill-rate, raycasts y división por frecuencia |
| [Consejos de optimización UI](https://unity.com/how-to/unity-ui-optimization-tips) | Oficial | Overdraw, Canvas y perfilado |
| [Canvas estáticos y dinámicos](https://learn.unity.com/tutorial/working-with-static-and-dynamic-canvases-1) | Oficial | Aislar contenido por frecuencia de cambio |
| [Safe Area](https://docs.unity3d.com/6000.0/ScriptReference/Screen-safeArea.html) | Oficial | Área segura del dispositivo |
| [Device Simulator](https://docs.unity3d.com/cn/current/Manual/device-simulator-introduction.html) | Oficial | Diagnóstico de layout; no reemplaza rendimiento real |
| [Anchors y pivots en varias resoluciones](https://discussions.unity.com/t/any-tips-on-how-to-place-your-anchors-and-pivots-so-the-ui-element-can-scale-multiple-resolutions/601898) | Comunidad | Casos que justifican probar anclas por intención funcional |
| [Experiencias de rendimiento UGUI](https://discussions.unity.com/t/unity-ui-performance-tips-sharing-my-findings/697057) | Comunidad | Señales sobre rebuild y overdraw que deben confirmarse con Profiler |

## Assets, pruebas y rendimiento móvil

| Fuente | Tipo | Aplicación |
|---|---|---|
| [Preset Manager](https://docs.unity3d.com/6000.0/Documentation/Manual/class-PresetManager.html) | Oficial | Importación reproducible por familia |
| [Texture Import Settings](https://docs.unity3d.com/6000.0/Documentation/Manual/class-TextureImporter.html) | Oficial | Tamaño, alfa, mipmaps y compresión |
| [Sprite Atlas](https://docs.unity3d.com/6000.0/Documentation/Manual/sprite/atlas/create-sprite-atlas.html) | Oficial | Agrupar sprites finales que se usan juntos |
| [Gestión de assets en runtime](https://docs.unity3d.com/ja/current/Manual/assets-managing-introduction.html) | Oficial | Elegir referencias directas, `Resources` o Addressables según necesidad |
| [Unity Test Framework](https://docs.unity3d.com/6000.0/Documentation/Manual/com.unity.test-framework.html) | Oficial | Edit Mode, Play Mode y plataforma objetivo |
| [Graphics Test Framework](https://github.com/Unity-Technologies/com.unity.testframework.graphics) | Oficial archivado | Comparación visual; revisar mantenimiento antes de adoptar |
| [Perfilado en aplicación](https://docs.unity3d.com/2022.2/Documentation/Manual/profiler-profiling-applications.html) | Oficial | El Editor no sustituye la medición en la plataforma final |
| [Optimización de juegos Android](https://developer.android.com/games/optimize) | Oficial | CPU, GPU, memoria, carga y energía |
| [Android Frame Pacing](https://developer.android.com/games/sdk/frame-pacing) | Oficial | Cadencia estable y elección consciente de FPS |
| [Memoria en juegos Android](https://developer.android.com/games/optimize/memory-overview) | Oficial | Presupuesto y diagnóstico de memoria |

## Control de cambios y continuidad

| Fuente | Tipo | Aplicación |
|---|---|---|
| [Buenas prácticas de control de versiones en Unity](https://unity.com/how-to/version-control-systems) | Oficial | Cambios pequeños, frecuentes y trazables |
| [Unity Smart Merge](https://docs.unity3d.com/6000.0/Documentation/Manual/SmartMerge.html) | Oficial | Reducir conflictos en YAML serializado; no sustituye coordinación |

## Límites de esta investigación

- Varias páginas históricas usan versiones anteriores de Unity; se conservaron sólo ideas
  que siguen respaldadas por documentación vigente o que se presentan como casos.
- Los foros aportan síntomas y soluciones candidatas; toda adopción requiere reproducir el
  problema en el proyecto y medir el resultado.
- El repositorio Graphics Test Framework está archivado; puede informar un método de
  comparación, pero no debe integrarse automáticamente como dependencia nueva.
