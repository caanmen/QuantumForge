# Klondike: cosmético básico marcado como equipado bajo un componente premium

Fecha: 2026-09-15  
Estado: corregido y convertido en pruebas bloqueantes.

## Incidente

La personalización indicaba `Clásico azul — EQUIPADO` y `Esmeralda clásico — EQUIPADO`,
pero la vista previa mostraba un dorso y un tapete marrones. Los botones y el guardado
contenían los identificadores básicos correctos; sin embargo, los campos de reverso y tapete
procedentes de un tema premium seguían siendo los propietarios visuales efectivos.

La vista previa sustituía únicamente el identificador básico candidato y conservaba el
`themeId` premium. La etiqueta `EQUIPADO` comparaba sólo el identificador básico. Al aplicar
un básico tampoco se devolvía el componente temático equivalente a la familia clásica.

## Defensa permanente

1. Un dorso básico seleccionado se previsualiza con el recurso de la familia clásica.
2. Un tapete básico seleccionado se previsualiza con el recurso de la familia clásica.
3. Un básico sólo aparece como `EQUIPADO` si su componente temático activo también es clásico.
4. Equipar un dorso básico restablece sólo `EquippedCardBackThemeId` a clásico.
5. Equipar un tapete básico restablece sólo `EquippedFeltThemeId` a clásico.
6. El otro componente conserva su propietario actual para permitir mezclas deliberadas.
7. Las pruebas construyen primero un estado premium real y después verifican preview,
   etiqueta, transición de estado y captura runtime.

## Auditoría transversal posterior

La revisión solicitada después del incidente encontró el mismo defecto estructural en las
diez modalidades restantes. Todas almacenaban una preferencia básica y un propietario
premium por componente, pero al equipar el básico sólo cambiaban la preferencia. La defensa
se extendió a los once servicios de progresión y a sus seis rutas de presentación: Klondike,
Spider, FreeCell, Pyramid, TriPeaks, Golf y el presentador compartido por Yukon, Canfield,
Forty Thieves, Scorpion y Russian Solitaire.

La validación transversal exige que cada modalidad libere únicamente el componente elegido,
preserve las demás piezas premium mezcladas y no marque un básico como `EQUIPADO` mientras
el propietario efectivo siga siendo premium. FreeCell usa el frente básico como dueño del
componente `Cards`; los otros diez usan el dorso básico como dueño del componente `Back`.
