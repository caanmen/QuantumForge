# DIMENSIÓN 2 — DISEÑO CANÓNICO COMPLETO Y PLAN DE TRABAJO

## Alcance completo y alcance del chat actual

Estado del documento: resumen técnico secundario. Los tres TXT originales son
la base creativa; las decisiones explícitas posteriores del usuario pueden
completar sus apartados tentativos. Este archivo debe reflejarlas, no sustituirlas.

Este es el resumen técnico consolidado de Dimensión 2. Reúne el diseño confirmado,
la estructura técnica propuesta, el orden de implementación, las pruebas y los
asuntos pendientes de sus tres civilizaciones y de la integración final.

El alcance completo queda dividido en:

- Bloque 1: base común de Dimensión 2.
- Bloque 2: Civilización 1.
- Bloque 3: Civilización 2.
- Bloque 4: Civilización 3.
- Bloque 5: pactos mayores, conexiones e integración final.

En el estado actual están implementados los Bloques 1–5F, incluidas las tres
civilizaciones, sus pactos mayores y la corrección 4H recuperada de los TXT.
Quedan una prueba manual integral, las pruebas largas de balance y el arte/pulido
definitivo.

## 1. Fuentes de diseño y orden de autoridad

Fuentes revisadas:

1. Archivos reales actuales del proyecto.
2. `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`.
3. Reglas adjuntas por el usuario el 18 de julio de 2026.
4. `dimension de pactos 1era parte.txt` — Civilización 1.
5. `dimension 2 pactos 2da parte.txt` — Civilización 2.
6. `dimension de pactos 3 parte.txt` — Civilización 3.

Orden de autoridad durante el trabajo:

1. Instrucción actual y explícita del usuario.
2. Reglas de trabajo y seguridad vigentes.
3. Los tres TXT originales de pactos, como fuente creativa principal.
4. Decisiones posteriores aprobadas explícitamente por el usuario para completar
   apartados tentativos de los TXT.
5. Este resumen técnico, que debe registrar fielmente esas decisiones sin ampliarlas por iniciativa del asistente.
6. Resumen de continuidad del chat más reciente.
7. Código y archivos reales para determinar el estado técnico existente.

El código real determina qué está implementado. Si contradice este documento,
se debe informar antes de realizar cambios importantes. El diseño canónico
determina el comportamiento deseado, salvo una instrucción posterior del
usuario.

### Fidelidad obligatoria al diseño

- No se implementará ninguna mecánica, valor, coste, fórmula, recurso, nivel,
  desbloqueo, recompensa, penalización o relación que no aparezca en el diseño
  aprobado por el usuario.
- Si el diseño no contiene un dato indispensable, la parte afectada se detiene,
  se explica el vacío y se presentan opciones. Ninguna opción se implementa sin
  permiso explícito.
- Los valores temporales para pruebas también requieren aprobación previa y
  deben quedar registrados como provisionales.
- Una inferencia técnica o de diseño del asistente nunca sustituye una decisión
  creativa del usuario.
- Toda decisión nueva aprobada debe añadirse a este documento para que viaje a
  los chats siguientes.

### Regla conservadora para archivos protegidos de Unity

Las reglas adjuntas requieren permiso explícito antes de modificar archivos
`.unity`, `.prefab`, `.asset`, `.meta`, binarios u otros recursos protegidos de
Unity. Esta regla estricta se aplicará durante los Bloques 1 y 2.

Se puede avanzar de forma autónoma en C#, JSON, documentación y comprobaciones
ligeras dentro del bloque expresamente autorizado. Antes de modificar una
escena, prefab, asset o sus referencias, se debe indicar al usuario qué se
modificará y solicitar autorización explícita.

No se usará Git salvo petición explícita del usuario.

## 2. Estado técnico encontrado antes de comenzar

- Dimensión 1 se considera funcionalmente cerrada por ahora. Conserva una lista
  separada de balance, localización y pulido futuro.
- `GameState` ya contiene `dimension02Unlocked`, pero Dimensión 2 todavía no
  posee estado jugable propio.
- Prestigio 1 prepara actualmente las tres dimensiones mediante sus indicadores
  de desbloqueo.
- No existe todavía un panel, sistema de simulación, guardado específico ni UI
  jugable para Dimensión 2.
- `TabsUI` solo tiene conexión funcional para Dimensión 1.
- Dimensión 1 terminó concentrando mucha lógica en archivos grandes. Dimensión 2
  debe dividirse desde el principio por responsabilidades y civilizaciones.

### Aprobación de diseño de Bloque 2A

Bloque 1 está terminado y probado. Bloque 2A fue probado por el usuario y quedó
aprobado el 18 de julio de 2026. Las siguientes ampliaciones, ausentes en el
documento original, fueron explicadas y aceptadas expresamente como diseño
canónico provisional:

- 5 Seguidores iniciales;
- llegada base de 0.05 Seguidores por segundo, equivalente a 3 por minuto;
- asignación exclusiva de Seguidores al Refugio;
- bonus de asignación `1 + (√asignados × 0.15)`;
- diez niveles de Refugio durante el alcance actual;
- mejora de Refugio pagada con Seguidores disponibles;
- coste `techo(12 × 1.85^(nivel actual - 1))`;
- incremento de 25% de la producción base por cada nivel posterior al primero;
- producción online y offline con conservación del progreso fraccional.

Estos valores pueden ajustarse durante el balance final sin cambiar la mecánica
central. Ya no se consideran elementos pendientes de aprobación.

## 3. Identidad general de Dimensión 2

Dimensión 2 es la Dimensión de los Pactos. Su tema central es la relación del
jugador con civilizaciones y entes mediante formas distintas de cooperación,
resistencia y conocimiento.

Contiene tres territorios principales, cada uno correspondiente a una
civilización:

| Territorio | Civilización | Identidad principal | Estado inicial |
| --- | --- | --- | --- |
| 1 | Civilización 1 | Devoción, peregrinaciones y confianza | Disponible |
| 2 | Civilización 2 | Resistencia contra un ente hostil | Bloqueado |
| 3 | Civilización 3 | Ruinas, arqueología y conocimiento | Bloqueado |

Se utilizará **territorio** para las tres selecciones del mapa general. Esto
evita confundirlas con las regiones internas de Civilización 2 y las zonas
arqueológicas de Civilización 3.

Flujo global:

```text
Entrada a Dimensión 2
→ presentación de aproximación al mundo
→ mapa con tres territorios visibles
→ solo Civilización 1 disponible
→ progreso de Civilización 1
→ Civilización 2 disponible a 300 de 500 de Confianza
→ progreso futuro de Civilización 2
→ Civilización 3 disponible al alcanzar el requisito futuro de Civ 2
```

## 4. Alcance del Bloque 1 — Base común de Dimensión 2

### Objetivo

Crear la infraestructura persistente y navegable necesaria para que Dimensión 2
pueda alojar las tres civilizaciones sin tener que rehacer su base más adelante.

### 4.1 Estado y guardado

Debe existir un estado serializable propio de Dimensión 2 que incluya como
mínimo:

- versión interna de progreso/migración;
- indicador de primera entrada;
- territorio seleccionado;
- estado de desbloqueo de cada civilización;
- estado completo de Civilización 1;
- espacios preparados para añadir Civ 2 y Civ 3 sin romper partidas antiguas;
- marcas de presentación ya vista cuando corresponda.

La incorporación debe preservar partidas antiguas. Una partida sin datos de
Dimensión 2 debe inicializar un estado válido sin borrar ni alterar progreso
existente.

### 4.2 Coordinador de Dimensión 2

Un coordinador general debe encargarse solamente de:

- inicialización y saneamiento del estado;
- desbloqueos entre civilizaciones;
- tick general y distribución hacia cada sistema;
- progreso offline común;
- consultas compartidas por la UI;
- reinicio y validación de Dimensión 2.

