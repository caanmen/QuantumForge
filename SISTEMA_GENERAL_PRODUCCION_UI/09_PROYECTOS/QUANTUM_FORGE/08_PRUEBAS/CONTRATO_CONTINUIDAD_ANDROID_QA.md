# Contrato de continuidad Android QA

Estado: vigente para todas las dimensiones y pantallas de Quantum Forge.
Actualizado: 10 de septiembre de 2026.

## Identidad que no debe cambiar

- Paquete Android: `com.nedfla.quantumforge`.
- Certificado SHA-256 autorizado para la línea QA instalada:
  `4D704230994E826E0431BF360EC98BFCDF10AB816C985DA4EB2E0484260AE4B3`.
- La keystore y sus contraseñas permanecen fuera del proyecto y fuera de Git.
- Configuración local principal: `%LOCALAPPDATA%/QuantumForge/Signing`.
- Copia de recuperación local: `Documents/QuantumForge_QA_Signing_Backup`.
- Nunca registrar contraseñas en documentos, registros, comandos compartidos o capturas.

La firma pública puede registrarse para validación. La clave privada y sus contraseñas
no forman parte del sistema documental del proyecto.

## Última línea QA verificada

- APK anterior verificada: `0.1.8-qa-update`, versionCode 9.
- APK generada y verificada: `0.1.9-qa-update`, versionCode 10.
- Arquitectura: `arm64-v8a`.
- Min SDK: 26.
- Target SDK: 36.
- Build QA: Development con herramientas QA habilitadas.
- Constructor: `Assets/Project/Scripts/Editor/QuantumForgeAndroidDemoBuild.cs`.

Toda APK posterior del mismo paquete debe usar la misma firma y un versionCode mayor que
10. El constructor debe detener la generación si paquete, firma o versionCode no cumplen.

## Condición bloqueante antes de rendimiento final y lanzamiento

Decisión registrada el 10 de septiembre de 2026: mientras Quantum Forge continúe en
desarrollo se puede conservar la APK QA Debug actual para investigar errores. Esa APK usa
`BuildOptions.Development`, `BuildOptions.AllowDebugging` e IL2CPP Debug y no representa
el consumo de una compilación final.

La APK QA Debug no puede utilizarse para aprobar calor, batería, consumo sostenido de CPU,
rendimiento final ni preparación para lanzamiento. El diagnóstico en el RedMagic NX809J
demostró que la instrumentación de puntos de secuencia y pausa del depurador mantiene una
carga elevada en `UnityMain` aunque el juego ya haya bajado a 15 FPS en reposo. Evidencia:
`Logs/android_thermal_diagnosis_2026-09-10.md`.

Antes de la primera validación térmica definitiva y, en todo caso, antes de preparar una
candidata de lanzamiento, se debe crear y verificar una variante QA de rendimiento que:

1. retire `BuildOptions.AllowDebugging`;
2. compile IL2CPP con `Il2CppCompilerConfiguration.Release`;
3. conserve las herramientas QA necesarias durante el desarrollo sin reactivar la
   depuración de scripts;
4. preserve paquete, firma, guardado y continuidad de actualización, con un versionCode
   superior al instalado;
5. respalde la partida antes de instalarla, siguiendo el procedimiento de este contrato;
6. repita en el mismo dispositivo las mediciones de FPS, CPU por hilo, memoria, batería,
   temperatura de carcasa, temperaturas internas y estado térmico;
7. compare condiciones equivalentes de juego activo, reposo con pantalla encendida y una
   prueba corta y segura en bolsillo cuando siga siendo relevante.

Los umbrales numéricos de aceptación deberán acordarse antes de esa prueba; no se fijan
valores provisionales en este recordatorio. Si la variante sin depuración conserva una
carga o calentamiento anormales, se deberá perfilar entonces la lógica del juego antes de
autorizar el lanzamiento.

**Bloqueo:** no declarar rendimiento térmico final aprobado ni autorizar una build de
lanzamiento mientras esta condición continúe pendiente. La build pública tampoco puede
incluir `BuildOptions.Development`, `BuildOptions.AllowDebugging` ni IL2CPP Debug.

## Guardado y recuperación

- Archivo principal: `save.json`.
- Respaldo atómico: `save.json.bak`.
- Históricos recuperables: `save.json.history.1`, `.2` y `.3`.
- El save declara `saveSchemaVersion`.
- Una carga crea una copia histórica antes de recuperar o migrar.
- Un schema futuro no se carga ni se sobrescribe desde una versión anterior.
- RESET sólo se expone en QA y requiere confirmación explícita.

Antes de tocar un teléfono conectado:

1. Detectar el dispositivo sin instalar nada.
2. Extraer principal, `.bak` e históricos a una carpeta externa fechada.
3. Registrar tamaño y SHA-256 de cada copia disponible.
4. Cancelar la instalación si no puede completarse el respaldo del progreso existente.
5. Instalar la APK nueva encima de la anterior; nunca desinstalar ni borrar datos.
6. Verificar carga, recursos, desbloqueos, guardado posterior y creación de respaldos.

La instalación nueva se prueba en un emulador, perfil o dispositivo limpio independiente.
No se obtiene una «instalación limpia» borrando la partida del probador de actualización.

## Control de tamaño

- `0.1.3`: 170,62 MiB.
- `0.1.4`: 259,20 MiB.
- Delta: +88,58 MiB.
- Causa medida: `sharedassets0` aumentó 87,15 MiB; las bibliotecas nativas sólo 0,73 MiB.
- La optimización fue pospuesta hasta terminar el trabajo gráfico.

Antes de otra entrega se debe repetir la comparación por grupos y conservar el informe de
archivos responsables. La optimización no puede reducir la fidelidad visual aprobada.

## Evidencia de la APK 0.1.9

- SHA-256:
  `3A414BCF3EE5EE28EF3A332570AEF67BBCD333BA6BB04E9B2C35859EF5095F05`.
- Informe local:
  `Builds/Android/QuantumForge-QA-0.1.9-Update-ARM64-REPORT.txt`.
- Compilación Unity: PASS, 0 errores.
- Verificación física de instalación y actualización: pendiente; la APK se generó sin
  instalarla ni tocar el guardado personal del dispositivo.
