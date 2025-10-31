# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a turn-based strategy game project implemented in C# for Unity (latest version). Players (humans and NPCs) compete for territory control and/or elimination of opponents. The game features a hierarchical AI system with three levels: Strategic (global game AI), Tactical (unit-level AI), and Operational (movement execution).

## Game Core Mechanics

### Units
- Multiple unit types (infantry, cavalry, artillery, aerial, etc.)
- Unit attributes: Cost, Movement Points (PM), Attack Points (PA), Defense Points (PD), Attack Range, preferred/penalized terrain types
- Units can be produced using resources and may require production structures (barracks, factories)

### Map and Terrain
- Grid-based board (can be rectangular or hexagonal cells)
- Different terrain types affect movement and combat
- Special cells provide resources when occupied or when structures are built on them

### Resources
- Types: wood, gold, food, energy, etc.
- Collected by occupying special cells or building structures
- Required for producing new units and structures

### Victory Conditions
- Territory domination
- Elimination of other players
- Capture of specific cells

## AI Architecture

The AI must operate as a hierarchical system with three levels:

### 1. Operational Level (Movement)
- **Purpose**: Execute orders from tactical level, finding optimal routes
- **Implementation**: Use A* or Dijkstra algorithms with tactical pathfinding
- **Considerations**: Distance, terrain type, risk, enemy unit disposition, range
- **Key concept**: Tactical Pathfinding - modified A* with weighted tactical variables

### 2. Tactical Level (Decision Making)
- **Purpose**: Decide actions for each unit in the current turn
- **Implementation**: Behavior Trees, Hierarchical FSM, or hybrid structures
- **Goal**: Maximize short-term benefit while advancing toward strategic objectives
- **Receives**: Orders/goals from Strategic level
- **Outputs**: Specific unit actions

### 3. Strategic Level (Strategy)
- **Purpose**: Coordinate all units to achieve intermediate and final objectives
- **Implementation**: Influence maps, tactical waypoints, fog-of-war
- **Decisions**: Territory conquest, combat engagement, resource production

## Required AI Techniques

### Pathfinding
- **Primary**: Tactical Pathfinding (modified A* with tactical variable weights)
- **Alternative**: Breadth-First Search or Dijkstra for multi-objective shortest paths
- **Advanced**: Hexagonal grid representation (valued positively for complexity)

### Agent Decision Making
- **Required**: More complex than decision trees or simple FSM
- **Recommended**: Behavior Trees, Hierarchical FSM, or hybrid structures
- **Reference**: Chapter 5 "Decision Making" from "Artificial Intelligence for Games" by Ian Millington and John Funge

### Global AI Decision Making

#### Influence Maps
- **Critical**: Systematic procedure to place influence sources (enemy units, production sources)
- **Features**: Propagation modeling (range and decay with distance)
- **Reference**: Section 6.2.2 "Simple Influence Maps" from "Artificial Intelligence for Games"

#### Tactical Waypoints
- **Purpose**: Define refuge zones, attack points, ambush locations
- **Usage**: AI uses them based on current situation
- **Reference**: Section 6.1 "Waypoint Tactics" from "Artificial Intelligence for Games"

#### Fog of War
- **Optional**: Models decision prediction with incomplete board information

### Strategic and Tactical Variables

The AI should use multiple tactical information sources to define and execute strategy. Each source provides a key parameter for evaluation. Examples:

- **Security**: Risk level of a zone based on friendly/enemy troop concentration and vulnerable resources
- **Tactical Refuge Waypoints**: Fixed or dynamic safe points for cover, regrouping, or healing
- **Production Capacity**: Number, type, and distribution of production buildings and units
- **Visibility**: Visible map portions based on terrain, fog of war, and sensors
- **Enemy Proximity**: Detected enemy units within vision or attack range
- **Additional**: Many more parameters can be studied and implemented as needed

Parameters must be measured, normalized, and combined (e.g., with influence maps or weighted evaluation functions) for coherent, adaptive AI decisions.

### Unit and Resource Management
- **Individual AI**: Each NPC has independent AI
- **Tactical Units**: Homogeneous groups of NPCs
- **Resources**: Various types of structures/weapons (attack, defense, communications, etc.)

## Implementation: Unity & C#

This project is implemented in C# for Unity (latest version).

### Unity Project Structure
```
Assets/
├── Scripts/
│   ├── AI/
│   │   ├── Strategic/      # Global AI, influence maps, strategic decisions
│   │   ├── Tactical/        # Unit decision making, behavior trees
│   │   └── Operational/     # Pathfinding algorithms (A*, Dijkstra)
│   ├── Core/
│   │   ├── Units/           # Unit types and attributes
│   │   ├── Map/             # Grid system, terrain, cells
│   │   └── Resources/       # Resource management
│   ├── Managers/            # Game manager, turn manager, etc.
│   └── UI/                  # User interface
├── Scenes/
├── Prefabs/
├── Materials/
└── Resources/
```

### Unity Commands

#### Opening the Project
- Open Unity Hub
- Add project from this directory
- Select the latest Unity LTS version

#### Running the Game
- Open the main scene in `Assets/Scenes/`
- Press Play button in Unity Editor (or F5)

#### Building the Game
```bash
# Build from Unity Editor: File > Build Settings > Build
# Or use Unity command line:
"C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe" -quit -batchmode -projectPath . -buildWindows64Player "Build/Game.exe"
```

#### Running Tests
- Open Unity Test Runner: Window > General > Test Runner
- Run PlayMode tests for integration testing
- Run EditMode tests for unit testing

```bash
# Run tests from command line
"C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe" -runTests -batchmode -projectPath . -testResults results.xml
```

### C# Code Conventions for Unity
- Use `MonoBehaviour` for game objects that need Unity lifecycle methods
- Use ScriptableObjects for data containers (unit types, terrain types)
- Implement AI systems as manager classes (not necessarily MonoBehaviours)
- Use namespaces to organize code (e.g., `TurnBasedStrategy.AI.Strategic`)
- Follow Unity's C# coding standards

## Key References

- **Book**: "Artificial Intelligence for Games" by Ian Millington and John Funge
  - Section 6.3: Tactical Pathfinding
  - Chapter 5: Decision Making
  - Section 6.1: Waypoint Tactics
  - Section 6.2.2: Simple Influence Maps
- **Online**: [Introduction to A* Algorithm](https://www.redblobgames.com/pathfinding/a-star/introduction.html)

## Development Guidelines

### AI Implementation Priority
1. Implement operational level (pathfinding) first with tactical considerations
2. Build tactical level (unit decision making) with behavior trees or hierarchical FSM
3. Create strategic level (global AI) using influence maps and waypoints
4. Integrate all three levels into a cohesive hierarchical system

### Code Organization
- Separate namespaces/folders for each AI level
- Use interfaces (IStrategicAI, ITacticalAI, IPathfinding) for hierarchical communication
- ScriptableObjects for unit definitions, terrain types, and resource configurations
- Modular component-based design for units (UnitController, UnitStats, UnitMovement)
- Flexible grid system supporting both rectangular and hexagonal layouts
- Use Unity's event system or C# events for decoupled communication between systems

### Testing Strategy
- EditMode tests for pathfinding algorithms (pure C# logic)
- EditMode tests for AI decision-making logic
- PlayMode tests for Unity-integrated features (unit movement, combat)
- Integration tests for hierarchical AI communication
- Performance tests for large maps and many units (Unity Profiler)
