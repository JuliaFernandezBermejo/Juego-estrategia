# Turn-Based Strategy Game - Unity Project

A hexagonal grid-based turn-based strategy game with a 3-level hierarchical AI system implementing Strategic, Tactical, and Operational decision-making.

## Project Structure

```
Assets/
├── Scripts/
│   ├── AI/
│   │   ├── Strategic/      # Strategic AI: Influence maps, waypoints, global decisions
│   │   ├── Tactical/        # Tactical AI: Behavior trees, unit decisions
│   │   ├── Operational/     # Operational AI: A* pathfinding with tactical weights
│   │   └── AIController.cs  # Integrates all 3 AI levels
│   ├── Core/
│   │   ├── Units/           # Unit system, unit data, unit types
│   │   ├── Map/             # Hexagonal grid, cells, terrain
│   │   └── Resources/       # Resource management, structures
│   ├── Managers/            # Game, Turn, and Unit managers
│   └── UI/                  # Simple game UI
├── Scenes/
├── Prefabs/
└── ScriptableObjects/       # Unit data assets
```

## Setup Instructions

### 1. Open in Unity

1. Open Unity Hub
2. Click "Add" and select this project folder
3. Open with Unity 2022.3 LTS or newer

### 2. Create Scene Setup

Create a new scene called "GameScene" in `Assets/Scenes/` with:

#### Required GameObjects:

1. **GameManager** (Empty GameObject)
   - Add `GameManager` component
   - Add `TurnManager` component
   - Add `MapManager` component
   - Add `UnitManager` component
   - Add `ResourceManager` component

2. **Main Camera**
   - Position: (0, 15, -10)
   - Rotation: (45, 0, 0)

3. **Directional Light** (default lighting)

### 3. Create Prefabs

Create simple prefabs in `Assets/Prefabs/`:

#### Hex Tiles:
- **PlainsTile**: 3D Hexagon mesh, green material
- **ForestTile**: 3D Hexagon mesh, dark green material
- **MountainTile**: 3D Hexagon mesh, gray material

#### Units:
- **Infantry**: Cube primitive (0.5, 0.5, 0.5), blue material
- **Cavalry**: Cylinder primitive (0.4, 0.5, 0.4), cyan material
- **Artillery**: Capsule primitive (0.3, 0.5, 0.3), red material

### 4. Configure MapManager

In the Inspector for MapManager:
- Map Radius: 5
- Hex Size: 1.0
- Assign tile prefabs to Plains/Forest/Mountain slots

### 5. Configure UnitManager

In the Inspector for UnitManager:
- Assign unit prefabs (Infantry, Cavalry, Artillery)
- Assign unit data assets from ScriptableObjects folder

### 6. Create UI (Optional)

Create a Canvas with:
- Turn text (TextMeshPro)
- Player text (TextMeshPro)
- Resource displays (Wood, Gold, Food)
- End Turn button

Add `GameUI` component to Canvas and wire up references.

## How to Play

### Controls

- **Space**: Advance turn (in AI vs AI mode)
- **End Turn Button**: End current player's turn

### Game Modes

Set in GameManager Inspector:
- `Player Is Human = false`: AI vs AI (auto-play with Space)
- `Player Is Human = true`: Human vs AI (not fully implemented)

## AI Architecture

### Level 1: Strategic AI (`StrategicAI.cs`)
- **Influence Maps**: Tracks friendly/enemy strength across map
- **Tactical Waypoints**: Attack, Defense, Resource, Rally points
- **Production Decisions**: Decides what units to produce
- **Output**: Provides objectives to Tactical AI

### Level 2: Tactical AI (`TacticalAI.cs`)
- **Behavior Trees**: Decision-making for individual units
- **Priorities**:
  1. Retreat if low health + enemy nearby
  2. Attack enemy if in range
  3. Move to objective (from Strategic AI)
  4. Defend position
- **Output**: Action commands to Operational AI

### Level 3: Operational AI (`Pathfinding.cs`)
- **A* Pathfinding**: Hexagonal grid navigation
- **Tactical Weighting**:
  - Terrain preferences
  - Enemy threat avoidance
  - Movement cost calculation
- **Output**: Optimal movement paths

## Game Mechanics

### Units
- **Infantry**: Balanced (PM:3, PA:5, PD:5, Range:1)
- **Cavalry**: Fast/Fragile (PM:5, PA:6, PD:3, Range:1)
- **Artillery**: Ranged/Slow (PM:2, PA:8, PD:2, Range:3)

### Terrain
- **Plains**: No modifiers (Cost: 1)
- **Forest**: Defense +20%, Movement cost 2
- **Mountain**: Defense +40%, Movement cost 3

### Resources
- **Wood, Gold, Food**: Collected from special cells
- Used to produce units and structures

### Victory Condition
- Control 60% of the map territory

## Key Features Implemented

✅ Hexagonal grid system with axial coordinates
✅ 3 unit types with unique stats
✅ 3 terrain types affecting gameplay
✅ Resource collection and management
✅ Production structures (Barracks, Factory)
✅ Turn-based game flow
✅ **A* pathfinding with tactical weights**
✅ **Behavior Tree decision system**
✅ **Influence maps for strategic planning**
✅ **Tactical waypoints (Attack, Defense, Resource, Rally)**
✅ **3-level hierarchical AI** (Strategic → Tactical → Operational)

## Academic Requirements Met

This implementation fulfills the university project specifications:

1. ✅ Turn-based strategy game with units and territory control
2. ✅ Hexagonal grid (higher complexity)
3. ✅ Multiple unit types with different attributes
4. ✅ Terrain affecting movement and combat
5. ✅ Resource system and production
6. ✅ **Tactical Pathfinding (A* with weights)**
7. ✅ **Behavior Trees (not simple FSM)**
8. ✅ **Influence Maps**
9. ✅ **Tactical Waypoints**
10. ✅ **Strategic variables (Security, Production, Proximity)**
11. ✅ **3-level AI hierarchy fully integrated**

## Notes

- This is a university-level project following KISS principles
- No advanced features like save/load, networking, or extensive testing
- Focus is on AI architecture and game mechanics
- Simple visuals with primitive shapes
- Designed to demonstrate AI concepts, not production-ready gameplay
