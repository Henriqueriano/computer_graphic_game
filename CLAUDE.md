# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A Unity 6 (6000.3.10f1) 3D maze game using URP. The player navigates a randomly generated maze from entrance to exit under proximity constraints.

## Unity Workflow

No CLI build command — all development is in the Unity Editor:

- **Play/test**: Press Play in the Editor
- **Build**: File > Build Settings
- **Scripts compile automatically** when C# files are saved

Configured for **JetBrains Rider** and **Visual Studio**.

## How to Run the Game

1. Open the project in Unity
2. Create a new empty Scene (or open `Scenes/Project.unity`)
3. Create an empty GameObject → add **`MazeGenerator`** component
4. Press **Play** — the entire maze, player, and camera are spawned in code; no prefabs required

## Architecture

### Entry Point

**`MazeGenerator.cs`** — self-contained scene setup. On `Awake` it:
1. Adds `GameManager` to itself
2. Runs iterative DFS to carve a perfect maze (`width × height` grid, default 7 × 7 → ~64 walls)
3. Instantiates walls and floors as `PrimitiveType.Cube` children (all parented to `MazeGenerator.transform` so `NavMeshSurface` with `CollectObjects.Children` bakes only the maze geometry)
4. Bakes the `NavMeshSurface` **before** placing obstacles
5. Places ≥6 fixed obstacles (`NavMeshObstacle` carve=true) and ≥6 mobile obstacles (`NavMeshObstacle` carve=false + `MobileObstacle`)
6. Spawns a capsule player with `CharacterController` + `PlayerController`
7. Finds or creates `Camera.main` and attaches `CameraLogic`

### Core Scripts

| Script | Responsibility |
|---|---|
| `MazeGenerator.cs` | DFS maze generation, physical construction, NavMesh bake, scene bootstrap |
| `PlayerController.cs` | WASD movement, wall (1 m) and obstacle (0.5 m) distance checks, reset to start, exit detection |
| `MobileObstacle.cs` | Random patrol within cell radius using `Vector3.MoveTowards`; `NavMeshObstacle` component added by generator |
| `GameManager.cs` | Singleton; `Win()` method; `OnGUI` win screen with restart button |
| `CameraLogic.cs` | Third-person follow using `Vector3.Lerp` + `LookAt` in `LateUpdate` |
| `MarkerComponent.cs` | Tag-free marker (`MazeObjectType`: Wall / Obstacle / Exit) used for proximity queries instead of Unity tags |

### Proximity Rule Implementation

`PlayerController` calls `Physics.OverlapSphere` (centered on the CharacterController's geometric center) and checks for `MarkerComponent` — no Unity tag configuration needed.

- Wall check radius: **1.0 m** → `ResetToStart()`
- Obstacle check radius: **0.5 m** → `ResetToStart()`
- 0.5 s grace period after every reset to prevent immediate re-trigger

### Wall Count

7 × 7 maze: 112 possible wall segments − 48 DFS passages carved = **64 walls** (satisfies ≥ 40 requirement).

### Input System

Keyboard polling via `Keyboard.current` (Unity's new InputSystem). The configured `InputSystem_Actions.inputactions` asset is not yet wired to `PlayerController`.

### Rendering

URP with separate **PC** and **Mobile** renderer profiles under `Assets/Settings/`.

### Key Packages

| Package | Version | Purpose |
|---|---|---|
| `com.unity.render-pipelines.universal` | 17.3.0 | URP rendering |
| `com.unity.inputsystem` | 1.19.0 | Input handling |
| `com.unity.ai.navigation` | 2.0.12 | `NavMeshSurface` + `NavMeshObstacle` |
| `com.unity.probuilder` | 6.0.9 | In-editor level geometry |

## Commit Conventions

`feat(scope):`, `refactor(scope):`, `fix(scope):`
