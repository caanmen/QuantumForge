# Plan maestro general de QA UI

Este plan amplía `PUERTA_OBLIGATORIA_ACEPTACION.md` para entregables de UI y se instancia por
pantalla. Sólo se ejecutan módulos compatibles con la ficha del proyecto, pero cada
exclusión debe tener una razón registrada. Sus resultados alimentan el registro universal;
esta lista por sí sola no autoriza el cierre.

## Preflight

- [ ] Proyecto, pantalla, versión y fuente definitiva identificados.
- [ ] Diseño, perfil, referencia y contrato vigentes localizados.
- [ ] Escenario, idioma, resolución, entrada y plataforma registrados.
- [ ] Evidencia anterior identificada como base, no como resultado nuevo.
- [ ] Datos personales y persistencia protegidos antes de inyectar estados QA.
- [ ] Preflight del motor o plataforma completado cuando corresponda.
- [ ] En Unity batch no existe ningún `Unity.exe`, prueba antigua de Hub ni
      `Temp/UnityLockfile`; no se termina automáticamente un proceso ajeno.
- [ ] Licencia, cachés, `Library`, `Temp`, logs y resultados tienen escritura normal antes
      de atribuir un fallo a código o pruebas.

## Estructura

- [ ] Objetos, componentes y referencias esenciales presentes.
- [ ] Un único propietario por dato y propiedad dinámica.
- [ ] Jerarquía, orden de capas y zonas seguras válidos.
- [ ] Una única fuente o constructor canónico por raíz.
- [ ] Reconstruir o reabrir no pierde conexiones ni restaura valores antiguos.

## Visual

- [ ] Resoluciones y relaciones de aspecto del contrato.
- [ ] En productos existentes, una captura vigente identifica el shell común antes de producir el concepto.
- [ ] Cabecera, barra inferior, acciones, orden, posiciones, navegación y lenguaje de botones permanecen iguales salvo autorización explícita.
- [ ] La matriz de capas separa shell inmutable, componentes universales y capas tematizables.
- [ ] La comparación antes/después bloquea cualquier cambio del shell que no esté en el alcance aprobado.
- [ ] Todo maestro universal existente se reutiliza por ruta, estado y hash; no se aceptan placeholders ni reinterpretaciones en el mockup.
- [ ] Grandes bloques, centros, márgenes y ocupación interior comparados.
- [ ] Proporciones, alfa, silueta y medio gráfico conservados.
- [ ] Tipografía legible con idioma y valores extremos.
- [ ] Estados comparados y diferencias autorizadas registradas.
- [ ] Captura producida después del último cambio relevante.
- [ ] La captura tiene resolución, variación tonal, entropía y color suficientes; un PNG
      existente pero gris, vacío o de baja información se rechaza automáticamente.
- [ ] La evidencia visual demuestra por regiones las capas exigidas —fondo aprobado, shell
      común y contenido principal—; tamaño de archivo y fecha no prueban render válido.

## Familias de assets repetidos

- [ ] El contenido universal existente y sus casos extremos fueron medidos a escala final antes de producir el marco.
- [ ] Una prueba geométrica limpia bloqueó campo útil, zonas reservadas, márgenes y máscaras antes del arte decorativo.
- [ ] El marco envuelve la geometría aprobada; ningún contenido universal fue reducido, deformado o desplazado para hacerlo caber.
- [ ] Cada archivo declara si es elemento completo, capa interior o composición modular.
- [ ] El juego no añade una segunda base o marco a un asset que ya los contiene.
- [ ] La composición declara una sola superficie propietaria por región visible: fondo
      temático, contenido universal y marco no pueden aportar bases opacas duplicadas.
- [ ] Las capas superiores son transparentes fuera de su contenido autorizado; el contenido
      universal no lleva una cara completa opaca y el marco no rellena el campo interior.
- [ ] El render final se recompone de forma determinista desde las capas aisladas y una
      diferencia de píxeles confirma que no existen parches, dobles fondos ni orden distinto.
