## Propuesta

Integrantes:
- Carlos Manrique
- Miguel Granada


El proyecto consiste en el desarrollo de un videojuego de plataformas 2D desarrollado en Unity (versión 6000.301f1 LTS), centrado en la acción y la movilidad del personaje a través de escenarios con obstáculos, donde la mecánica principal, es el uso de un yoyo que puede lanzarse para atacar y balancearse. El juego contará con 1 nivele, diseñado para introducir y luego exigir un mayor dominio de las mecánicas principales.
En cuanto al diseño de niveles, antes de cada zona de caída hay checkpoints invisibles que reposicionan al jugador en caso de caer. Los enemigos actúan como obstáculos dinámicos para el jugador y hay elementos coleccionables que otorgan puntos.

El jugador cuenta con un sistema de salud visible como barra naranja en la esquina superior derecha. Recibe daño tanto de enemigos como del entorno. Si llega a cero, el nivel se reinicia. La interfaz también muestra un contador de coleccionables en la parte inferior.

Se contemplan tres tipos de enemigos con comportamientos distintos. El primero es un enemigo saltador que sigue una ruta fija mediante saltos, obligando al jugador a anticipar sus movimientos. El segundo es un enemigo terrestre que patrulla una ruta pero puede detectar al jugador y perseguirlo. El tercero es un enemigo volador que sigue al jugador desde el aire y ataca con proyectiles, combinando movilidad con ataque a distancia. Los parámetros de cada tipo, como velocidad, potencia de salto o cadencia de proyectiles, son ajustables para variar la dificultad. Algunos ataques también pueden aplicar efectos negativos temporales, como empujar fuertemente al jugador o invertir sus controles por un corto periodo. Al invertirse los controles, la dirección de movimiento horizontal se invierte temporalmente, lo que obliga al jugador a reorientarse.

El jugador puede lanzar el yoyo en cualquier momento con clic izquierdo y, dependiendo del contexto, cumple distintos roles. Como arma, permite atacar enemigos a distancia sin necesidad de acercarse directamente. La mecánica más importante del yoyo es el sistema de desplazamiento por balanceo. A lo largo del nivel hay puntos de anclaje representados como círculos blancos. Cuando el jugador se acerca a uno, este se torna amarillo indicando que puede engancharse. Al presionar E, el yoyo sale disparado hacia ese punto y el jugador comienza a balancearse automáticamente como un péndulo, usando A y D para controlar la dirección en la que quiere salir impulsado.

El jugador puede soltarse y volver agarrarse para volver a tener momentum. Para soltarse, basta con presionar Space, lo que además aplica un impulso en la dirección que mire el jugador, permitiendo encadenar el momentum hacia el siguiente punto de anclaje o hacia una plataforma. Si hay varios puntos cercanos, el sistema selecciona automáticamente el más conveniente.

El jugador se mueve con W/A/D o con las flechas del teclado. W y Space sirven para saltar: presionarlo brevemente produce un salto corto, y mantenerlo presionado ejecuta un salto más alto. El juego no tiene menú inicial por el momento, así que arranca directamente. Para cerrar el ejecutable se usa Esc.

## Scripts
Los scripts del proyecto se encuentran en Assets/Scripts dentro de la carpeta Scripting_Plataformero. Las pruebas unitarias están en Assets/Tests . Para ejecutarlas, desde Unity se accede a Window → General → Test Runner, se selecciona la pestaña EditMode y se presiona Run All.