La lógica detallada de cada civilización no debe acumularse en este coordinador.

### 4.3 Acceso y navegación

Debe existir:

- acceso visible a Dimensión 2 cuando `dimension02Unlocked` sea verdadero;
- panel raíz de Dimensión 2;
- mapa general con tres territorios;
- entrada al panel de Civilización 1;
- territorios 2 y 3 visibles como bloqueados;
- retorno claro desde una civilización al mapa;
- refresco correcto de estados y textos.

La presentación de aproximación al planeta/mundo forma parte del diseño, pero
puede comenzar con una transición funcional provisional. La animación y el arte
definitivos requieren decisión visual y permiso para modificar recursos de
Unity.

### 4.4 Herramientas de validación

El Bloque 1 debe incluir herramientas DEBUG o validadores equivalentes para:

- asegurar e imprimir el estado de Dimensión 2;
- simular primera entrada;
- comprobar desbloqueos del mapa;
- verificar saneamiento y migración;
- probar guardado/carga sin depender de horas de juego;
- reiniciar únicamente el estado de Dimensión 2 cuando sea seguro.

Una partida modificada con DEBUG no se utilizará como evidencia de balance.

### 4.5 Criterio de cierre del Bloque 1

El Bloque 1 solo podrá cerrarse cuando:

- el código compile sin errores relacionados;
- una partida antigua cargue conservando su progreso;
- una partida nueva inicialice correctamente Dimensión 2;
- el estado de Dimensión 2 se guarde y cargue;
- el mapa muestre los tres territorios con estados correctos;
- se pueda entrar y salir de Civilización 1;
- las conexiones indispensables de Unity estén realizadas y probadas;
- se hayan registrado claramente el arte o pulido todavía provisionales.

## 5. Alcance del Bloque 2 — Civilización 1

### 5.1 Identidad

Civilización 1 tiene una relación amistosa y sagrada con su ente. El jugador no
gobierna la civilización; llega como visitante y forma su propio grupo de
seguidores.

Tono:

- espiritualidad;
- edad media oscura;
- peregrinaciones;
- refugios, altares y ofrendas;
- noviciado y acólitos;
- ritos;
- confianza y pacto.

Ciclo principal:

```text
Refugio de Peregrinos
→ Seguidores
→ Altares y Ofrendas
→ Peregrinaciones
→ Confianza
→ Noviciado, Acólitos y Ritos
→ Umbral Velado
```

### 5.2 Seguidores y Refugio de Peregrinos

- Los Seguidores son el recurso principal.
- No tienen límite duro.
- El Refugio genera Seguidores pasivamente.
- Mejorar el Refugio aumenta su llegada y posteriormente puede mejorar sistemas
  asociados.
- La cantidad visible de Seguidores será entera. Si la producción genera
  fracciones, se conservará internamente un acumulador para no perder progreso.
- Los Seguidores asignados no pueden utilizarse simultáneamente en varios
  sistemas.
- Debe ser posible asignarlos y retirarlos de forma clara.

Eficiencia decreciente base:

```text
eficiencia efectiva = raíz cuadrada de la cantidad asignada
bonus = eficiencia efectiva × factor del sistema
```

Implementación aprobada de Bloque 2A:

```text
Seguidores iniciales = 5
Llegada base = 0.05/s = 3/min
Multiplicador por apoyo = 1 + (√Seguidores asignados × 0.15)
Producción final = llegada base del nivel × multiplicador por apoyo
```

El Refugio tiene provisionalmente 10 niveles. Cada nivel posterior al primero
añade 25% de la producción base. Mejorarlo consume Seguidores disponibles:

```text
Coste = techo(12 × 1.85^(nivel actual - 1))
```

Los Seguidores asignados quedan ocupados y pueden retirarse. Los consumidos por
una mejora representan población dedicada permanentemente a ampliar y mantener
el Refugio. Las cantidades fraccionales se conservan y el sistema admite
progreso offline.

Factores tentativos del documento original:

| Sistema | Factor tentativo |
| --- | ---: |
| Altares | 0.35 |
| Ritos con seguidores | 0.25 |
| Peregrinaciones | 0.20 |
| Refugio | 0.15 |
| Noviciado | 0.18 |

Estos factores son configurables y no se considerarán balance definitivo.

### 5.3 Altares y Ofrendas

Habrá cinco altares:

| Altar | Estado inicial | Ofrenda |
| --- | --- | --- |
| Altar de Cera | Disponible | Cera |
| Altar de Pan Ritual | Disponible | Pan ritual |
| Altar de Incienso | Bloqueado | Incienso |
| Altar de Tela Sagrada | Bloqueado | Tela sagrada |
| Altar de Piedra Tallada | Bloqueado | Piedra tallada |

Decisión canónica para la implementación:

- Las cinco ofrendas se almacenan como recursos separados.
- Comparten la misma infraestructura de producción y presentación.
- Sus usos y costes pueden pedir combinaciones distintas sin crear cinco
  sistemas independientes.

Diseño aprobado para el Bloque 2B:

- Altar de Cera y Altar de Pan Ritual están disponibles desde el inicio.
- Los otros tres Altares permanecen visibles y bloqueados. No se inventa un
  requisito de desbloqueo: se definirá antes del bloque que los habilite.
- Cada Altar conserva su propio saldo de Ofrenda y su propia asignación
  exclusiva de Seguidores.
- La producción base común es `0.05 Ofrendas/s`.
- El multiplicador por Seguidores asignados es
  `1 + (raíz_cuadrada(asignados) × 0.35)`.
- Los Altares no tienen niveles ni mejoras, porque el diseño original no los
  contempla.
- La producción funciona online y offline mediante el sistema general de
  progreso ya existente.
- Las Ofrendas todavía no se gastan. Sus costes se definirán en los bloques de
  Peregrinaciones, Noviciado, Ritos o Pactos que correspondan.

Las ofrendas se utilizan para peregrinaciones, noviciado, ritos, pactos de
civilización y progreso interno. No se transfieren directamente a otras
dimensiones.

### 5.4 Peregrinaciones y Confianza

Las peregrinaciones son actividades temporizadas. Las tres iniciales son:

| Peregrinación | Duración base | Confianza base | Recompensa general |
| --- | ---: | ---: | --- |
| Corta | 1 minuto | 1 | Pocas ofrendas comunes |
| Media | 4 minutos | 5 | Ofrendas y posible llegada de seguidores |
| Larga | 10 minutos | 12 | Más ofrendas, seguidores y progreso avanzado futuro |

Reglas canónicas:

- Iniciar una peregrinación consume sus costes al comienzo.
- Seguidores o acólitos comprometidos quedan ocupados hasta completarla.
- No se generan recompensas duplicadas al cargar una partida.
- El progreso offline puede completar peregrinaciones, respetando el mismo
  resultado que el progreso online.
- La Confianza va de 0 a 500.
- Civilización 2 se desbloquea una sola vez al alcanzar 300 de Confianza.
- Al alcanzar 500 se manifiesta el **Umbral Velado**. La interfaz y la narrativa
  no deben identificar todavía qué existe detrás ni utilizar la palabra Ente.
- Altares, producción y ritos no generan Confianza directamente.
- Ritos y pactos pueden modificar la Confianza obtenida.
- El multiplicador inicial total de Confianza no debe superar ×2.

Balance provisional aprobado para el Bloque 2C, sujeto a afinación mediante
partidas de prueba:

