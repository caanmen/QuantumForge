# Unity batch, captura inválida y cierre global incompleto

- Proyecto: SolitarioUnity.
- Fecha: 2026-09-18.
- Sistemas: ejecución batch de Unity, evidencia visual y cierre de la colección.

## Síntomas observados

1. Windows mostró una excepción de `Unity.Licensing.Client.exe` mientras quedaban procesos
   de Unity o una prueba antigua iniciada por Hub.
2. Una ejecución con perfil gráfico inadecuado produjo un PNG gris que existía en disco,
   pero no demostraba el fondo, el shell ni las cartas.
3. Se confundieron once modalidades jugables con once modalidades terminadas, aunque el
   inventario de `Assets/Resources/Free` sólo contenía dos temas en nueve modalidades.
4. Después de pausas largas, era posible reutilizar pruebas anteriores sin comprobar si
   habían cambiado fuentes, hashes, procesos o locks.

## Causas comprobadas

- Unity comparte licencia, cachés y servicios auxiliares entre proyectos; evitar sólo dos
  instancias sobre la misma carpeta no bastaba.
- `-noUpm`, `-quit` junto a `-runTests` y `-nographics` se habían tratado como flags
  universales, aunque alteran respectivamente paquetes, ciclo del Test Runner y render.
- La aceptación de capturas comprobaba existencia y fecha, pero no información visual.
- El estado se resumía con un único conteo y no mediante una matriz por modalidad y tipo de
  entrega.

## Soluciones aplicadas

- Se creó un preflight no destructivo que bloquea ante cualquier `Unity.exe`, Hub con
  `-runTests` o `Temp/UnityLockfile`.
- Se definieron perfiles de comando separados y se prohibieron las combinaciones inseguras.
- Se añadió un validador de resolución, entropía, variación tonal y presencia de color para
  rechazar capturas planas antes de su inspección visual.
- Se creó una matriz de once modalidades que separa jugabilidad, contenido clásico,
  gratuitos, premium, tienda, equipamiento, persistencia, pruebas y puerta final.
- Se formalizó la reanudación mediante fecha, hashes, procesos, lock y validadores rápidos.

## Reglas generales extraídas

- `UI-GEN-050`: exclusividad global de Unity batch.
- `UI-GEN-051`: perfil seguro de flags batch.
- `UI-GEN-052`: validez informativa de una captura.
- `UI-GEN-053`: matriz de completitud global.
- `UI-GEN-054`: reanudación sin deriva.

## Qué no debe heredarse

- Los once nombres, el objetivo de seis premium y los conteos `9/11` pertenecen a
  SolitarioUnity. Otros proyectos deben definir su propio inventario.
- Los umbrales predeterminados del validador de captura son una defensa mínima; cada
  producto puede exigir regiones o referencias visuales más estrictas.
- Un estado `PENDING` en la matriz no prueba un defecto funcional: sólo impide afirmar
  cierre sin evidencia vigente.

## Evidencia

- La primera defensa se aplicó desde el script de preflight propio de SolitarioUnity.
- `05_QA/Test-UnityBatchPreflight.ps1` conserva la versión reutilizable para otros proyectos.
- `05_QA/validate_capture_information.py --self-test` prueba una fixture plana y otra válida.
- `09_PROYECTOS/SOLITARIO/MATRIZ_COMPLETITUD_GLOBAL.json` registra el inventario fechado.
- `05_QA/validate_solitario_completion_matrix.py --structure-only` comprueba sus once filas.
