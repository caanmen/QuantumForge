# Fuentes de instrucciones y responsabilidad

Este documento evita que una misma regla se copie en varios lugares y termine divergiendo.

## Orden de autoridad

De mayor a menor prioridad:

1. Instrucción actual y explícita del usuario.
2. Diseño o decisión canónica aprobada para el proyecto.
3. `AGENTS.md` del repositorio o carpeta activa.
4. Documentación específica del proyecto dentro de `09_PROYECTOS`.
5. Este sistema general y sus módulos técnicos aplicables.
6. Resúmenes, notas históricas y conversaciones anteriores.

Los archivos y la evidencia real determinan el estado técnico actual. Si dos fuentes se
contradicen, se informa y se resuelve la contradicción antes de modificar la parte afectada.

## Responsabilidad de cada capa

- Personalización global (`C:\Users\nedfla\.codex\AGENTS.md`): idioma, forma de colaborar y reglas de enrutamiento comunes. Debe ser breve.
- `AGENTS.md` del proyecto: restricciones persistentes que aplican siempre en ese repositorio.
- `README.md`, inventario, enrutador y `MAPA_DE_USO.md`: entrada, conocimiento de la estructura y selección de la ruta; no obligan a leer todo el sistema.
- Guías y módulos: procedimiento detallado que se consulta sólo cuando la tarea lo requiere.
- `09_PROYECTOS`: diseño, estilo, activos, decisiones y evidencia propios de cada juego.
- Skill `puerta-aceptacion`: flujo ejecutable y criterios universales de aceptación.
- Estado y decisiones: información cambiante para continuar entre sesiones sin copiar reglas estables.
- Resumen de chat: delta reciente, pruebas, pendientes y siguiente paso; nunca sustituye los archivos reales.

## Interpretación de la petición del usuario

- «Revisa la carpeta de producción» activa inventario, enrutador y mapa; no autoriza una lectura completa del árbol.
- El usuario indica proyecto, objetivo, etapa y problemas en lenguaje natural. No tiene que conocer rutas ni plantillas.
- El asistente selecciona, registra y comunica las fuentes aplicables, y justifica sólo las exclusiones importantes.
- Conocer que una carpeta existe no equivale a adoptar su contenido. Una guía, caso o fuente se abre únicamente por coincidencia con la tarea.

## Regla contra duplicación y deriva

- Cada procedimiento tiene una sola fuente canónica.
- Las demás capas enlazan esa fuente y sólo añaden restricciones propias.
- La skill canónica vive en `10_SKILL/puerta-aceptacion`; la carpeta instalada es un despliegue sincronizado.
- Una regla nueva se registra primero en su propietario. No se replica completa en personalización, resúmenes o mapas.
- Si se detecta una divergencia, se conserva la fuente canónica, se corrigen las referencias y se añade una comprobación automática cuando sea razonable.

## Lectura mínima por tipo de tarea

- Pregunta general: ninguna guía de proyecto.
- Inicio o continuación de proyecto: inventario, enrutador, mapa, reglas y estado específico.
- Corrección pequeña: reglas del repositorio, archivos afectados y guía específica necesaria.
- Bloque amplio: reglas del repositorio, estado vigente, diseño canónico y módulos aplicables.
- Pantalla nueva o rediseño: instrucciones maestras, perfil visual, ficha, contrato y guías de creación.
- Lote, integración o cierre: lo anterior según alcance más `puerta-aceptacion`.
