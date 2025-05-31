# 🕹️ The 30th Floor

**The 30th Floor** es un videojuego 2D tipo **roguelike** con combates tácticos por turnos, desarrollado en Unity como parte de un proyecto de fin de curso. El jugador deberá explorar mazmorras generadas de forma procedural, recoger mejoras y enfrentarse a enemigos mientras intenta avanzar de piso en piso… hasta llegar al número 30.

---

## 🎮 Características principales

- 🧩 **Generación procedural de mazmorras**
- ⚔️ **Combate táctico por turnos** en tablero
- 🔁 **Alta rejugabilidad** con contenido aleatorio en cada partida
- 💡 Interfaz visual clara y adaptativa
- 🔊 **Efectos de sonido y música dinámica** según contexto
- 🌐 Exportación a **Windows** y **WebGL** (jugable desde navegador)
- 📈 Sistema de **Leaderboard online** para registrar puntuaciones

---

## 🗂️ Estructura del proyecto

Assets/
│

├── Data/ → ScriptableObjects (enemigos, parámetros)

├── DungeonProceduralGeneration/ → Lógica de generación procedural

├── Enemies/ → IA, controladores y datos de enemigos

├── FightingScene/ → Sistema de combate, tablero, turnos

├── Game Scrip/ → Gestores globales (GameManager, Audio, Transiciones)

├── Player & Camera/ → Control del jugador y sistema de input

└── UI/ → HUD y elementos visuales (barra de vida, textos)

---

## 🧪 Cómo jugar

1. Ejecuta el juego (versión Windows o WebGL).
2. Desde el **Menú Principal**, accede al **Tutorial** si es tu primera vez.
3. Comienza una partida, explora salas, recoge objetos o enfréntate a enemigos.
4. Si sobrevives al combate, continuarás explorando. Si tu vida llega a 0, la partida termina.
5. Intenta llegar lo más lejos posible y superar tu puntuación anterior.

---

## 🧠 Controles

- **Exploración**: WASD o flechas para moverse.
- **Combate**: Clic en las casillas marcadas para moverse. Pulsa `E` para atacar si estás adyacente a un enemigo.
- **Pausa**: `ESC`

---

## 🛠️ Requisitos Técnicos

- **Unity version recomendada**: 2022.3.x (LTS)
- **Sistemas soportados**: Windows (x64), WebGL
- **Resolución mínima**: 1280x720
- **Dependencias**:
  - Unity Input System
  - UI Toolkit

---

## 👨‍💻 Créditos y autoría

Proyecto desarrollado por [Tu nombre aquí], como Trabajo de Fin de Curso.  
Inspirado en títulos como *The Binding of Isaac*, *Fire Emblem*, y *Helltaker*.

---

## 📚 Licencia y uso

Este proyecto ha sido creado con fines educativos. Algunos assets visuales y sonoros utilizados provienen de fuentes libres de derechos. Si deseas reutilizar o contribuir al proyecto, consulta los archivos de licencia correspondientes o contacta al autor.

---

## 🧭 Roadmap futuro (posibles mejoras)

- Sistema de guardado y progreso entre partidas
- Selección de clases/personajes
- IA enemiga avanzada
- Editor de mazmorras
- Multijugador online real con sincronización de turnos

---

## 🌐 Leaderboard online

El juego incluye un sistema de puntuación con ranking mundial a través de LeaderboardCreator. Sube tu puntuación y compite por llegar más lejos que los demás.

---

¡Gracias por jugar a *The 30th Floor*! 🏆
