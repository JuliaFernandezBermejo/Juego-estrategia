# Quick Setup Guide

## Prerequisites
- Windows laptop with Unity installed (2022.3 LTS or newer recommended)
- Git (if cloning the repository)

## Important: What's in the Repository
This repository contains:
- ✅ All C# scripts (`Assets/Scripts/`)
- ✅ Folder structure (`Assets/Prefabs/`, `Assets/Scenes/`, `Assets/ScriptableObjects/`)
- ✅ ScriptableObject asset files (may need GUID fixes - see Step 6)
- ✅ Documentation files

**NOT included** (Unity auto-generates these):
- ❌ Library/ folder (Unity cache)
- ❌ Temp/ folder (temporary files)
- ❌ Prefab files (.prefab) - **you must create these**
- ❌ Scene files (.unity) - **you must create the scene**
- ❌ Materials - **you must create these**

## Step-by-Step Unity Setup

### 1. Clone/Download and Open Project
1. **Clone or download** this repository to your Windows laptop
2. Open **Unity Hub**
3. Click **"Add"** (or "Open")
4. Navigate to and select the project folder (the one containing the `Assets/` folder)
5. If prompted, select Unity version: **2022.3 LTS or newer**
6. Click **"Open"**
7. Unity will import the project and create Library/ and other folders automatically
8. **First time setup takes a few minutes** - Unity is importing and compiling scripts

**Important**: Do NOT create a new Unity project. Just open this existing folder.

### 2. Wait for Script Compilation
Unity will automatically compile all scripts in `Assets/Scripts/`.
1. Wait for compilation to finish (bottom-right corner shows progress)
2. Check Console window (Window → General → Console) for errors
3. **Should show 0 errors** - all scripts should compile successfully
4. If you see errors, check that you have the correct Unity version

### 3. Create Hex Tile Prefabs

#### Method 1: Simple Cubes (Quick)
For each terrain type, create a prefab:

**Plains Tile:**
```
1. In Hierarchy: Right-click → 3D Object → Cube
2. Rename to "PlainsTile"
3. In Inspector → Transform → Scale: X=1.732, Y=0.1, Z=1.5
4. Create Material: Project → Assets → Right-click → Create → Material
5. Name it "GreenMaterial", set color to green (RGB: 0, 255, 0)
6. Drag GreenMaterial onto the cube in the Scene view
7. Drag the cube from Hierarchy to Assets/Prefabs folder (creates prefab)
8. Delete from Hierarchy (keep the prefab in Assets/Prefabs)
```

**Forest Tile:**
```
1. Create cube, rename "ForestTile"
2. Scale: X=1.732, Y=0.1, Z=1.5
3. Create "DarkGreenMaterial" (RGB: 0, 128, 0)
4. Apply material
5. Drag to Assets/Prefabs/ForestTile.prefab
6. Delete from Hierarchy
```

**Mountain Tile:**
```
1. Create cube, rename "MountainTile"
2. Scale: X=1.732, Y=0.3, Z=1.5 (taller)
3. Create "GrayMaterial" (RGB: 128, 128, 128)
4. Apply material
5. Drag to Assets/Prefabs/MountainTile.prefab
6. Delete from Hierarchy
```

#### Method 2: Actual Hexagons (Better)
Download or create hexagon 3D models and use those instead.

### 4. Create Unit Prefabs

**Infantry:**
```
1. Hierarchy → Right-click → 3D Object → Cube
2. Rename to "Infantry"
3. Scale: X=0.5, Y=0.5, Z=0.5
4. Create "BlueMaterial" (RGB: 0, 0, 255), apply it
5. Drag to Assets/Prefabs/Infantry.prefab
6. Delete from Hierarchy
```

**Cavalry:**
```
1. Create Cylinder, rename "Cavalry"
2. Scale: X=0.4, Y=0.5, Z=0.4
3. Create "CyanMaterial" (RGB: 0, 255, 255), apply it
4. Drag to Assets/Prefabs/Cavalry.prefab
5. Delete from Hierarchy
```

**Artillery:**
```
1. Create Capsule, rename "Artillery"
2. Scale: X=0.3, Y=0.5, Z=0.3
3. Create "RedMaterial" (RGB: 255, 0, 0), apply it
4. Drag to Assets/Prefabs/Artillery.prefab
5. Delete from Hierarchy
```

**Note**: The Unit script component will be added automatically at runtime by UnitManager.

### 5. Setup Main Scene

1. Create new scene: File → New Scene → Select "Basic (Built-in)" or "3D"
2. Save scene: File → Save As → Navigate to Assets/Scenes/ → Name: "MainGame"

**Note**: The Scenes, Prefabs, and ScriptableObjects folders already exist in the repo.

#### Create GameManager GameObject:
```
1. In Hierarchy: Right-click → Create Empty
2. Rename to "GameManager"
3. In Inspector, click "Add Component" and search for each:
   - Type "GameManager" → Add
   - Type "TurnManager" → Add
   - Type "MapManager" → Add
   - Type "UnitManager" → Add
   - Type "ResourceManager" → Add
```

#### Configure MapManager:
```
1. Select GameManager in Hierarchy
2. In Inspector, scroll to MapManager component
3. Set Map Radius: 5
4. Set Hex Size: 1.0
5. Assign Prefabs (drag from Assets/Prefabs to Inspector slots):
   - Plains Tile Prefab: Drag PlainsTile prefab
   - Forest Tile Prefab: Drag ForestTile prefab
   - Mountain Tile Prefab: Drag MountainTile prefab
```

#### Configure UnitManager:
```
1. In Inspector, scroll to UnitManager component
2. Assign Unit Prefabs (from Assets/Prefabs):
   - Infantry Prefab: Drag Infantry
   - Cavalry Prefab: Drag Cavalry
   - Artillery Prefab: Drag Artillery
3. Assign Unit Data (from Assets/ScriptableObjects):
   - Infantry Data: Drag InfantryData
   - Cavalry Data: Drag CavalryData
   - Artillery Data: Drag ArtilleryData
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
