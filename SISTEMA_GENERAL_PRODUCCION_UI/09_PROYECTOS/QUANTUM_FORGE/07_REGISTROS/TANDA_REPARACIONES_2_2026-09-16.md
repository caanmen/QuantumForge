# Quantum Forge — decisiones de la tanda de reparaciones 2

Fecha: 2026-09-16
Alcance: inicio del juego hasta el desbloqueo de Dimensiones; no incluye el contenido interno de las dimensiones.

## Decisiones cerradas

1. Los nombres internos de edificios no pueden aparecer en Generación; Energía usa el mismo contrato visual de compra que LE y Trazas.
2. Los contadores superiores deben abreviar valores y conservarlos dentro de sus marcos.
3. En Mejoras, consola, minijuego y navegación no cambian de geometría al comprar, completar o revelar. Sólo se desplaza la lista de mejoras.
4. El estado completado usa marco y esquinas completamente verdes.
5. Revelar se superpone sobre la consola sin reducir su espacio.
6. El bloque del Triángulo baja sin estirarse; se conserva visible la parte superior del laboratorio.
7. Resonancia Experimental pasa a bonificación global total de Trazas de 13 % y 15 %, independiente del circuito y la sincronización. El circuito de Trazas conserva su efecto propio.
8. Las luces de acceso del Monolito viven dentro de alojamientos hundidos. El daño alrededor se repara, pero el alojamiento funcional permanece.
10. La entrada a una cara debe ser una sola transición elegante: sin doble cambio de escala, fotograma vacío ni aparición tardía de la ficha.
11. Mezclas no muestra puntos decorativos residuales ni caracteres `V` sobrantes.
12. La UI explica el origen y la cadencia actual de los fragmentos: Higgs/Condensación, Tetraquark/Confinamiento y Modulador/Interferencia residual; base de uno cada 30 segundos.
13. La inestabilidad baja no puede producir una frecuencia de fallos incoherente; se audita y corrige contra la probabilidad real.
14. Se conserva el orden físico: cara 1 superior izquierda, cara 2 superior derecha, cara 3 inferior derecha y cara 4 inferior izquierda. El defecto era que la cara 3 reutilizaba una geometría superior. Las caras 3 y 4 usan hojas inferiores propias, con borde superior plano y orientación complementaria; los overlays de símbolos no se reflejan.
15. Títulos de cara e índice quedan centrados en su marco propietario.
16. Prestigio temprano depende del 80 % de reparación, no de un símbolo concreto. El Canal de Convergencia se retira de esta etapa, deja de contar y no se menciona antes de completar las tres dimensiones.
17. La consola de Mejoras elimina la línea cian sobrante y contiene todo su texto.
18. El título MONOLITO queda centrado.

## Progresión de símbolos del Monolito

- Al retirar el Canal quedan 36 nodos públicos y el 80 % requiere 29 reparados.
- Las tres caras funcionales presentan ocho ramas cada una: 8/8/8.
- Cada etapa visual (0–19, 20–39, 40–59 y 60 % o más) posee su propia zona segura.
- Los símbolos iniciales ocupan la piedra disponible en el estado destruido.
- Las áreas recuperadas revelan símbolos adicionales.
- Un símbolo no cambia de posición después de aparecer.
- Ninguna firma ni su halo puede invadir daño, huecos, rieles, columna, base o borde.
- La distribución es irregular, sin filas ni patrones evidentes.
- Debe probarse matemáticamente que los nodos accesibles permiten alcanzar el siguiente umbral.

## Compatibilidad

El nodo retirado se conserva como definición histórica para reconocer partidas antiguas. Si estaba reparado, la migración debe excluirlo del progreso y devolver una vez su coste nominal, sin duplicar devoluciones.

## Resultado implementado y comprobado

