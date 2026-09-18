# Mapa de uso

Empieza aquí y sigue sólo la ruta relacionada con la tarea. El orden de autoridad y la
responsabilidad de cada capa están en `00_INICIO/FUENTES_DE_INSTRUCCIONES.md`.

Antes de elegir una ruta, leer `00_INICIO/INVENTARIO_DE_CARPETAS.md` y
`00_INICIO/ENRUTADOR_DE_PROYECTOS_Y_TAREAS.md`. No recorrer el árbol completo.

## Interpretar una petición natural

1. El usuario puede dar nombre, tipo de proyecto, etapa y objetivo sin conocer documentos.
2. El asistente clasifica la tarea, busca rutas por palabras clave y selecciona las guías.
3. El asistente informa brevemente qué seleccionó y por qué.
4. «Revisa» significa inspeccionar; «corrige» o «implementa» autoriza cambios dentro del alcance.

## Adoptar el sistema en un proyecto nuevo

1. Clasificar el tipo de proyecto con el enrutador.
2. Inspeccionar su carpeta real o definir dónde vivirá.
3. Elegir módulos existentes y declarar cualquier cobertura todavía ausente.
4. Crear `09_PROYECTOS/NOMBRE_DEL_PROYECTO` sólo cuando se necesite autoridad documental compartida.
5. Registrar diseño, estado y políticas más estrictas que el núcleo.
6. Si es videojuego o UI, continuar con `00_INICIO/COMO_INICIAR_UN_JUEGO_NUEVO.md` y las plantillas visuales.

Cuando el juego use colecciones de elementos repetidos o temas visuales, adoptar también
`03_GUIAS_TECNICAS/COMUN/GUIA_FAMILIAS_VISUALES_MODULARES.md`.

## Crear, corregir o refactorizar código

1. Leer `03_GUIAS_TECNICAS/CODIGO/GUIA_DESARROLLO_Y_CALIDAD_CODIGO.md`.
2. Inspeccionar las instrucciones, el código real y el estado de trabajo del proyecto.
3. Localizar el propietario actual, consumidores, datos persistentes y validadores.
4. Evaluar sólo los impactos aplicables: persistencia, ciclo de vida, integración,
   compatibilidad, localización, rendimiento, herramientas y validación.
5. Corregir la fuente definitiva mediante el bloque coherente más pequeño comprobable.
6. Ejecutar prueba dirigida y regresión vecina; ampliar cobertura según el riesgo.
7. Convertir todo fallo repetido en una protección automática y registrar únicamente el
   aprendizaje transferible.

Para Quantum Forge, consultar además
`09_PROYECTOS/QUANTUM_FORGE/11_CODIGO/MAPA_CODIGO_Y_VALIDACION.md` y seleccionar pruebas
desde `09_PROYECTOS/QUANTUM_FORGE/11_CODIGO/INDICE_VALIDADORES_CODIGO.md`.

## Crear una pantalla

1. Leer el perfil del proyecto y de su familia visual.
2. Completar FICHA_PANTALLA.txt y CONTRATO_APROBACION.txt.
3. Elegir Ruta A, B o C según el riesgo principal.
4. Auditar recursos y componentes existentes.
5. Construir, comparar, conectar y validar siguiendo el flujo.
6. Archivar evidencia y aprendizajes en el sistema específico del proyecto.

## Reducir iteraciones sin bajar calidad

1. Leer `02_FLUJO_DE_TRABAJO/GUIA_REDUCIR_ITERACIONES.md`.
2. Resolver primero el riesgo que podría invalidar más trabajo.
3. Modificar una categoría por vez y producir evidencia comparable.
4. Automatizar sólo comprobaciones repetibles; conservar aprobación humana para claridad,
   composición, tacto y sensación.
5. Consultar `06_CATALOGO_APRENDIZAJES/ANTIPATRONES_Y_ERRORES.md` antes de repetir una
   solución que ya falló.

## Decidir si usar agentes

1. Leer `02_FLUJO_DE_TRABAJO/REGLAS_USO_AGENTES.md`.
2. Delegar sólo auditorías o subtareas independientes con ventaja real.
3. Nombrar un coordinador y un único escritor por archivo o sistema.
4. Mantener agentes auxiliares en lectura salvo alcance exclusivo.
5. Integrar y verificar localmente todos los hallazgos antes de cerrar.

