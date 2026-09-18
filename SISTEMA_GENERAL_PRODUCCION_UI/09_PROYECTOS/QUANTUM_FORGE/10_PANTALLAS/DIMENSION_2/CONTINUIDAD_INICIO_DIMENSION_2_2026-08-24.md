# Continuidad para iniciar Dimensión 2

Fecha: 24 de agosto de 2026.
Proyecto: `C:\Users\nedfla\Quantum Forge`.

## Regla de inicio

No comenzar desde cero ni asumir que los mockups ya están implementados. Primero se debe
auditar el código, la escena, los constructores y las referencias reales existentes.

Antes de modificar una pantalla:

1. Leer `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md` completo.
2. Leer las instrucciones maestras y las dos guías centrales de UI completas.
3. Leer el perfil candidato de Dimensión 2, este resumen, el contrato visual, el plan de
   capas y la matriz de veinte vistas.
4. Confirmar que ninguna instancia normal o batch usa esta misma carpeta de proyecto.
5. No usar Git para escribir salvo petición explícita del usuario.

## Autoridad visual vigente

- Colección candidata para vistas 02 a 18:
  `05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21`.
- Vista 01 vigente:
  `01_Mapa_De_Los_Pactos_Corregido_V5.png`.
- Vista 19 vigente:
  `19_Investigacion_Del_Ente_Corregida_V5.png`.
- Vista 20 vigente:
  `20_Pacto_Opcional_Con_El_Ente_Corregido_V5.png`.
- V2 y V3 son historial, no autoridad de construcción.
- Todas son conceptos estáticos, no capturas reales de Unity ni assets finales.
- El perfil de estilo sigue siendo candidato hasta aprobar la primera familia estática
  implementable y consolidar su evidencia.

Las veinte vistas representan cuatro pantallas base:

1. Mapa de los Pactos: vista 01.
2. Civilización 1 — Santuario: vistas 02 a 08.
3. Civilización 2 — Resistencia: vistas 09 a 15.
4. Civilización 3 — Arqueología y Ente: vistas 16 a 20.

Las vistas 15 y 20 son estados posteriores, no pestañas adicionales. La vista 17 es una
subvista de Arqueología/Excavar y no una ruta base independiente.

El Mapa de los Pactos muestra siempre los tres territorios. Ruinas Sepultadas se ve
bloqueada hasta que el Dominio total de Civilización 2 baja a 30% o menos. El controlador
actual aún oculta esa tercera tarjeta y debe ajustarse cuando se implemente el mapa real.

## Estado técnico existente que debe inspeccionarse

- Escena real del juego: `Assets/Project/Scenes/Main.unity`.
- Escena aislada de prototipo:
  `Assets/Project/Scenes/Dimension2PactPrototype.unity`.
- Controlador real principal:
  `Assets/Project/Scripts/UI/Dimension2PanelUI.cs`.
- Constructor que actualmente escribe Dimensión 2 en Main:
  `Assets/Project/Scripts/Editor/Dimension2Block1UISetup.cs`.
- Prototipo aislado que por contrato no modifica Main:
  `Assets/Project/Scripts/Editor/Dimension2PactPrototypeSetup.cs`.
- Controlador del prototipo:
  `Assets/Project/Scripts/UI/Dimension2PactPrototypeUI.cs`.
- Sistemas funcionales existentes: `Dimension2System`, `Dimension2State`, civilizaciones,
  altares, peregrinaciones, noviciado, ritos, pactos, resistencia, arqueología y Ente.
- Existen controladores UI específicos para varias secciones; deben auditarse antes de
  crear reemplazos.

El archivo `Dimension2Block1UISetup.cs` ya construye más que su nombre histórico de
«Block 1», incluyendo raíces para las tres civilizaciones y código legacy. Al iniciar el
nuevo trabajo se debe localizar cada escritura de `Dimension2Panel`, declarar un único
constructor canónico para la raíz real y mantener el prototipo completamente separado.

## Orden recomendado de trabajo

1. Auditar raíces, constructores, controladores y rutas reales de Dimensión 2.
2. Crear la matriz `concepto -> origen -> ruta -> estado -> decisión` de Puerta 0.
3. Confirmar con el usuario qué composición estática será la primera autoridad visual.
4. Completar ficha y decisiones de la primera pantalla base.
5. Descomponer el concepto en assets; ningún PNG de pantalla completa entra como fondo.
6. Construir y aprobar primero la composición estática a 1080×1920 y 720×1280.
7. Consolidar el perfil definitivo de Dimensión 2 desde esa evidencia aprobada.
8. Conectar datos y estados reales sin inventar nombres, recursos ni mecánicas.
9. Probar navegación física, botón Atrás, reapertura, estados dinámicos y raycasts.
10. Añadir animaciones únicamente después de aprobar estática e interacción.
11. Capturar evidencia nueva y sincronizar escena, constructor, ficha y decisiones.

## Riesgos que no deben repetirse

- No heredar estética, iconos ni navegación de Dimensión 1.
- No convertir veinte vistas en veinte raíces independientes.
- No permitir que un constructor histórico reconstruya la misma raíz de producción.
- No usar candidatos o referencias como assets finales sin descomposición y aprobación.
- No hornear textos, cifras, selección, bloqueo o progreso dentro de las ilustraciones.
- No validar botones llamando métodos directamente; usar GraphicRaycaster y Button real.
- No permitir que una subpantalla deje interactiva la interfaz situada detrás.
- No ejecutar QA con progreso ficticio sin mantener el guardado suprimido hasta cerrar Unity.

## Continuidad Android y peso

El contrato general está en:
`08_PRUEBAS/CONTRATO_CONTINUIDAD_ANDROID_QA.md`.

La optimización global de texturas se realizará después de terminar la parte gráfica, pero
todo asset nuevo debe mantenerse fuera de la build mientras sólo sea referencia o candidato.
La siguiente comparación de APK debe usar `0.1.4` como línea base documentada.

## Estado al cerrar esta continuidad

- No se modificaron escenas, prefabs ni conexiones de Dimensión 2.
- No se ejecutó Unity.
- No se utilizó Git.
- Se corrigió la documentación para señalar V4/V5 como colección candidata vigente.
- Sigue pendiente la revisión visual y aprobación explícita de la familia completa.
