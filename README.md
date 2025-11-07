# Cyber Defense 2
**University project — cooperative first-person shooter built in Unity**

Cyber Defense 2 is a continuation of a previous university project.  
It’s a cooperative (co-op) shooter where up to two players fight endless waves of robotic enemies in a cyberpunk environment.  
Players must collaborate to survive, manage resources, and achieve high scores.

---

## Gameplay Overview
- **Genre:** Cooperative shooter  
- **Mode:** Co-op multiplayer
- **Objective:** Survive as long as possible while defeating increasingly difficult enemy waves  
- **Target audience:** Players aged 16+ who enjoy tactical and cooperative gameplay  

---

## Lobby Menu
<img width="1906" height="953" alt="Screenshot 2025-11-06 094004" src="https://github.com/user-attachments/assets/24b89200-304b-49e7-9a30-9c745e3f3a1e" />
<p><em>Lobby interface implemented with Unity UI and Relay services, allowing players to create, list, and join multiplayer sessions in real time.</em></p>

---

## Core Features
- **Co-op Multiplayer** via Unity Relay and Netcode for GameObjects  
- **Adaptive Enemy Waves** that scale with the number of players  
- **Dynamic Pathfinding** with an A* grid-based navigation system  
- **Scoring System** that rewards teamwork and efficiency  
- **Continuous Play:** Endless waves with increasing challenge  

---

## Gameplay Screenshots
<img width="1917" height="1072" alt="Screenshot 2025-11-06 100835" src="https://github.com/user-attachments/assets/99a160a5-df3b-41b0-b10c-2d47a926c909" />
<img width="1919" height="1079" alt="Screenshot 2025-11-06 100637" src="https://github.com/user-attachments/assets/5efc645a-6cd1-4f62-84ca-9a1618269727" />
<img width="1850" height="1061" alt="Screenshot 2025-11-06 100824" src="https://github.com/user-attachments/assets/7aa1709e-38d9-4a68-a411-6e09bc045a9f" />
<p><em>Players face waves of AI-controlled robots that use A* pathfinding to identify and pursue the nearest player, adapting their movement around obstacles and reacting dynamically to the environment.</em></p>

---

## Technical Highlights
- **Pathfinding (A\*)**  
  Implemented grid-based A* navigation for dynamic enemy movement, including runtime obstacle detection and collider updates.  
- **Multiplayer Networking**  
  Built using **Unity Netcode for GameObjects** with **Relay and Lobby Services** to handle two-player cooperative gameplay.  
  Designed an authoritative server structure to minimize desync and maintain synchronized scoring.  
- **Synchronization Challenges**  
  Developed solutions for lag, collider inconsistencies, and timing issues in scene transitions.  
- **Project Architecture**  
  Utilized modular gameplay scripts, Unity Version Control (Plastic SCM), and feature branching for collaboration.  


---

## Level Grid Visualization
<img width="2635" height="1869" alt="Grid" src="https://github.com/user-attachments/assets/d485ab7c-42d1-4e3d-b6e8-693a0408b45b" />
<p><em>Grid-based AI navigation system used for A* pathfinding, dynamic wave spawning, and runtime obstacle updates.</em></p>

---


## Team & Responsibilities
**Jason Ball** – Pathfinding (A*), UI, World setup  
**Florian B.** – Networking, Wave system, Scoreboard  

---

## Development Process
- Independent feature development with regular sync meetings  
- Feature-based branching and version control via Plastic SCM  
- Focused refactoring and bug fixing in later stages  
- Maintained realistic schedule with built-in buffer and consistent progress  

---

## Reflection
The project successfully demonstrated:
- Functional co-op gameplay using Unity’s Netcode stack  
- Custom A* implementation integrated into real-time gameplay  
- Ability to troubleshoot engine and network-level issues  
- Strong collaboration and project organization

---

## Future Plans
- Improve UI and player feedback  
- Enhance collider accuracy and pathfinding precision  
- Increase stability of multiplayer sessions  
- Add more levels and dynamic scaling for additional players  

---

## Assets & Licensing
Some 3D environment assets were removed because they were based on **paid Unity Asset Store content**.  
The uploaded version includes all **scripts, logic, and systems** but excludes proprietary models and textures.

---

## Technologies
**Unity Engine**, **C#**, **Netcode for GameObjects**, **Relay & Lobby Services**, **Plastic SCM**, **GitHub**

---

© 2025 Jason Ball.  
This project was created for educational and portfolio purposes.  
Commercial use or redistribution of included code and assets is not permitted.
