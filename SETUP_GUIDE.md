# Quick Setup Guide

## Step-by-Step Unity Setup

### 1. Create Unity Project
1. Open Unity Hub
2. Click "New Project"
3. Select "3D Core" template
4. Name: "TurnBasedStrategy"
5. Location: This folder
6. Create project

### 2. Import Scripts
All scripts are already in `Assets/Scripts/` folder. Unity will compile them automatically.

### 3. Create Hex Tile Prefabs

#### Method 1: Simple Cubes (Quick)
For each terrain type, create a prefab:

**Plains Tile:**
```
1. GameObject → 3D Object → Cube
2. Scale: (1.732, 0.1, 1.5) to approximate hexagon
3. Add green material
4. Drag to Assets/Prefabs/PlainsTile.prefab
5. Delete from scene
```

**Forest Tile:**
```
- Same as Plains but with dark green material
- Save as ForestTile.prefab
```

**Mountain Tile:**
```
- Same as Plains but scale Y to 0.3 (taller)
- Gray material
- Save as MountainTile.prefab
```

#### Method 2: Actual Hexagons (Better)
Download or create hexagon 3D models and use those instead.

### 4. Create Unit Prefabs

**Infantry:**
```
1. GameObject → 3D Object → Cube
2. Scale: (0.5, 0.5, 0.5)
3. Create blue material, apply it
4. Add empty component (Unit script will be added at runtime)
5. Save as Assets/Prefabs/Infantry.prefab
```

**Cavalry:**
```
- Cylinder, scale (0.4, 0.5, 0.4)
- Cyan material
- Save as Cavalry.prefab
```

**Artillery:**
```
- Capsule, scale (0.3, 0.5, 0.3)
- Red material
- Save as Artillery.prefab
```

### 5. Setup Main Scene

1. Create new scene: File → New Scene
2. Save as `Assets/Scenes/MainGame.unity`

#### Create GameManager GameObject:
```
1. Create Empty GameObject, name it "GameManager"
2. Add Components:
   - GameManager
   - TurnManager
   - MapManager
   - UnitManager
   - ResourceManager
```

#### Configure MapManager:
```
Inspector → MapManager:
- Map Radius: 5
- Hex Size: 1.0
- Plains Tile Prefab: [Drag PlainsTile]
- Forest Tile Prefab: [Drag ForestTile]
- Mountain Tile Prefab: [Drag MountainTile]
```

#### Configure UnitManager:
```
Inspector → UnitManager:
- Infantry Prefab: [Drag Infantry]
- Cavalry Prefab: [Drag Cavalry]
- Artillery Prefab: [Drag Artillery]
- Infantry Data: [Drag InfantryData from ScriptableObjects]
- Cavalry Data: [Drag CavalryData]
- Artillery Data: [Drag ArtilleryData]
```

#### Configure GameManager:
```
Inspector → GameManager:
- Player Is Human: false (for AI vs AI)
- Number Of Players: 2
- Starting Units Per Player: 2
```

#### Setup Camera:
```
Main Camera:
- Position: (0, 15, -10)
- Rotation: (45, 0, 0)
```

### 6. Fix ScriptableObject GUIDs (Important!)

The .asset files need proper script GUIDs. To fix:

1. Select all three .asset files in ScriptableObjects folder
2. Delete them
3. In Unity: Assets → Create → TBS → Unit Data
4. Create three unit data assets:
   - Name: "InfantryData"
   - Configure stats as in original files
5. Repeat for Cavalry and Artillery

OR manually edit the .asset files and replace the GUID line with the actual GUID of the UnitData script (found in UnitData.cs.meta).

### 7. Create Simple UI (Optional)

```
1. GameObject → UI → Canvas
2. Add GameUI component
3. Create UI elements:
   - Text (TMP) for Turn, Player, Wood, Gold, Food
   - Button for End Turn
4. Wire up references in GameUI component
```

### 8. Test the Game

1. Press Play in Unity Editor
2. Watch AI vs AI gameplay
3. Press Space to advance turns
4. Check Console for AI decisions

## Troubleshooting

### "Script missing" errors
- Make sure all scripts compiled without errors
- Check namespace declarations match

### Units not spawning
- Verify prefabs are assigned in UnitManager
- Check UnitData ScriptableObjects are created and assigned

### Hex tiles not appearing
- Verify tile prefabs assigned in MapManager
- Check MapManager.GenerateMap() is called

### AI not working
- Check Console for errors
- Verify AIController is created in GameManager.InitializeGame()

## Testing the AI Hierarchy

Watch the Console log to see:
- "Strategic AI produced [unit type]" - Strategic level
- "Unit [type] attacking/retreating" - Tactical level
- Pathfinding automatically happens in Operational level

## Next Steps (Optional Improvements)

- Add player input handling for human vs AI mode
- Create better 3D models for units and tiles
- Add visual effects for combat
- Display influence map as overlay
- Show waypoint markers
- Add unit selection and movement preview

## Delivery for University

This project demonstrates:
✅ All required game mechanics
✅ 3-level hierarchical AI (Strategic/Tactical/Operational)
✅ A* with tactical pathfinding
✅ Behavior Trees
✅ Influence Maps
✅ Tactical Waypoints
✅ Hexagonal grid (extra complexity)

The code is simple, well-commented, and focuses on AI architecture as required by the specification.