## Corregir una pantalla existente

1. Identificar la fuente definitiva: escena, prefab, código, plantilla o generador.
2. Capturar el estado base y leer decisiones anteriores.
3. Localizar el propietario del fallo antes de modificar síntomas.
4. Corregir la fuente definitiva y cualquier reconstrucción asociada.
5. Ejecutar regresión proporcional al riesgo.
6. Invalidar y renovar evidencia si cambió la salida visible o funcional.
7. En transiciones, comprobar inicio, 25 %, 50 %, 75 % y final.

## Procesar una tanda de hallazgos de prueba

1. Aceptar la lista numerada natural del usuario con pantalla o sistema, problema y evidencia disponible; no exigir tablas.
2. El asistente clasifica severidad, dependencia y propietario; separa defecto, balance, decisión de diseño y preferencia visual.
3. Agrupar en bloques pequeños relacionados, empezando por bloqueos, pérdida de datos y causas compartidas.
4. Corregir un bloque y ejecutar regresión dirigida antes de abrir el siguiente.
5. Convertir un fallo repetido en prueba, aserción o validador bloqueante.
6. Mantener el resto de la lista visible; no declarar terminado el lote por cerrar sólo un bloque.

Para Quantum Forge, combinar esta ruta con
`09_PROYECTOS/QUANTUM_FORGE/08_PRUEBAS` y las reglas del repositorio.

## Corregir un fallo funcional

1. Reproducirlo o reunir evidencia suficiente.
2. Identificar la fuente definitiva y sus dependencias.
3. Corregir la causa, no sólo el síntoma.
4. Ejecutar pruebas dirigidas y una regresión del sistema vecino.
5. Actualizar el estado y registrar cualquier decisión nueva.

## Auditar guardado, progreso o migraciones

1. Consultar el diseño canónico y el propietario real del estado.
2. Probar serialización, cierre y reapertura o carga desde una copia independiente.
3. Separar partida nueva, partida antigua y estado preparado mediante DEBUG.
4. Verificar compatibilidad, recuperación ante fallo y ausencia de escrituras inesperadas.
5. No usar un estado DEBUG como prueba de progresión natural o balance.

## Preparar un build o una entrega

1. Confirmar alcance, versión, plataforma y artefactos esperados.
2. Aplicar `puerta-aceptacion` y pasar `preflight` antes de producir un lote.
3. Validar una unidad representativa si existe producción repetida o costosa.
4. Construir, instalar o publicar sólo lo autorizado por el usuario.
5. Ejecutar `final` después del último cambio relevante y conservar la evidencia.

## Cerrar un hito o cambiar de chat

1. Actualizar el estado del proyecto y el registro de decisiones, si existen.
2. Resumir cambios, pruebas, pendientes, riesgos y siguiente paso verificable.
3. Enlazar las reglas y documentos canónicos por ruta; no copiarlos completos.
4. Usar `04_PLANTILLAS/ESTADO_PROYECTO.md` y `04_PLANTILLAS/REGISTRO_DECISIONES.md` cuando el proyecto no tenga formatos propios.
5. Ejecutar la validación final aplicable antes de describir el hito como terminado.

Usar `00_INICIO/COMO_CONTINUAR_EN_OTRO_CHAT.md`. En el chat nuevo, «revisar la carpeta»
activa el enrutador y no una lectura completa.

## Registrar un aprendizaje

1. Conservar el caso original dentro de su proyecto.
2. Extraer una regla sin nombres, estética ni valores del caso.
3. Indicar alcance: general, motor, plataforma o tipo de pantalla.
4. Añadir prevención y prueba reproducible.
5. Vincular el ID general con el caso histórico y la fuente externa cuando exista, sin
   convertirlos en autoridad creativa.

## Consultar errores, guías y consejos anteriores

1. Buscar en `06_CATALOGO_APRENDIZAJES` por sistema, síntoma, tecnología y tipo de tarea.
2. Leer sólo las reglas coincidentes y sus pruebas mínimas.
3. Abrir un caso de `07_CASOS_HISTORICOS` únicamente si la regla lo enlaza o falta comprender la causa.
4. Abrir `08_INVESTIGACION_Y_FUENTES` sólo para comprobar procedencia o vigencia.
5. Si el mismo fallo reaparece, convertirlo en prueba, aserción o validador bloqueante.
