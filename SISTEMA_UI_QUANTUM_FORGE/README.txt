SISTEMA UI — QUANTUM FORGE

Esta carpeta centraliza el método de producción de interfaces del juego.
No obliga a que todas las dimensiones tengan el mismo aspecto: comparte reglas,
pruebas y procesos, mientras cada dimensión conserva su propio perfil visual.

CLASIFICACIÓN DE ALCANCE

- REGLA GENERAL: se aplica a cualquier dimensión o pantalla cuando corresponda.
- EJEMPLO HISTÓRICO: registra dónde se descubrió una regla; no transfiere la estética
  ni el contenido de esa pantalla.
- DECISIÓN DE DIMENSIÓN: sólo se aplica al perfil visual indicado.
- DECISIÓN DE PANTALLA: sólo se aplica a la ficha o pantalla indicada.

La presencia de ejemplos de Dimensión 1 se debe a que allí se obtuvieron las primeras
evidencias aprobadas. Esos ejemplos no convierten su paleta, marcos, iconos, navegación
o lenguaje visual en reglas para Dimensión 2, 3, 4 o dimensiones futuras.

EMPEZAR SIEMPRE POR:
00_INICIO/INSTRUCCIONES_MAESTRAS_UI.md

CONTENIDO

00_INICIO
  Índice, reglas de uso y flujo obligatorio.

01_GUIAS
  Guía de creación correcta y guía de prevención de errores.

02_PLANTILLAS
  Fichas y contratos que deben completarse antes de implementar una pantalla o
  consolidar el perfil de una dimensión.

03_ESTILOS
  Contrato común y perfiles visuales independientes por dimensión.

04_CATALOGO_ASSETS
  Registro de conceptos, nombres oficiales y rutas de assets canónicos.

05_REFERENCIAS
  Referencias permanentes con su clasificación y estado. Pueden ser candidatas o
  aprobadas, pero nunca deben confundirse entre sí ni con capturas reales de Unity.

06_CAPTURAS_APROBADAS
  Capturas reales de Unity que superaron revisión.

07_REGISTROS
  Decisiones, estado de cada pantalla y bitácora de aprendizajes continuos.

08_PRUEBAS
  Matrices de estados, resoluciones y criterios de aprobación.

09_HERRAMIENTAS
  Documentación y salidas de herramientas generales de Unity.

10_PANTALLAS
  Ficha, contrato y decisiones específicas de cada pantalla.

Los scripts, prefabs y ScriptableObjects que Unity debe importar permanecen bajo
Assets/Project. Esta carpeta los registra y explica; no se duplican assets.