| Peregrinación | Seguidores ocupados | Coste inicial | Recompensa al completar |
| --- | ---: | --- | --- |
| Corta | 1 | 2 Cera + 2 Pan ritual | 1 Confianza + 1 Cera + 1 Pan ritual |
| Media | 3 | 10 Cera + 10 Pan ritual | 5 Confianza + 3 Cera + 3 Pan ritual + 25% de obtener 1 Seguidor |
| Larga | 6 | 25 Cera + 25 Pan ritual | 12 Confianza + 8 Cera + 8 Pan ritual + 1 Seguidor |

Reglas aprobadas para esta primera implementación:

- Solo puede existir una Peregrinación activa a la vez.
- Los Seguidores comprometidos quedan ocupados y regresan al finalizar.
- Las Ofrendas se consumen al iniciar.
- Cancelar devuelve los Seguidores, pero no devuelve Ofrendas ni concede
  recompensas o Confianza.
- La posibilidad de Seguidor de la Peregrinación Media se decide una sola vez
  al iniciar y se conserva en el guardado; recargar no permite repetirla.
- La finalización y la entrega de recompensas son automáticas online y offline.
- El desbloqueo de Civilización 2 a 300 y del Umbral Velado a 500 son
  permanentes. Sus contenidos se implementan en sus bloques correspondientes.

Peregrinaciones avanzadas tentativas:

| Tipo | Confianza base |
| --- | ---: |
| Larga con un acólito | 16 |
| Sagrada con dos acólitos | 25 |

### 5.5 Noviciado y Acólitos

Los Acólitos se forman mediante Seguidores, Ofrendas y tiempo. No se obtienen
automáticamente por Confianza.

Base inicial:

```text
5 Seguidores + Ofrendas + tiempo → tanda de Acólitos
```

Tabla tentativa:

| Nivel de Noviciado | Duración | Acólitos por tanda |
| --- | ---: | ---: |
| 1 | 5 minutos | 1 |
| 2 | 6 minutos | 2 |
| 3 | 8 minutos | 4 |
| 4 | 10 minutos | 7 |
| 5 | 12 minutos | 10 |

Balance provisional aprobado para implementación y afinación mediante runs:

| Nivel | Seguidores convertidos | Cera | Pan ritual |
| --- | ---: | ---: | ---: |
| 1 | 5 | 12 | 12 |
| 2 | 10 | 22 | 22 |
| 3 | 20 | 40 | 40 |
| 4 | 35 | 65 | 65 |
| 5 | 50 | 90 | 90 |

Costes provisionales de mejora:

| Mejora | Seguidores | Cera | Pan ritual |
| --- | ---: | ---: | ---: |
| Nivel 1 → 2 | 25 | 30 | 30 |
| Nivel 2 → 3 | 60 | 75 | 75 |
| Nivel 3 → 4 | 140 | 160 | 160 |
| Nivel 4 → 5 | 300 | 350 | 350 |

Reglas de implementación aprobadas:

- Solo puede existir una tanda de formación activa.
- Los Seguidores quedan ocupados durante la formación y se convierten en
  Acólitos al completarla; no regresan como Seguidores.
- Las Ofrendas se consumen al iniciar.
- Cancelar devuelve los Seguidores, pero no devuelve las Ofrendas.
- Mejorar el Noviciado consume permanentemente sus costes y no puede hacerse
  mientras una tanda esté activa.
- La formación puede completarse offline sin duplicar resultados.
- Los Acólitos no tienen límite duro y permanecen como recurso separado.

Peregrinaciones avanzadas provisionales del Bloque 2D:

| Tipo | Duración | Requisito | Coste | Recompensa |
| --- | ---: | --- | --- | --- |
| Larga con Acólito | 10 min | 6 Seguidores + 1 Acólito | 35 Cera + 35 Pan | 16 Confianza + 10 Cera + 10 Pan + 1 Seguidor |
| Sagrada | 15 min | 8 Seguidores + 2 Acólitos | 50 Cera + 50 Pan | 25 Confianza + 15 Cera + 15 Pan + 2 Seguidores |

Los Acólitos usados en Peregrinaciones quedan ocupados y regresan al terminar o
cancelar. Estos valores no son balance definitivo.

Los niveles superiores aumentan la producción por tanda; no buscan reducir
todo a ciclos de pocos segundos. Los Acólitos no tienen límite duro, pero su
efecto utiliza eficiencia decreciente.

Usos del Bloque 2:

- potenciar ritos;
- participar en peregrinaciones especiales;
- preparar el misterio del Umbral Velado.

### 5.6 Ritos

Habrá cinco ritos:

| Rito | Función |
| --- | --- |
| Recibimiento | Mejora llegada de Seguidores |
| Ofrenda | Mejora producción de Altares |
| Camino | Mejora peregrinaciones |
| Noviciado | Mejora formación de Acólitos |
| Respeto | Mejora Confianza obtenida |

- Dos ritos pueden estar activos inicialmente.
- El tercer espacio activo es un desbloqueo avanzado.
- Un rito funciona mientras mantenga Seguidores y/o Acólitos asignados.
- Las unidades asignadas quedan ocupadas, pero pueden retirarse.
- Los ritos no generan Confianza por sí solos.

Potencia tentativa:

```text
potencia = (√Seguidores × 0.25) + (√Acólitos × 0.60)
```

Los límites de bonus del documento original se conservan como referencias de
balance, no como valores definitivos.

### 5.7 Pactos de Civilización

Los Pactos de Civilización son acuerdos internos, distintos del pacto mayor con
el Ente. Cada pacto combina beneficio y compromiso:

| Pacto | Beneficio | Compromiso |
| --- | --- | --- |
| Hospedaje | Más Seguidores | Mayor consumo de ofrendas básicas |
| Camino Abierto | Mejores peregrinaciones | Mayor preparación/coste |
| Consagración | Mejores Acólitos | Permanecen ocupados más tiempo |
| Voto Silencioso | Mucha más Confianza | Casi detiene llegada de Seguidores |
| Puerta Interior | Facilita contacto final | Limita Noviciado o encarece Ritos |

Los valores exactos, número de pactos simultáneos, activación, cancelación y
mantenimiento deben cerrarse antes de implementar esta subdivisión. No se debe
inventar ese comportamiento directamente en código.

### 5.8 Umbral Velado y revelación futura

Al alcanzar 500 de Confianza, algo desconocido responde y se manifiesta el
**Umbral Velado**. Para conservar la sorpresa, ningún texto visible de esta
etapa debe llamarlo Ente, contacto con el Ente o pacto mayor.

El flujo interno reservado al diseño es:

```text
Umbral Velado
→ lugar o altar de vínculo
→ asignación de Acólitos
→ revelación gradual de su naturaleza
→ líneas de vínculo todavía ocultas
→ mejoras graduales
```

El contenido, costes, revelación y efectos posteriores todavía no están
definidos. Dentro del Bloque 2 solo se implementa y valida la aparición del
Umbral Velado; no se muestra ni confirma su verdadera naturaleza hasta el
bloque narrativo aprobado por el usuario.

Existe una contradicción entre documentos antiguos:

- una versión asigna Civ 1 a Cuarto 1 + Cuarto 2, Civ 2 a Dimensión 1 y Civ 3 a
  Dimensión 3;
- otra versión asigna Civ 1 a Cuarto 1, Civ 2 a Cuarto 2 y Civ 3 a mejoras
  internas de Dimensión 2.

Esta contradicción no bloquea el ciclo principal de Civ 1, pero debe resolverse
antes de implementar los efectos del pacto mayor.

## 6. Subdivisión de implementación del Bloque 2

Para mantener bloques funcionales comprobables, Civilización 1 se dividirá así:

### 2A. Estado, Refugio y Seguidores

- estado persistente;
- producción pasiva;
- mejora del Refugio;
- asignación y liberación;
- progreso online/offline;
- UI funcional y DEBUG.

### 2B. Altares y Ofrendas

