# Enrutador de proyectos y tareas

## Significado de «revisa la carpeta de producción»

Esa frase activa el sistema, pero **no significa leer la carpeta completa**. Significa:

1. Leer `README.md`, este enrutador, `INVENTARIO_DE_CARPETAS.md` y `MAPA_DE_USO.md`.
2. Identificar proyecto, tipo de trabajo, etapa, plataforma y riesgo principal.
3. Buscar por palabras clave en índices y nombres de archivo antes de abrir documentos.
4. Leer sólo las reglas del proyecto, guías, aprendizajes y pruebas aplicables.
5. Informar brevemente qué se seleccionó y por qué; no recitar el inventario completo.

No se recorren por defecto otros proyectos, referencias gráficas, capturas, vídeos, casos
históricos ni todos los validadores.

## Lo mínimo que aporta el usuario

El usuario puede expresarse de forma natural. Es suficiente con:

- Nombre del proyecto o indicar que es nuevo.
- Tipo aproximado: videojuego, aplicación, web, herramienta, documento, investigación u otro.
- Objetivo o etapa actual.
- Problema, lista numerada o resultado deseado.
- Evidencia disponible, si existe.

El usuario no necesita conocer nombres de guías, clasificar cada problema ni llenar tablas.
El asistente convierte esa información en alcance, rutas, dependencias y criterios.

## Algoritmo de selección

1. Aplicar primero la instrucción actual, la personalización y el `AGENTS.md` del proyecto.
2. Inspeccionar los archivos reales del proyecto y su estado vigente.
3. Clasificar la petición como inicio, continuación, auditoría, corrección, producción,
   validación, entrega o investigación.
4. Elegir las áreas usando el inventario y la matriz siguiente.
5. Buscar aprendizajes por el síntoma, sistema y tecnología; abrir sólo coincidencias.
6. Consultar casos históricos únicamente si una regla aplicable los enlaza o su causa aporta
   evidencia necesaria.
7. Comprobar vigencia en fuentes externas sólo cuando la información pueda haber cambiado.
8. Aplicar `puerta-aceptacion` cuando el alcance sea sustancial.
9. Registrar las guías seleccionadas y las exclusiones relevantes en el preflight o en una
   nota breve de trabajo.

## Selección por tipo de proyecto

| Tipo | Núcleo inicial | Módulos que pueden aplicar |
|---|---|---|
| Videojuego | Principios, flujo, QA y carpeta específica | UI, motor, móvil, persistencia, rendimiento, assets y builds |
| Aplicación móvil | Principios, flujo y QA | Móvil, UI, datos, privacidad, seguridad, persistencia y distribución |
| Página web | Principios, flujo y QA | UI, accesibilidad, datos, seguridad, rendimiento, hosting y despliegue |
| Herramienta de PC | Principios, flujo y QA | UI, archivos, persistencia, seguridad, empaquetado y distribución |
| Documento o PDF | Autoridad, plantillas y QA | Fuentes, exactitud, maquetación, render y accesibilidad |
| Investigación | Autoridad, fuentes y registro de decisiones | Vigencia, contraste de fuentes, incertidumbre y reproducibilidad |

La especialización más desarrollada actualmente es videojuegos, Unity y UI. Para otros
tipos se usa el núcleo común y sólo módulos realmente existentes; si falta una guía, se
declara la carencia y no se inventa que el sistema ya la cubre.

## Selección por tarea

| Tarea | Consultar |
|---|---|
| Iniciar proyecto | Autoridad, mapa, principios, flujo, módulos compatibles y QA |
| Continuar en otro chat | Estado y decisiones del proyecto, resumen compacto y esta ruta |
| Corregir problema pequeño | Archivos afectados, decisiones locales, aprendizaje coincidente y prueba dirigida |
| Procesar lista de hallazgos | Lista numerada, fase actual, evidencias, catálogo por coincidencias y pruebas del proyecto |
| Crear o modificar código | Guía de calidad de código, propietario real, consumidores, matriz de impacto y pruebas dirigidas |
| Refactorizar código heredado | Fuente real, fachada compatible, riesgos, unidad representativa y plan por rebanadas |
| Error técnico repetido | Caso vigente, causa, aprendizaje coincidente y nueva aserción, prueba o validador |
| Crear o rediseñar UI | Instrucciones maestras, perfil visual, ficha, contrato, referencias y guías de UI |
| Guardado o migración | Diseño canónico, propietario del estado, compatibilidad y pruebas de reapertura |
| Build o publicación | Plataforma, versión, firma, puerta, instalación o despliegue autorizado |
| Investigar una opción actual | Fuentes primarias vigentes y fecha de consulta |

## Lectura y acción

- «Mira», «revisa», «audita» o «dime qué ves» autorizan inspección e informe, no cambios.
- «Corrige», «implementa», «haz» o una autorización posterior permiten actuar dentro del
  alcance indicado.
- Una lista numerada con pantalla o sistema, problema y captura es suficiente. El asistente
  clasifica criticidad, dependencias y tipo de defecto.
- Puede haber muchos bloques consecutivos en un chat; se mantiene un solo bloque activo a
  la vez para proteger integración y pruebas.
- Una mejora de buenas prácticas no autoriza una limpieza general: debe resolver un riesgo
  concreto, conservar compatibilidad y disponer de una comprobación proporcional.