- [ ] La visibilidad de la capa temática se mide dentro del campo útil en una zona sin tinta;
      una fixture con cara completa opaca encima debe fallar aunque el maestro y su hash sean correctos.
- [ ] Las variantes de una familia comparten plantilla, proporción y geometría aprobadas.
- [ ] Las zonas reservadas coinciden dentro de la familia o la excepción está registrada.
- [ ] Familias con marcos diferentes usan perfiles de colocación propios.
- [ ] El perfil se selecciona por estructura del bloque (`rango solo`, `rango + palo`, etc.) y no por comparar un valor literal aislado.
- [ ] Glifos anchos y estrechos de una misma clase comparten zona base y pasan una aserción de dimensiones y centro antes de guardar.
- [ ] El centro visible del índice se compara con el hueco real del marco mediante una fuente independiente de las coordenadas de generación.
- [ ] La ocupación vertical visible del bloque `rango + palo` no supera el máximo declarado por su perfil; en las barajas compactas vigentes de Solitario es `60 %`.
- [ ] La separación interna visible entre rango y palo se mide entre sus huellas, no sólo entre anclas; en una carta de `220 × 348 px` deja al menos `4 px` arriba y abajo.
- [ ] A–10 y J/Q/K pasan individualmente la separación mínima; el índice inferior conserva el mismo espacio tras su rotación de `180 grados`.
- [ ] Una fixture con rango y palo demasiado separados supera ese máximo y detiene el proceso antes de guardar.
- [ ] Una fixture con rango y palo pegados o solapados no puede pasar aunque el conjunto esté centrado y dentro del máximo de ocupación.
- [ ] Para grupos repetidos se comprueban el centro medio y la simetría de sus límites exteriores.
- [ ] Números y letras simples se probaron primero sin círculo, placa, cápsula ni fondo añadido.
- [ ] Todo contenedor de índice tiene una justificación visual o de contraste y no invade marco ni ornamentos.
- [ ] Ilustraciones, rostros y objetos conservan su proporción después de normalizar el lienzo.
- [ ] Una unidad representativa fue aprobada dentro del producto antes de producir el lote.
- [ ] Una cuadrícula al mismo tamaño permite comparar todas las variantes.
- [ ] Recortes ampliados comprueban marcos, zonas reservadas y bloques compuestos.
- [ ] Capturas reales confirman orden de capas, escalado, máscara y alineación a tamaño final.
- [ ] La fuente definitiva y el generador reproducen la colección aprobada sin deriva.
- [ ] Rangos, cifras, palabras, palos y cantidades proceden de una fuente determinista.
- [ ] Existe una tabla canónica de cantidad y posición para cada valor finito.
- [ ] La tabla canónica declara también la firma de filas o columnas, separación mínima y simetría del patrón.
- [ ] La matriz de barajas incluye As, `2`, `3`, `4`, `5`, `6`, `7`, `8`, `9` y `10` para los cuatro palos, sin limitar la prueba a las cartas mostradas en la propuesta.
- [ ] Cada valor coincide con `04_PLANTILLAS/PATRONES_CANONICOS_CARTAS_NUMERICAS.json`; el `10` clásico presenta dos mitades `2-1-2` con firma `2-1-2-2-1-2`.
- [ ] La orientación se valida por fila; el patrón inglés usa repartos de `1/1` a `5/5`, el `7` usa `5/2` y una fixture con orientación incorrecta queda bloqueada.
- [ ] El conteo central excluye o incluye el símbolo del índice según una regla explícita.
- [ ] Cada variante tiene cantidad esperada, cantidad producida y PASS individual.
- [ ] La cantidad producida se extrae del render; no se copia desde la cantidad esperada ni se autoasigna PASS.
- [ ] Una fixture centrada pasa y fixtures desplazadas, faltantes y adicionales fallan antes de guardar el lote.
- [ ] Para cada valor, fixtures con conteo y centro correctos pero topología incorrecta, separación insuficiente u orientación errónea también fallan.
- [ ] La ocupación horizontal visible se mide por tema contra el campo útil del marco; una fixture centrada pero demasiado estrecha falla.
- [ ] En cada índice, la huella visible de Q/K/10 y demás rangos anchos se centra en el hueco y no supera su ocupación horizontal máxima; la fixture sin aire lateral falla.
- [ ] En temas nuevos o modificados, cada palo de índice conserva una altura visible mínima del 8.4 % del ancho final de la carta; reducirlo para rescatar el marco falla.
- [ ] Los huecos superior e inferior se miden sobre el arte renderizado, coinciden tras rotación de 180 grados y presentan una diferencia máxima de `1 px` en ancho y alto.
- [ ] Cada índice A–10/J/Q/K se captura para los cuatro palos en capas `full`, `rank_only`, `suit_only` y `clean`; el bloque completo rango+palo pasa centrado horizontal y vertical, eje común, separación, contención y equivalencia de 180 grados en ambas esquinas. No se admite `N/A`.
- [ ] Los conteos se validaron automáticamente y se confirmaron visualmente al 100 %.
- [ ] Arte e índice tienen propietarios únicos; no hay información horneada y dinámica duplicada.
- [ ] No hay cartas completas, marcos ni fondos colocados como parches sobre otra cara completa.
- [ ] Píxeles con alfa cero tienen RGB neutralizado cuando la capa será filtrada, reescalada
      o exportada, para impedir cuadrículas, halos o basura oculta en los bordes.