- dos Altares iniciales;
- tres Altares bloqueados;
- cinco saldos separados;
- producción, mejoras y asignaciones;
- guardado/carga y UI.

### 2C. Peregrinaciones y Confianza

- tres peregrinaciones iniciales;
- costes, temporizadores y recompensas;
- Confianza 0–500;
- desbloqueo de Civ 2 a 300;
- manifestación del Umbral Velado a 500;
- progreso offline sin duplicación.

### 2D. Noviciado y Acólitos

- formación por tandas;
- costes y temporizador;
- niveles 1–5;
- ocupación y liberación de unidades;
- peregrinaciones avanzadas.

### 2E. Ritos

- cinco Ritos;
- dos espacios iniciales y tercero avanzado;
- asignación de Seguidores/Acólitos;
- bonus con límites seguros;
- guardado, carga y UI.

### 2F. Pactos de Civilización

- diseño numérico final aprobado;
- activación, mantenimiento y cancelación;
- beneficio + compromiso;
- interacción con los sistemas anteriores.

### 2G. Umbral Velado

- desbloqueo y presentación del contacto;
- lugar de vínculo funcional;
- estado preparado para una revelación futura;
- naturaleza, vínculos y efectos aplazados hasta resolver su diseño.

Cada subdivisión debe compilar y validarse antes de iniciar la siguiente. No se
deben modificar simultáneamente varias subdivisiones grandes.

## 7. Alcance del Bloque 3 — Civilización 2

### 7.1 Identidad y desbloqueo

Civilización 2 vive sometida por un Ente hostil. El jugador no intenta ganar
su confianza: organiza una Resistencia, reduce el Dominio del Ente, controla la
Amenaza y finalmente intenta contenerlo.

Se desbloquea una sola vez cuando Civilización 1 alcanza 300 de 500 de
Confianza.

Flujo principal:

```text
Civilización sometida
→ Miembros de Resistencia
→ Operaciones regionales
→ reducción de Dominio
→ Amenaza y Represalias
→ fase de Alerta
→ intento de Contención
→ pacto mayor con el Ente
```

### 7.2 Regiones y Dominio

Habrá tres regiones jugables y una cuarta visible como contenido futuro:

| Región | Desbloqueo tentativo |
| --- | --- |
| 1 | Disponible al entrar en Civ 2 |
| 2 | Dominio total en 80% |
| 3 | Dominio total en 60% |
| 4 | Visible, bloqueada para actualización futura |

Cada región tiene:

- Dominio regional de 0% a 100%;
- Amenaza regional de 0% a 100%;
- Miembros asignados;
- operaciones activas;
- Cobertura;
- modificadores y Represalias regionales.

Cada región comienza con 100% de Dominio. El Dominio total es el promedio de
las regiones activas. Cuando una nueva región se desbloquea con 100% de
Dominio, el promedio puede subir; el desbloqueo ya conseguido no debe perderse.
La presentación debe explicar este repunte para que no parezca pérdida o error.

Al llegar a 30% de Dominio total:

- se desbloquea Civilización 3;
- se desbloquea Intentar Contención;
- comienza la fase de Alerta del Ente.

### 7.3 Miembros de Resistencia

- Recurso principal de Civilización 2.
- Inicio tentativo: 10 Miembros.
- Sin límite duro inicial.
- La cantidad visible es entera y las fracciones de producción se acumulan.
- Los Miembros deben asignarse de forma exclusiva entre regiones, operaciones,
  pactos y Contención.
- Efectividad base: `√Miembros asignados × factor`.

Debe existir un saldo global y asignaciones regionales inequívocas. Ningún
Miembro puede participar simultáneamente en dos actividades.

### 7.4 Operaciones regionales

Cada región dispone de cuatro operaciones:

| Operación | Requisito tentativo | Efecto principal | Dominio/min | Amenaza/min |
| --- | ---: | --- | ---: | ---: |
| Rescate | 5 Miembros | Genera Miembros | -0.05% | +0.20% |
| Protección | 5 Miembros | Baja Amenaza y genera Cobertura | -0.02% | -0.35% |
| Espionaje | 10 Miembros | Avance más seguro | -0.12% | +0.25% |
| Sabotaje | 20 Miembros | Avance rápido y riesgoso | -0.30% | +0.60% |

Producción tentativa:

- Rescate: `√Miembros × 0.20 Miembros/min`.
- Protección: `√Miembros × 0.08 Miembros/min`.
- Protección: `√Miembros × 0.25 Cobertura/min`.
- Espionaje reduce 5% la fuerza de la próxima Represalia de esa región.

Antes de implementar debe definirse si varias operaciones pueden funcionar a
la vez en una región. La recomendación técnica es permitirlo solo mediante
asignaciones separadas y exclusivas, porque crea decisiones sin duplicar
Miembros.

### 7.5 Amenaza, Represalias y Cobertura

Cuando la Amenaza regional alcanza 100%, ocurre una Represalia en esa región:

```text
Represalia
→ pérdida de una parte de los Miembros asignados
→ debilitamiento temporal de una operación
→ Fragmentos de Control
→ Amenaza vuelve a 25%
```

Valores tentativos:

- pérdida base: 8% de los Miembros asignados en la región;
- debilitamiento: 3 minutos;
- operación debilitada: una de las cuatro, elegida según la regla final;
- Fragmentos de Control normales: 3;
- Fragmentos durante Alerta: 6;
- no se destruyen mejoras permanentes;
- no se recupera Dominio ya reducido.

Cobertura reduce la pérdida:

- cada 10 de Cobertura reduce un punto porcentual de pérdida;
- pérdida mínima: 2%.

La Cobertura necesita límite, consumo o deterioro. No puede acumularse
indefinidamente hasta anular permanentemente el peligro. La regla exacta debe
cerrarse antes de implementar Represalias.

También debe definirse el límite y consumo de la reducción de Represalia
obtenida mediante Espionaje, para evitar acumulaciones ilimitadas.

### 7.6 Fragmentos de Control y mejoras

Los Fragmentos representan restos de los mecanismos de dominio del Ente. Se
obtienen al resistir Represalias y sirven para mejorar:

- Rescate;
- Protección;
- Espionaje;
- Sabotaje;
- defensa contra Represalias;
- Pactos de Resistencia.

Las líneas tentativas son Romper Marca Menor, Refugios Sellados, Lectura de
Símbolos y Falla en la Cadena. Sus niveles, costes y límites deben definirse en
configuración antes de implementarlos.

### 7.7 Pactos de Resistencia

Son pactos globales que afectan todas las regiones. Requieren Miembros
asignados, sufren desgaste y producen una penalización temporal si se
incumplen.

| Pacto | Asignación inicial | Beneficio tentativo | Desgaste |
| --- | ---: | --- | --- |
| Refugios Ocultos | 30 | -20% pérdidas por Represalia | 1 Miembro/2 min |
| Campanas Silenciadas | 25 | -30% duración de debilitamientos | 1 Miembro/2 min |
| Cuchillos Bajo la Mesa | 40 | +20% efectividad de Sabotaje | 1 Miembro/90 s |

Penalizaciones tentativas:

- Refugios: próxima Represalia causa +25% de pérdidas.
- Campanas: próximo debilitamiento dura +40%.
- Cuchillos: Sabotaje genera +30% de Amenaza durante 5 minutos.

Antes de implementar se debe precisar si los Miembros desgastados se pierden,
quedan temporalmente indisponibles o regresan después de un tiempo. También se
debe definir si una penalización que dice “5 minutos o hasta el próximo evento”
termina con lo primero que ocurra.

### 7.8 Alerta y ataques del Ente

Desde 30% de Dominio total:

- Rescate, Espionaje y Sabotaje generan 50% más Amenaza;
- las Represalias entregan 6 Fragmentos;
- cada 10 minutos el Ente puede marcar una región activa;
- si la región no mantiene Protección, su próxima Represalia añade 3 puntos
  porcentuales de pérdida;
