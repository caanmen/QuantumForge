# Recomendaciones oficiales de Unity aplicadas a Quantum Forge

Consulta realizada el 18 de agosto de 2026. Este documento resume recomendaciones
de Unity que complementan las guías internas; las secciones 46 a 53 de la guía de
creación contienen la versión operativa para el proyecto.

## Alcance

Estas recomendaciones son técnicas y generales, pero se aplican según mediciones de
cada pantalla. No definen paleta, composición, iconografía ni identidad visual. Que una
optimización funcione en D1 no autoriza a copiar su jerarquía exacta en otra dimensión.

## Recomendaciones adoptadas

1. Separar Canvas según frecuencia de cambio, equilibrando reconstrucciones y lotes.
2. Desactivar Raycast Target en gráficos no interactivos y retirar Graphic Raycaster
   de Canvas sin interacción.
3. Evitar Layout Groups anidados en composiciones fijas y medir antes de retirarlos.
4. Usar Preset Manager para configuraciones de importación repetibles y filtradas.
5. Agrupar sprites finales que aparecen juntos mediante Sprite Atlas.
6. Crear Prefab Variants para diferencias controladas, no para copiar pantallas enteras.
7. Combinar Device Simulator, pruebas Edit/Play Mode y teléfono Android real.
8. Guardar una línea base del Profiler para verificar que una optimización sí mejora.

## Aplicación gradual

- Desde la próxima pantalla: raycasts mínimos, presets, Device Simulator y línea base.
- Cuando existan al menos dos componentes aprobados equivalentes: prefab base y variante.
- Cuando una familia de sprites finales sea estable: Sprite Atlas correspondiente.
- Si el Profiler detecta reconstrucciones costosas: dividir Canvas estático y dinámico.

No migrar toda la interfaz de golpe ni reemplazar el sistema UGUI actual. Estas mejoras
se aplican por pantalla y se validan antes de convertirlas en estándar global.

## Fuentes oficiales

- Unity UI optimization tips:
  https://unity.com/how-to/unity-ui-optimization-tips
- Optimizing Unity UI:
  https://learn.unity.com/course/introduction-to-ui-in-unity/tutorial/optimizing-unity-ui
- Sprite Atlas, Unity 6:
  https://docs.unity3d.com/6000.0/Manual/sprite/atlas/atlas-landing.html
- Prefab Variants, Unity 6:
  https://docs.unity3d.com/6000.0/Manual/PrefabVariants.html
- Preset Manager, Unity 6:
  https://docs.unity3d.com/6000.0/Manual/class-PresetManager.html
- Device Simulator introduction:
  https://docs.unity3d.com/2022.3/Manual/device-simulator-introduction.html
- Edit Mode y Play Mode tests:
  https://docs.unity3d.com/Packages/com.unity.test-framework@2.0/manual/edit-mode-vs-play-mode-tests.html
- Profiler navigation, Unity 6:
  https://docs.unity3d.com/6000.0/Manual/profiler-window-navigating.html