- [ ] Las uniones retocadas no muestran rectángulos, halos, costuras ni cambios bruscos de textura.
- [ ] La lámina de contacto se ensambló desde assets aprobados sin volver a generar su contenido.
- [ ] Cada celda coincide con su contrato de tema, tipo, rango, palo, orientación y cantidad.

## Estados y datos

- [ ] Neutral o primera apertura.
- [ ] Normal y seleccionado.
- [ ] Bloqueado o deshabilitado con motivo.
- [ ] Vacío, carga, completado y error cuando existan.
- [ ] Valores mínimos, máximos, largos y cardinalidad real completa.
- [ ] Cada selección sincroniza ID, texto, arte, material, estadísticas y acción.
- [ ] Un cambio de ámbito invalida o filtra selecciones anteriores.

## Interacción y navegación

- [ ] Entrada desde la pantalla anterior mediante el control real.
- [ ] Acción principal y al menos una segunda acción interna.
- [ ] Respuesta inmediata y estado posterior a refrescos o animación.
- [ ] Entradas rápidas, repetidas y simultáneas cuando sean posibles.
- [ ] Atrás, cancelar, cerrar, regresar y reabrir.
- [ ] Elementos ocultos o decorativos no reciben eventos.
- [ ] Modales bloquean y restauran únicamente lo previsto.
- [ ] Controles indispensables son visibles, alcanzables y descubribles.

## Animación

- [ ] La capa y el movimiento coinciden con el contrato.
- [ ] Pivote, base y elementos inmóviles permanecen estables.
- [ ] Evidencia renderizada demuestra un cambio perceptible.
- [ ] Texto, layout e interacción conservan estabilidad.
- [ ] Se detiene o reduce cuando la pantalla no está visible o la calidad lo exige.
- [ ] Transiciones capturadas a 0 %, 25 %, 50 %, 75 % y 100 %.
- [ ] No existe un instante donde origen, destino y cobertura sean insuficientes a la vez.
- [ ] La capa saliente deja de bloquear raycasts y se desactiva al terminar.

## Integración y persistencia

- [ ] Reglas y datos proceden de la autoridad funcional real.
- [ ] La acción persistente guarda o transacciona según contrato.
- [ ] Cerrar, cargar y migrar conserva el estado esperado.
- [ ] Estados QA no contaminan el progreso personal.
- [ ] Entrada, acción, salida y regreso funcionan dentro del flujo completo.

## Accesibilidad