- Generación ya oculta claves internas, abrevia los tres recursos y mantiene las filas de compra a ancho completo en 1080×1920 y 720×1280.
- Mejoras conserva una consola de 520 px fuera del desplazamiento; revelar se superpone sin cambiar la geometría y sólo se desplaza la lista.
- Resonancia Experimental aplica 13 % y 15 % global de Trazas sin depender del circuito activo, conservando aparte la bonificación del circuito.
- Mezclas muestra fuente y segundos por unidad de cada fragmento, explica por separado el riesgo de modo e inestabilidad y eliminó puntos y caracteres `V` residuales.
- Prestigio se habilita con 80 % de reparación. El Canal de Convergencia quedó retirado, migrado y fuera del progreso temprano.
- El Monolito usa umbrales 20/40/60, distribución fija progresiva 3/5/7/8 por cara, 8/8/8 nodos funcionales y luces estrechas dentro de las hendiduras.
- Se conserva el orden físico 1 superior izquierda, 2 superior derecha, 3 inferior derecha y 4 inferior izquierda. Las caras 3 y 4 dejaron de reutilizar geometría superior y usan hojas V55 propias con borde superior plano; los símbolos no se reflejan.
- La transición a una cara resuelve título, geometría y ficha dentro de una sola animación V58 de 1,12 s: 0,24 s de acercamiento frontal centrado y 0,88 s de cruce hacia la cara. Pasó sondas con fotogramas simulados de 33 y 67 ms.

## Correcciones posteriores a la APK 0.1.13 — 2026-09-17

- Generación conserva el Triángulo sin deformarlo, separa selectores y compras y extiende el
  laboratorio para cubrir el viewport sin franjas de cuadrícula en 1080×1920 y 720×1280.
- Mejoras mantiene centrado el núcleo del receptáculo. «Ocultar completados» usa un estado
  vacío intencional sin colapsar la consola, navegación ni geometría fija.
- La apertura cinematográfica interna de Prestigio queda omitida mientras el usuario prepara
  su video externo. Carrusel, regreso y confirmación triple permanecen operativos.
- La transición V58 anima únicamente `MonolithOverview` durante el acercamiento frontal. El
  laboratorio, pedestal y viewport permanecen estables; después se interpola el cambio de
  altura y se conservan opacidades complementarias sin fotograma vacío.
- La captura automatizada recalcula la geometría del CanvasScaler al cambiar de resolución y
  la restaura al terminar, evitando que la segunda captura quede recortada.
- Las regresiones integradas de Generación, Mejoras, Triángulo, Prestigio, guardado y carga
  finalizaron en PASS. No se generó APK y la revisión física en el teléfono sigue pendiente.

Evidencia final:

- `Logs/tanda2_validation_final.log`
- `Logs/tanda2_validation_closure.log`
- `Logs/tanda2_vertical_capture_final.log`
- `Logs/tanda2_upgrades_fixed_capture_final.log`
- `Logs/tanda2_fusion_capture_final.log`
- `Logs/tanda2_monolith_capture_final.log`
- `Logs/VisualQA/VerticalUIBlock7/05_generation_energy_focus_es_1080x1920.png`
- `Logs/VisualQA/VerticalUIBlock7/08_upgrades_polished_es_1080x1920.png`
- `Logs/VisualQA/MachineFusion/fusion_panel_operational_1080x1920.png`
- `Logs/VisualQA/MachineMonolith2D_V54/review_faces_1_2_3_damaged.png`
- `Logs/monolith_v55_capture_retry3.log`
- `Logs/VisualQA/MachineMonolith2D_V55/review_faces_1_2_3_damaged.png`
- `Logs/VisualQA/MachineMonolith2D_V55/review_faces_1_2_3_repaired.png`
- `Logs/post_0.1.13_final_vertical_capture.log`
- `Logs/post_0.1.13_final_upgrades_capture.log`
- `Logs/post_0.1.13_final_monolith_transition_validation.log`
- `Logs/post_0.1.13_final_qa_reset_validation.log`
- `Logs/post_0.1.13_final_prestige_transition_validation.log`
- `Logs/post_0.1.13_final_validate_only.log`
- `Logs/AcceptanceGates/correcciones_visuales_post_0.1.13_2026-09-17.json`

La aprobación creativa final de estas capturas corresponde al usuario; esta ronda acredita implementación y QA técnico, no sustituye esa aprobación visual.
