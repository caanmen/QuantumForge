# Inventario de carpetas

Este inventario permite saber qué existe sin abrir todos los documentos ni recorrer los
assets. Se consulta al iniciar o continuar un proyecto y después se eligen únicamente las
áreas relacionadas con la tarea.

| Carpeta | Qué contiene | Consultar cuando | No hacer por defecto |
|---|---|---|---|
| `00_INICIO` | Índice, autoridad, enrutador y entrada al sistema | Siempre que se active la carpeta | Saltar directamente a guías profundas |
| `01_PRINCIPIOS` | Reglas generales de UI y producción transferibles | La tarea afecte estructura, claridad o criterios generales | Releer para una corrección trivial ya cubierta por reglas del proyecto |
| `02_FLUJO_DE_TRABAJO` | Creación, reducción de iteraciones y uso de agentes | Se vaya a planear un bloque, una pantalla o trabajo paralelo | Usar agentes por rutina en tareas pequeñas |
| `03_GUIAS_TECNICAS` | Módulos de código, tecnología y plataforma | Coincidan arquitectura, motor, plataforma o riesgo técnico | Aplicar módulos incompatibles o releerlos para una consulta trivial |
| `04_PLANTILLAS` | Fichas, contratos, estado y decisiones | Haga falta registrar o producir un entregable repetible | Obligar al usuario a llenar formatos; el asistente los completa desde lenguaje natural |
| `05_QA` | Puertas, planes y validadores ejecutables | Se produzca, integre, entregue o cierre trabajo sustancial | Ejecutar todas las pruebas existentes sin revisar aplicabilidad |
| `06_CATALOGO_APRENDIZAJES` | Reglas depuradas, errores y antipatrones | El problema o la tarea pueda coincidir con un aprendizaje previo | Leer el catálogo completo en cada chat |
| `07_CASOS_HISTORICOS` | Origen y evidencia de aprendizajes | Una regla aplicable enlace un caso o haga falta entender su causa | Copiar estética, valores o solución completa de otro proyecto |
| `08_INVESTIGACION_Y_FUENTES` | Procedencia y vigencia de consejos externos | Haya que verificar una regla técnica o actualizar información cambiante | Tratar una fuente histórica como confirmación vigente |
| `09_PROYECTOS` | Decisiones, perfiles, referencias, capturas y pruebas específicas | Se trabaje en uno de los proyectos registrados | Leer otros proyectos o recorrer todas las imágenes y vídeos |
| `10_SKILL` | Fuente canónica de la puerta de aceptación | Se mantenga o audite la skill | Editar manualmente la copia instalada |

## Inventario de un proyecto registrado

La carpeta de cada proyecto puede contener sólo las áreas que necesite. En Quantum Forge:

- `00_INICIO`: instrucciones y mapa específico.
- `01_GUIAS`: creación, prevención e integración gráfica.
- `02_PLANTILLAS`: fichas y contratos propios.
- `03_ESTILOS`: contrato común y perfiles por dimensión.
- `04_CATALOGO_ASSETS`: nombres, origen y rutas canónicas.
- `05_REFERENCIAS`: referencias permanentes; no son capturas runtime aprobadas.
- `06_CAPTURAS_APROBADAS`: evidencia visual aceptada.
- `07_REGISTROS`: estado, decisiones y bitácoras.
- `08_PRUEBAS`: matrices y criterios específicos.
- `09_HERRAMIENTAS`: documentación y salidas de herramientas.
- `10_PANTALLAS`: ficha, contrato y decisiones de cada pantalla.
- `11_CODIGO`: mapa de propietarios, riesgos, validadores y evolución técnica segura.

No se enumeran todos los archivos internos. El enrutador busca por nombre de proyecto,
pantalla, sistema, tecnología y tipo de tarea antes de abrir documentos.