- Protección mitiga la marca.

Debe definirse si las marcas pueden acumularse. La recomendación es una sola
marca por región, renovable pero no acumulable.

### 7.9 Contención

La Contención se desbloquea con Dominio total menor o igual a 30%.

Tabla tentativa:

| Dominio total | Probabilidad de éxito |
| ---: | ---: |
| 30% | 20% |
| 20% | 45% |
| 10% | 70% |
| 0% | 100% |

El documento original también propone
`20 + ((30 - Dominio) × 2.67)`, pero esa fórmula no coincide exactamente con la
tabla. Antes de programar se debe elegir la tabla con interpolación o una nueva
fórmula canónica.

Fallo tentativo:

- +20% de Amenaza en todas las regiones activas;
- pérdida de 5% de Miembros no protegidos;
- cooldown de 10 minutos;
- no reinicia Dominio ni elimina mejoras o Pactos permanentemente.

Éxito:

- el Ente queda contenido;
- se desbloquea su pacto mayor;
- Miembros pueden asignarse a sostener Contención;
- el pacto mayor puede mejorarse gradualmente.

### 7.10 Duración y subdivisiones del Bloque 3

Objetivo tentativo:

- jugador atento: 4–6 horas hasta Contención;
- jugador pasivo o desordenado: 7–9 horas.

Subdivisiones recomendadas:

- 3A: estado, Miembros, Región 1 y asignaciones;
- 3B: cuatro operaciones, Dominio y Amenaza;
- 3C: Represalias, Cobertura y Fragmentos;
- 3D: regiones 2 y 3 y promedio total;
- 3E: mejoras y Pactos de Resistencia;
- 3F: fase de Alerta y marcas;
- 3G: Contención, resultado y preparación del pacto mayor.

## 8. Alcance del Bloque 4 — Civilización 3

### 8.1 Identidad y desbloqueo

Civilización 3 no es una sociedad viva, sino una civilización perdida. El
jugador explora ruinas, analiza restos y descubre un Ente que responde al
conocimiento sin revelarse completamente.

Se desbloquea cuando Civilización 2 alcanza 30% de Dominio total.

Sus tres capas son:

```text
Arqueología normal
→ Anomalías
→ Investigación y pacto con el Ente
```

### 8.2 Zonas

| Zona | Nombre tentativo | Recurso propio | Rol |
| --- | --- | --- | --- |
| 1 | Entrada Sepultada | Fragmentos Base | Inicio arqueológico |
| 2 | Galería de Inscripciones | Inscripciones Parciales | Interpretación y anomalías |
| 3 | Santuario Sellado | Sellos Antiguos | Ente y pacto |

Cada zona conserva utilidad porque produce un recurso exclusivo.

### 8.3 Excavación, restos y calidad

Flujo:

```text
Excavar
→ obtener restos
→ analizar con el Erudito de la zona
→ Conocimiento Antiguo + recurso propio
→ mejorar zona, Erudito y Archivo
→ aumentar Investigación de zona
```

Distribución tentativa de calidad:

| Zona | Baja | Media | Alta |
| --- | ---: | ---: | ---: |
| 1 | 70% | 25% | 5% |
| 2 | 50% | 35% | 15% |
| 3 | 30% | 45% | 25% |

La calidad determina Conocimiento y probabilidad de indicios. Antes de
implementar deben definirse duración, coste, espacios simultáneos, inventario y
automatización de excavaciones, además de duración y cola de análisis.

### 8.4 Eruditos

- Máximo de un Erudito por zona.
- Se contratan y mejoran; no son una población masiva.
- Zona 1: Erudito de Campo.
- Zona 2: Erudito de Inscripciones.
- Zona 3: Erudito de Sellos.

Costes tentativos:

- Erudito 1: recursos pequeños de Civ 1.
- Erudito 2: recursos de Civ 1 y Zona 1 al 60%.
- Erudito 3: recursos de Civ 1 y Zona 2 al 60%.

Esto crea una dependencia deliberada con Civilización 1. Los recursos exactos,
niveles y efectos deben cerrarse antes de implementar.

### 8.5 Investigación por zona

Cada zona tiene progreso de 0% a 100%:

| Progreso | Efecto tentativo |
| ---: | --- |
| 20% | +5% restos obtenidos en esa zona |
| 40% | +5% velocidad de análisis |
| 60% | permite destapar la zona siguiente |
| 80% | +10% Conocimiento por análisis |
| 100% | bonus especial de zona completa |

Zona 2 requiere Zona 1 al 60% y pago de recursos de Civ 1. Zona 3 requiere Zona
2 al 60% y otro pago de recursos de Civ 1.

Se debe definir cuánto progreso entrega cada calidad de resto y cuál es el
bonus especial del 100% de cada zona.

### 8.6 Recursos del análisis

Todo análisis entrega Conocimiento Antiguo y un recurso propio:

- Zona 1: Fragmentos Base.
- Zona 2: Inscripciones Parciales.
- Zona 3: Sellos Antiguos.

El Conocimiento Antiguo paga mejoras generales, Archivo, Eruditos e
Investigación del Ente. Los recursos de zona pagan sus mejoras específicas y
los hitos correspondientes.

### 8.7 Archivo de Interpretación

Se desbloquea después del primer análisis exitoso:

| Nivel | Condición | Función |
| --- | --- | --- |
| Archivo I | Primer análisis | Mejoras básicas |
| Archivo II | Zona 1 al 40% | Mejoras medias |
| Archivo III | Zona 2 al 40% | Lectura de Anomalías |
| Archivo IV | Zona 3 al 30% | Mejoras avanzadas y Ente |

Los costes combinan Conocimiento con el recurso correspondiente. Las mejoras
exactas de cada nivel deben diseñarse antes de implementar el Archivo.

### 8.8 Indicios y Anomalías

La detección comienza cuando Zona 2 alcanza 20%. Desde entonces, cualquier
análisis elegible puede producir indicios:

| Calidad | Probabilidad tentativa |
| --- | ---: |
| Baja | 3% |
| Media | 8% |
| Alta | 18% |

| Zona | Indicios necesarios | Anomalía revelada |
| --- | ---: | --- |
| 1 | 8 básicos | Básica |
| 2 | 10 simbólicos | Simbólica |
| 3 | 12 profundos | Profunda |

Leer una Anomalía consume Conocimiento Antiguo y el recurso de su zona, y
entrega Datos Anómalos Básicos, Simbólicos o Profundos.

Debe decidirse si existe una sola Anomalía canónica por zona o si el sistema es
repetible. La investigación inicial del Ente requiere al menos un Dato de cada
tipo. Si los indicios dependen del azar, se debe incluir protección contra mala
suerte o progreso garantizado para evitar bloqueos prolongados.

Decisión aprobada para el Bloque 4D — 22 de julio de 2026:

- Existe una Anomalía única por zona.
- La Básica se revela permanentemente con 8 Indicios Básicos y la Simbólica
  con 10 Indicios Simbólicos. Los Indicios no se consumen y quedan como registro.
- Archivo III se desbloquea automáticamente con Zona 2 al 40% y habilita la lectura.
- Cada Anomalía puede leerse una sola vez y después permanece archivada como leída.
- La lectura Básica cuesta 25 Conocimiento Antiguo y 15 Fragmentos Base.
- La lectura Simbólica cuesta 40 Conocimiento Antiguo y 25 Inscripciones Parciales.
- Cada lectura entrega exactamente 1 Dato Anómalo de su tipo.
- La estructura de la Anomalía Profunda queda preparada, pero su lectura y coste
  se definirán junto con Zona 3 en el Bloque 4E.