- [ ] Navegación y foco adecuados para cada método de entrada.
- [ ] Etiquetas y lectura accesible sincronizadas con el estado visual.
- [ ] Contraste, tamaño, movimiento reducido y alternativas definidos cuando apliquen.
- [ ] La información no depende exclusivamente del color o de un efecto breve.

## Rendimiento y entrega

- [ ] Escenario más costoso comparado con una línea base equivalente.
- [ ] Presupuestos del proyecto respetados o diferencias aprobadas.
- [ ] Crecimiento de build y dependencias explicado cuando corresponda.
- [ ] Assets no utilizados, conceptos y capturas excluidos de producción.
- [ ] Las dimensiones cargadas por el runtime coinciden con las fuentes aprobadas; texturas
      NPOT no se redimensionan automáticamente ni alteran su relación de aspecto.
- [ ] Zonas de toque, arrastre o caída superpuestas a un fondo continuo conservan la
      interacción sin añadir superficies opacas no autorizadas.
- [ ] Todo viewport transparente muestra realmente sus hijos: no basta con que existan o
      reciban clics; validar recorte, `CanvasRenderer.cull`, desplazamiento y captura runtime.
- [ ] Cuando un aspecto combina identificador básico y componente de tema, `EQUIPADO`, vista
      previa, partida y guardado coinciden con el mismo propietario visual efectivo.
- [ ] La regresión de propietario cosmético se ejecuta en las once modalidades: tras equipar
      un tema premium completo, el básico de dorso o frente devuelve sólo cartas/reverso a
      clásico y el básico de superficie devuelve sólo el tapete a clásico; las piezas no
      relacionadas conservan su tema premium.
- [ ] El validador transversal de propietarios cosméticos devuelve `PASS (11/11)` después
      del último cambio en tiendas, progresión o presentadores de Solitario.
- [ ] Entrega identificada mediante versión, fecha, tamaño y hash cuando aplique.
- [ ] La medición final procede de la plataforma objetivo, no sólo del Editor.
- [ ] Una prueba sostenida comprueba calor, cadencia y memoria cuando sea relevante.

## Cierre

- [ ] PASS estructural.
- [ ] PASS visual.
- [ ] PASS de interacción real.
- [ ] PASS de recorrido jugable.
- [ ] PASS de plataforma o dispositivo cuando corresponda.
- [ ] Fidelidad aprobada según el método del proyecto.
- [ ] Evidencia archivada y diferencias restantes registradas.
- [ ] Ficha, matriz de cobertura, registro global y archivo de capturas declaran el mismo
      estado final, sin candidatos o aprobaciones pendientes obsoletos.
- [ ] Si el alcance contiene varias modalidades o unidades, una matriz enumera todas y
      separa jugabilidad, contenido base, contenido desbloqueable, tienda, persistencia,
      pruebas y puerta final; ningún subtotal autoriza el cierre global.
- [ ] Al reanudar después de una pausa se verifican fecha, hash, procesos y bloqueo del
      proyecto; las pruebas costosas sólo se reutilizan si sus fuentes siguen idénticas.
- [ ] Aprendizajes generales separados de decisiones específicas.

No combinar los PASS en uno solo si las pruebas no cubren las mismas capacidades.
El cierre requiere además que el validador universal devuelva `GATE PASS [final]`.

## Defensa del maestro universal A–10

- [ ] El maestro está en estado `LOCKED` y cada tema referencia su hash aprobado.
- [ ] Los centros de índices, posiciones de símbolos y tamaños visibles coinciden exactamente
      con `04_PLANTILLAS/CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json`.
- [ ] No existe traslación ni escala por tema para resolver conflictos con el marco.
- [ ] Las tres zonas limpias del marco contienen por completo las envolventes universales.
- [ ] Una superposición del arte confirma cero intrusiones dentro de las zonas reservadas.
- [ ] Las fixtures de contenido trasladado, reducido, marco estrecho y hash no aprobado fallan.

Si alguna prueba falla, se modifica el marco y se repite su evidencia; no se modifica el
maestro universal aprobado.
