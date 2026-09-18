# Flujo general para crear una pantalla

## 0. Contrato del proyecto

Confirmar motor, sistema UI, plataforma, entrada, orientación, resoluciones, perfil visual,
umbral de fidelidad, rendimiento y evidencia requerida.

## 1. Función

Definir objetivo, entrada, salida, acción principal, datos, consecuencias, persistencia y
estados. Si no cabe en una frase clara, la pantalla todavía no está lista para producirse.

## 2. Autoridades y referencias

Clasificar cada fuente como diseño funcional, referencia visual, concepto, asset final o
captura real. Resolver contradicciones y materiales ausentes antes de fijar decisiones.

## 3. Auditoría de reutilización

Crear una matriz por concepto:

    concepto | origen | ruta o ID | estado | alcance | decisión

Buscar en el proyecto real. Registrar faltantes antes de crear candidatos nuevos.

## 4. Elegir la ruta

- Ruta A: fidelidad dirigida por referencia.
- Ruta B: interacción desconocida.
- Ruta C: skin sobre sistema funcional existente.

Documentar por qué se eligió y qué puerta impide adelantar trabajo final.

## 5. Composición y sistema de diseño

Definir grandes bloques, cuadrícula, zonas seguras, escala tipográfica, contenido máximo,
componentes reutilizados y comportamiento responsivo. Medir primero el conjunto y después
el detalle.

## 6. Arquitectura y propietarios

Diseñar jerarquía y asignar propietarios para layout, datos, visuales, interacción,
navegación, animación y persistencia. Separar contenedores estables de capas animables.

## 7. Prototipo correspondiente

- Ruta A: composición estática completa con contenido real representativo.
- Ruta B: prototipo de interacción marcado como no final y sin crear estética canónica.
- Ruta C: inventario completo del controlador y adaptadores a su estado real.

## 8. Comparación y aprobación intermedia

Evaluar proporciones, ocupación interior, jerarquía, legibilidad, identidad y diferencias.
No pulir detalles mientras cambien estructura o flujo.

## 9. Datos, estados e interacción

Conectar fuentes reales, localización, formatos, motivos de bloqueo, respuesta inmediata,
toques repetidos, métodos de entrada y navegación. Los refrescos no reconstruyen el layout.

## 10. Animación

Planear capas y pivotes antes de producir arte final. Implementar movimiento sólo después de
la puerta definida por el proyecto. Validar píxeles renderizados y estabilidad del layout.

## 11. QA e integración

Probar estructura, estados, valores extremos, relaciones de aspecto, recorrido real,
persistencia, accesibilidad, rendimiento y plataforma. Separar PASS estructural, visual,
interactivo y jugable.

## 12. Evidencia y aprendizaje

Archivar capturas vigentes, versión, escenario, idioma, resolución, prueba y diferencias.
Registrar decisiones específicas en el proyecto y extraer sólo aprendizajes transferibles
al catálogo general. La aprobación final debe actualizar como una sola operación lógica la
ficha de pantalla, la matriz de cobertura, el registro global y el archivo de capturas; no
dejar una fuente marcada como candidata o pendiente cuando las demás ya declaran cierre.