- Estos costes son provisionales y se afinarán mediante runs largas sin cambiar
  la estructura aprobada.

Decisión aprobada para el Bloque 4E — 22 de julio de 2026:

- Zona 3 se destapa con Zona 2 al 60% y el pago provisional de 50 Incienso,
  50 Tela Sagrada y 50 Piedra Tallada.
- El Erudito de Sellos cuesta provisionalmente 30 Cera y 30 Pan ritual; se
  contrata una sola vez y no tiene mantenimiento.
- Zona 3 conserva excavación manual de 30 segundos, una actividad propia que
  puede coexistir con las otras zonas, sin cola ni automatización.
- Su distribución es 30% Baja, 45% Media y 25% Alta. El análisis dura 30 segundos
  y mantiene recompensas 1/3/8 de Conocimiento, 1/2/4 Sellos y 1/3/8% de Investigación.
- Archivo IV se desbloquea automáticamente con Zona 3 al 30%.
- La Anomalía Profunda se revela con 12 Indicios Profundos, que no se consumen.
  Su lectura es única, cuesta provisionalmente 60 Conocimiento Antiguo y
  35 Sellos Antiguos, y entrega 1 Dato Anómalo Profundo.
- Los bonus especiales del 100% de las tres zonas quedan aplazados hasta 4G,
  donde se diseñarán junto con las mejoras internas finales.
- Todos los valores numéricos anteriores son provisionales para runs largas.

### 8.9 Investigación del Ente

Se desbloquea al reunir Datos Básicos, Simbólicos y Profundos. Consume
Conocimiento Antiguo como combustible y progresa de 0% a 100%:

| Hito | Requisito | Recompensa |
| ---: | --- | --- |
| 30% | Fragmentos Base + Datos Básicos | Conocimiento del Ente I |
| 60% | Inscripciones + Datos Simbólicos | Conocimiento del Ente II |
| 85% | Sellos + Datos Profundos | Conocimiento del Ente III |
| 100% | combinación de los tres recursos | Pacto con el Ente |

Recompensas tentativas de Conocimiento del Ente: 1, 2 y 3 puntos en los tres
primeros hitos. Debido a que el total es limitado, se recomienda tratarlo como
progreso permanente o diseñar precios completos antes de permitir que se
gaste. No se deben crear compras irreversibles capaces de bloquear el pacto.

Falta definir la conversión de Conocimiento Antiguo a progreso, velocidad,
actividad online/offline y comportamiento de los bloqueos de hito.

Decisión aprobada para el Bloque 4F — 22 de julio de 2026:

- La Investigación se desbloquea permanentemente al reunir 1 Dato Anómalo de
  cada tipo y dispone de una vista propia dentro de Civilización 3.
- Se inicia y pausa manualmente. Una vez iniciada progresa online y offline;
  si falta combustible queda esperando y se reanuda al obtenerlo.
- Conversión provisional: 1 Conocimiento Antiguo por 1% de progreso.
- Velocidad provisional: 1% cada 30 segundos, para 50 minutos activos hasta 100%.
- Se detiene exactamente en 30%, 60%, 85% y 100% sin consumir excedentes.
- Hito 30%: 25 Fragmentos Base + 1 Dato Básico; recompensa 1 Conocimiento del Ente.
- Hito 60%: 35 Inscripciones + 1 Dato Simbólico; recompensa 2 Conocimientos del Ente.
- Hito 85%: 45 Sellos + 1 Dato Profundo; recompensa 3 Conocimientos del Ente.
- Hito 100%: 50 de cada recurso de zona; prepara el Pacto para 4G.
- Cada hito se paga una sola vez. Los Datos no tienen otros gastos antes de sus hitos.
- El Conocimiento del Ente es acumulativo y permanente, con total inicial de 6;
  no puede gastarse hasta definir completamente 4G.
- Todos los números anteriores son provisionales para runs largas.

Decisión aprobada e implementada para el Bloque 4G — 22 de julio de 2026:

- Establecer el Pacto no tiene coste adicional: el hito 100% de 4F ya pagó su
  preparación.
- El Pacto tiene tres líneas independientes de tres niveles:
  - Expedición Resonante: +10% acumulación de restos adicionales por nivel.
  - Archivo Inagotable: +10% Conocimiento Antiguo y recursos de zona obtenidos
    por análisis por nivel.
  - Memoria Compartida: +3% resultados positivos repetibles de Civilizaciones
    1 y 2 por nivel.
- El Conocimiento del Ente no se gasta. Sirve como umbral permanente: nivel 1
  requiere 1, nivel 2 requiere 3 y nivel 3 requiere 6.
- Cada línea cuesta por nivel: 50/100/150 Conocimiento Antiguo y 25/50/75 de
  cada uno de los tres recursos de zona.
- Memoria Compartida mejora llegada de Seguidores, producción de Altares,
  recompensas materiales de Peregrinación, formación de Acólitos, progreso del
  Lugar de Vínculo, generación de Miembros, reducción de Dominio y Cobertura.
  No reduce costes ni mejora Amenaza, pérdidas o Represalias; no toca la Máquina.
- Bonus especiales por completar zonas al 100%:
  - Zona 1: +10% acumulación de restos adicionales en todas las zonas.
  - Zona 2: análisis de todas las zonas 10% más rápidos.
  - Zona 3: +10% Conocimiento Antiguo y recursos de zona por análisis.
- Los efectos externos Modulador y Prestigio 1 se reservan para el Bloque 5.
- Los números se mantienen provisionales hasta las pruebas de balance largas.

### 8.10 Pacto y utilidad tardía

Al 100% se desbloquea el pacto con un Ente neutral cuyo interés es recibir
conocimiento. Su función propuesta es mejorar internamente Dimensión 2:

- costes de pactos menores;
- eficiencia de Ritos;
- progreso de Civ 1 y Civ 2;
- Archivo y Anomalías;
- requisitos internos;
- condiciones de pactos mayores.

Como Civ 3 se desbloquea tarde, sus beneficios no deben limitarse a acelerar
contenido que el jugador ya terminó. Deben mejorar progresión repetible,
mantenimiento o niveles posteriores de los pactos mayores de Civ 1 y Civ 2.

Subdivisiones recomendadas:

- 4A: estado, Zona 1, excavación, restos y Erudito 1;
- 4B: análisis, recursos, progreso de zona y Archivo inicial;
- 4C: Zona 2, Archivo medio e Indicios;
- 4D: Anomalías y Datos Anómalos;
- 4E: Zona 3 y recursos avanzados;
- 4F: Investigación y Conocimiento del Ente;
- 4G: pacto y mejoras internas de Dimensión 2.

## 9. Alcance del Bloque 5 — Pactos mayores e integración final

### 9.1 Objetivo

Cerrar el arco de las tres civilizaciones y conectar Dimensión 2 con el resto
del juego sin que sus recompensas vuelvan irrelevantes sus propios sistemas.

### 9.2 Contradicción que debe resolverse

Existen dos distribuciones históricas:

1. Civ 1 mejora Cuarto 1 + Cuarto 2, Civ 2 mejora Dimensión 1 y Civ 3 mejora
   Dimensión 3.
2. Civ 1 mejora Cuarto 1, Civ 2 mejora Cuarto 2 y Civ 3 mejora internamente
   Dimensión 2.

Ninguna se considera todavía definitiva. Debe elegirse o diseñarse una versión
híbrida antes de programar efectos permanentes. Hasta entonces, cada bloque
puede preparar el estado y desbloqueo de su pacto, pero no inventar beneficios
externos.

### 9.3 Reglas comunes propuestas para pactos mayores

- Contactar al Ente desbloquea un sistema progresivo, no un bonus inmediato y
  único.
- Cada pacto posee niveles o líneas con requisitos visibles.
- Los recursos avanzados de su civilización siguen teniendo utilidad después
  del primer contacto.
- Los beneficios se desbloquean gradualmente.
- Ningún pacto debe ser obligatorio para corregir un sistema base roto.
- Deben evitarse ciclos multiplicativos ilimitados entre dimensiones.
- El estado completo se guarda y admite migración.
- El progreso offline no duplica recompensas ni mantenimiento.

Decisión aprobada para el Bloque 5A — 22 de julio de 2026:

- Se conserva la matriz de cinco líneas por civilización: tres internas y dos
  externas, todas con tres niveles y sin mejorar la Máquina.
- Civilización 1 conserva su Lugar de Vínculo actual:
  - Camino Peregrino: +5% llegada y recompensas materiales por nivel.
  - Oficio Sagrado: +7,5% producción de Altares por nivel.
  - Orden de Acólitos: +5% formación y potencia de Ritos por nivel.
  - Eco del Santuario: +1% producción de LE por nivel.
  - Liturgia de Trazas: +1% producción de Trazas por nivel.
- El pacto mayor de Civilización 2 se establece sin coste adicional tras una
  Contención exitosa. Los Miembros asignados generan Estabilidad de Contención
  a razón de raíz cuadrada de Miembros por minuto, online y offline.
- Costes por línea de Civilización 2: niveles 1/2/3 cuestan 20/40/60
  Estabilidad y 3/6/9 Fragmentos de Control.
- Líneas de Civilización 2:
  - Red Reconstituida: +5% generación de Miembros por nivel.
  - Levantamiento Coordinado: +5% reducción de Dominio por nivel.
  - Defensa Vinculada: +5% Cobertura y −1 punto porcentual de pérdidas por
    Represalia por nivel.
  - Custodia de Artefactos: +2% producción de Artefactos por nivel.
  - Geometría de Resistencia: +2% efectos positivos del Triángulo por nivel.
- Civilización 3 conserva sus tres líneas internas de 4G y añade:
  - Resonancia del Modulador: +5% velocidad de calibración por nivel.
  - Crónica del Primer Umbral: +1 punto de vista previa de Prestigio 1 por nivel,
    reclamado únicamente mediante el flujo normal de Prestigio.
- Las dos líneas externas de Civilización 3 usan los mismos costes y umbrales
  1/3/6 de sus líneas internas.
- Los bonus permanentes se agregan antes de multiplicar, no crean ciclos
  ilimitados y respetan el mismo progreso online/offline.
- Todos los números quedan provisionales hasta las runs largas de balance.

Implementación del Bloque 5B — 22 de julio de 2026:

- El Lugar de Vínculo existente se formaliza como el Pacto Mayor de
  Civilización 1 sin cambiar niveles, costes, porcentajes ni progreso.
- Preparar el Lugar de Vínculo equivale a establecer el Pacto Mayor después del
  contacto permitido por la civilización.
- La navegación conserva `UMBRAL` antes de establecerlo y muestra `PACTO`
  después; la vista revela su nombre completo únicamente tras el desbloqueo.
- Las cinco líneas, Acólitos asignados, progreso online/offline y conexiones de
  LE/Trazas conservan el comportamiento ya aprobado en 2G.
- La prueba visual y funcional manual fue completada y aprobada.

Implementación del Bloque 5C — 22 de julio de 2026:

- Tras una Contención exitosa puede establecerse, sin coste adicional, el Pacto
  Mayor de Civilización 2.
- Los Miembros asignados al sostenimiento generan Estabilidad de Contención a
  razón de `sqrt(Miembros)` por minuto, tanto online como offline.
- Se implementaron las cinco líneas aprobadas, tres niveles por línea y costes
  20/40/60 de Estabilidad más 3/6/9 Fragmentos de Control.
- Red Reconstituida, Levantamiento Coordinado y Defensa Vinculada se conectan a
  producción de Miembros, reducción de Dominio, Cobertura y pérdidas por
  Represalia.
- Custodia de Artefactos se conecta solamente a los Artefactos de LE de Cuarto 1
  y Geometría de Resistencia a los efectos positivos del Triángulo.
- El estado, progreso, niveles y resultados se guardan y migran con la versión
  9 de Civilización 2.
- Compilación, referencias de escena y validación automática completas; prueba
  visual y funcional manual completada y aprobada.

Implementación del Bloque 5D — 22 de julio de 2026:

- El Pacto con el Ente de Civilización 3 se amplía de tres a cinco líneas sin
  alterar las tres mejoras internas ya aprobadas.
- Resonancia del Modulador aporta +5% de velocidad real de calibración por
  nivel, hasta +15%.
- Crónica del Primer Umbral aporta +1 punto a la vista previa de Prestigio 1
  por nivel, hasta +3; comprar la mejora no entrega puntos directamente y su
  reclamación permanece dentro del flujo normal de Prestigio 1.
- Las dos líneas usan los costes existentes de 50/100/150 Conocimiento Antiguo,
  25/50/75 de cada recurso de zona y umbrales permanentes 1/3/6 de Conocimiento
  del Ente.
- La interfaz presenta las cinco líneas en una sola fila y conserva el mismo
  panel de establecimiento, información, costes y mejora.
- La versión de Civilización 3 pasa a 10; las partidas con tres líneas conservan
  sus niveles y reciben las dos nuevas en nivel 0.
- Compilación, escena, referencias, efectos, migración y validación automática
  completas; prueba visual y funcional manual pendiente.

Corrección del Bloque 4H — 22 de julio de 2026:

- La auditoría contra los tres TXT originales detectó la capa pendiente de
  mejora de Eruditos y Archivo de Civilización 3.
- Los Eruditos contratados son nivel 1 y pueden alcanzar nivel 3. Cada nivel
  adicional reduce 5% la duración de análisis y aumenta 5% el Conocimiento y
  recurso de su zona.
- El Archivo recibe Cartografía Estratificada (+5% excavación), Concordancia
  Anómala (+10% Indicios) y Exégesis Profunda (-10% costes de lectura).
- Los umbrales 1/3/6 de Conocimiento del Ente son permanentes y no gastables.
- Migración, offline, serialización, escena y referencias fueron validados.

Implementación del Bloque 5E — 22 de julio de 2026:

- Se validaron juntas las conexiones de los tres pactos y el funcionamiento
  simultáneo de las tres civilizaciones.
- Una simulación integral de diez minutos produjo los mismos resultados online
  y offline, sin duplicación.
- Se verificaron guardado completo y límite offline de 12 horas.

Implementación funcional del Bloque 5F — 22 de julio de 2026:

- Se revisaron acentos, terminología, navegación y límites de los paneles.
- Se validaron partida nueva, migración desde una partida anterior a D2,
  segunda carga estable, catálogos y versiones finales.
- El cierre es funcional; el balance definitivo y el arte/animación final se
  mantienen como etapas posteriores.

### 9.4 Integración y cierre total

El Bloque 5 incluye:

- diseño final y efectos de los tres pactos mayores;
- conexiones con Cuarto 1, Cuarto 2 y dimensiones externas;
- funcionamiento paralelo de las tres civilizaciones;
- equilibrio de recursos cruzados;
- progreso offline integral;
- localización de todos los textos;
- revisión editorial y terminológica;
- pulido visual final del mapa y paneles;
- validación de partida nueva, antigua y migraciones;
- runs de duración y balance sin DEBUG;
- documentación final de decisiones y pendientes.

Subdivisiones recomendadas:

- 5A: matriz definitiva de recompensas y conexiones;
- 5B: pacto mayor de Civ 1;
- 5C: pacto mayor de Civ 2;
- 5D: pacto mayor de Civ 3;
- 5E: interacciones cruzadas y offline integral;
- 5F: cierre funcional, localización, pulido técnico y validación final.

## 10. Estructura técnica propuesta

Los nombres finales pueden ajustarse después de inspeccionar dependencias, pero
la separación de responsabilidades debe mantenerse.

### Estado

- `Dimension2State.cs`: estado raíz y tipos serializables.
- `D2Civilization1State`: economía y progreso de Civ 1.
- `D2Civilization2State`: regiones, Resistencia, Pactos y Contención.
- `D2Civilization3State`: zonas, restos, Archivo, Anomalías e Investigación.
- Estados pequeños para Altares, peregrinaciones, Ritos y Pactos.
- `GameState` conserva una única referencia raíz a Dimensión 2 cuando sea viable.
- `SaveService` guarda, carga, inicializa y migra el estado.

### Lógica

- `Dimension2System.cs`: coordinación y desbloqueos generales.
- `D2Civilization1System.cs`: ciclo principal de Civ 1.
- `D2Civilization2System.cs`: ciclo regional de Civ 2.
- `D2Civilization3System.cs`: ciclo arqueológico de Civ 3.
- Sistemas auxiliares separados cuando una responsabilidad crezca demasiado,
  especialmente Represalias, Anomalías o Pactos mayores.
- Definiciones o configuración de balance separadas de la lógica.
- El tick de Civ 1 debe poder probarse independientemente de su UI.

Si Civ 1 crece demasiado, debe dividirse por dominios —por ejemplo,
peregrinaciones o ritos— antes de producir otro archivo monolítico.

### Interfaz

- `Dimension2PanelUI.cs`: panel raíz y mapa.
- `D2Civilization1PanelUI.cs`: coordinador visual de Civ 1.
- `D2Civilization2PanelUI.cs`: mapa regional y Resistencia.
- `D2Civilization3PanelUI.cs`: zonas, Archivo y Anomalías.
- Componentes de fila/tarjeta reutilizables para Altares, peregrinaciones, Ritos
  y Pactos.
- La UI consulta al sistema y solicita acciones; no debe contener las fórmulas
  principales ni ser la fuente del estado.

### Configuración

Duraciones, costes, factores, límites y recompensas deben estar centralizados.
No se deben dispersar números de balance por varios scripts o elementos de UI.

### Editor y validación

- Validador integral de Bloque 1.
- Validador por cada subdivisión de Bloque 2.
- Validador por cada subdivisión de Bloques 3 y 4.
- Validador de integración cruzada y pactos mayores para Bloque 5.
- Automatización de conexiones visuales solo después de autorización para tocar
  recursos de Unity.

## 11. Pruebas obligatorias

### Por subdivisión

- inicialización válida;
- acción permitida y acción bloqueada;
- costes exactos;
- asignación sin duplicar población;
- temporizadores online;
- guardado/carga durante una actividad;
- finalización y recompensa única;
- saneamiento de valores inválidos;
- UI actualizada después de cada acción relevante.

### Offline

Se debe comprobar:

- producción pasiva;
- finalización de peregrinaciones y Noviciado;
- ausencia de recompensas duplicadas;
- comportamiento con varias actividades simultáneas;
- límite offline, cuando sea definido;
- consistencia entre simulación online y offline.

Para Civ 2 también se probarán operaciones simultáneas, Represalias,
debilitamientos, desgaste de Pactos, marcas y cooldown de Contención. Para Civ
3 se probarán excavaciones, colas de análisis, indicios, protección contra mala
suerte, hitos de zona e Investigación del Ente.

### Partidas

- partida nueva;
- partida antigua anterior a Dimensión 2;
- partida guardada en mitad de cada actividad temporizada;
- partida DEBUG identificada como tal y no usada para juzgar balance.

### Unity

Al llegar a un hito que necesite Play Mode, se entregará al usuario una lista
exacta de acciones y resultados esperados. Un bloque con conexiones o prueba
jugable indispensable pendiente se describirá como “código terminado,
validación pendiente”, no como terminado.

## 12. Decisiones pendientes antes de las partes afectadas

Estas decisiones no bloquean el inicio del Bloque 1 ni la subdivisión 2A, pero
deben resolverse antes de implementar su sistema correspondiente:

- balance final de producción y costes del Refugio, cuya mecánica provisional ya fue aprobada;
- requisitos de desbloqueo de los tres Altares avanzados;
- condición de desbloqueo del tercer espacio de Rito;
- límites numéricos definitivos de los cinco Ritos;
- cantidad de Pactos de Civilización simultáneos;
- mantenimiento, cancelación y compromisos exactos de esos Pactos;
- límite y reglas de progreso offline;
- efectos y conexiones del pacto mayor;
- presentación visual y animación definitiva de entrada.
- comportamiento del promedio de Dominio al desbloquear una región;
- simultaneidad de operaciones por región;
- límite, consumo o deterioro de Cobertura;
- acumulación máxima del efecto de Espionaje;
- destino de Miembros desgastados por Pactos de Resistencia;
- fórmula canónica de Contención;
- acumulación de marcas durante Alerta;
- costes y niveles de mejoras con Fragmentos de Control;
- duración, coste, espacios e inventario de excavaciones;
- duración, cola y automatización de análisis;
- progreso de zona según calidad de resto;
- bonus al 100% de cada zona;
- mejoras exactas del Archivo;
- anomalías únicas o repetibles y protección contra mala suerte;
- conversión de Conocimiento Antiguo en Investigación del Ente;
- naturaleza acumulativa o gastable del Conocimiento del Ente;
- distribución definitiva de beneficios de los tres pactos mayores.

Todos estos valores se tratarán primero como diseño/balance configurable. El
balance definitivo se realizará después de disponer de una versión funcional y
jugable.

## 13. Protocolo de continuidad entre chats

Cuando el chat se esté haciendo largo, pierda claridad o vaya a comenzar un
bloque grande nuevo:

1. Avisar antes de que el contexto se vuelva insuficiente.
2. Terminar, cuando sea seguro, la subdivisión funcional en curso.
3. Recomendar un chat nuevo en un punto estable.
4. Cuando el usuario lo pida o se recomiende el cambio, entregar un resumen
   completo, autosuficiente y listo para pegar.
5. El resumen debe reproducir completas las reglas de trabajo vigentes, no solo
   enlazarlas o mencionar su nombre.
6. El resumen debe incluir todo el alcance futuro, no únicamente el siguiente
   paso.

El resumen de continuidad debe contener como mínimo:

- objetivo general y alcance actual;
- reglas completas de trabajo y seguridad;
- orden de autoridad;
- estado exacto de Bloques 1 y 2 y sus subdivisiones;
- estado exacto de Civ 2, Civ 3 y la integración final;
- funciones implementadas;
- archivos modificados;
- conexiones realizadas y pendientes en Unity;
- pruebas ejecutadas y resultados;
- errores encontrados y correcciones;
- contradicciones o decisiones pendientes;
- estado de cualquier partida alterada por DEBUG;
- siguiente subdivisión recomendada;
- instrucción de revisar archivos reales antes de modificar;
- instrucción explícita de no usar Git salvo petición.

Inicio recomendado del chat siguiente:

> Revisa primero el resumen completo y después inspecciona directamente los
> archivos actuales del proyecto. Antes de modificar, indica qué scripts o
> archivos tocarías, por qué, qué comportamiento se espera y si será necesario
> pasar a Unity. No uses Git ni modifiques archivos protegidos de Unity sin
> permiso explícito. Continúa desde el primer bloque pendiente sin repetir los
> bloques ya terminados y probados.

## 14. Próximo paso recomendado

Realizar una prueba manual integral de la corrección 4H y los Bloques 5D–5F.
Después, ejecutar runs largas sin DEBUG para ajustar duraciones, costes,
probabilidades y recompensas. El balance provisional no debe confundirse con
el cierre funcional ya alcanzado.
