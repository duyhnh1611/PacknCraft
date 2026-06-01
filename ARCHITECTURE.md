# ARCHITECTURE

# High-Level System Diagram ############################################

This diagram represents the overall architecture of the game, including core systems, gameplay systems, and the loot/backpack system.

---

## System Flow

```
[ Bootstrap Scene ]
        ↓
[ GameManager ]
        ↓
[ SceneLoader / LoadingManager ]
        ↓
[ Gameplay Scene ]
        ↓
[ GameplayManager ]
        ├── LevelManager
        ├── MissionManager
        ├── RecipeManager
		├── InputHandler
        ├── LootHandler
        └── Backpack System
                ├── BackpackHandler
                ├── PlacedItem
                ├── ItemData
                ├── ItemConfig
                └── ShapeData
```

---

## System Responsibilities

### 1. Core Layer

**Bootstrap Scene**

* Entry point of the application
* Initializes minimal systems to avoid long loading screens

**GameManager**

* Global singleton
* Controls game state and lifecycle

**SceneLoader / LoadingManager**

* Handles asynchronous scene loading
* Prepares assets before gameplay starts

---

### 2. Gameplay Layer

**GameplayManager**

* Central orchestrator of all gameplay systems
* Ensures communication between subsystems

**LevelManager**

* Loads level catalog from JSON, each catalog contains 100 level entries to prevent large file sizes and reduce loading overhead  
* Loads level data from JSON  
* Manages level progression

**MissionManager**

* Tracks objectives and completion state

**RecipeManager**

* Handles crafting logic
* Validates input items and produces output

---

### 3. Loot / Backpack Layer

Loot is a horizontal scroll-view with item featuring Rarity background images.
Backpack is a grid-based inventory system where items occupy multiple cells and can be rotated.

#### Data Flow

```
ItemConfig (static)
      ↓
ItemData (config + rotation)
      ↓
PlacedItem (data + grid position)
      ↓
BackpackHandler (grid management)
      ↓
BackpackInputHandler (player interaction)
```

```
ItemConfig (static)
      ↓
ItemData (config + rotation)
      ↓
LootHandler (scroll view)
      ↓
BackpackInputHandler (player interaction)
```

---

#### Components

**ItemConfig**

* Defines static item properties
* Shape, size, ID, metadata
* Have an Editor script to visualize Shape (2D array) for easier editing

**ItemData**

* Runtime representation
* Adds rotation state to ItemConfig

**PlacedItem**

* Represents item placed in grid
* Contains position and ItemData

**ShapeData**

* Defines item shape using a 2D grid
* Caches all rotated variants for performance

**BackpackHandler**

* Manages grid occupancy
* Validates placement and collision

**BackpackInputHandler**

* Handles drag, drop, and rotation input
* Currently Update-driven (candidate for refactor)

**LootHandler**

* Spawns starting items
* Hold crafted items if couldn't place in Backpack

---

## Data Flow Summary

```
JSON Data (Level / Recipe)
        ↓
Managers parse & initialize runtime data
        ↓
LootHandler generates items
        ↓
Backpack system
        ↓
Player interaction via UI
        ↓
Gameplay systems (crafting, missions)
```

---

## Design Characteristics

* Manager-driven architecture
* Clear separation of concerns
* Data-driven (JSON-based)
* Grid-based inventory with rotation support

---

## Notes

* The architecture favors simplicity and clarity over extreme modularity
* Can be extended with:
  * Save/Load functionality
  * Addressables lifecycle management


# Key Design Decisions and Trade-offs ############################################

## 1. Grid-based Inventory with Square Shape Representation

**Decision:**
Items are represented using a square matrix (`ShapeData.size x size`) with a boolean grid.

**Why:**

* Simplifies placement and collision logic
* Makes rotation easy via matrix transformation
* Works well with grid-based inventory systems

**Trade-offs:**

* Less flexible for irregular shapes (non-square bounds)
* Anchor point (top-left) may cause slight visual offset during drag
* UX is slightly compromised for implementation simplicity

---

## 2. Rotation Cache in ShapeData

**Decision:**
Precompute and cache all 4 rotation states of each item.

**Why:**

* Avoid recalculating rotation every frame during drag
* Improves performance in frequent operations

**Trade-offs:**

* Slight increase in memory usage
* Requires cache invalidation when shape data changes

---

## 3. Manager-driven Architecture

**Decision:**
Use centralized managers (GameManager, GameplayManager, etc.) to coordinate systems.

**Why:**

* Easy to understand and debug
* Fast to implement for small-to-mid scale projects
* Clear entry points for each system

**Trade-offs:**

* Risk of tight coupling between systems
* Can become harder to scale if project grows large
* Less flexible than event-driven or ECS-based architectures

---

## 4. JSON-driven Data (Levels, Recipes)

**Decision:**
Use JSON files instead of ScriptableObjects for gameplay data.

**Why:**

* Enables remote updates without rebuilding the app later on
* Easier to modify and extend outside Unity
* Supports live ops / A/B testing

**Trade-offs:**

* Requires custom parsing and validation
* Harder to debug compared to inspector-based data
* Less type safety

---

## 5. Bootstrap Scene Initialization

**Decision:**
Start the game from a lightweight Bootstrap Scene.

**Why:**

* Avoid long black screens during startup
* Allows controlled initialization of systems

**Trade-offs:**

* Adds an extra step in scene flow
* Requires careful lifecycle management

---

## 6. Update-driven Input Handling

**Decision:**
BackpackInputHandler currently uses Update() for input polling.

**Why:**

* Simple and straightforward to implement
* Works reliably for drag & drop interactions

**Trade-offs:**

* Not optimal for performance
* Harder to scale and maintain compared to event-driven systems

---

# What I Would Improve With More Time ############################################

## 1. Refactor Input System (High Priority)

* Replace Update-based input with event-driven approach
* Use UI event interfaces (e.g., IPointer handlers)

**Impact:**

* Reduce unnecessary per-frame computation
* Improve code structure and maintainability

---

## 2. Asset Management Optimization

* Implement proper Addressables lifecycle (load/unload)
* Group assets per scene/level

**Impact:**

* Reduce memory usage
* Prevent asset leaks during long sessions

---

## 3. Rendering Optimization

* Introduce Sprite Atlas for textures
* Optimize UI canvas batching
* Load/Unload Sprite Atlas per scene/level/popup

**Impact:**

* Reduce draw calls
* Improve GPU performance

---

## 4. UX / Visual Feedback Improvements

* Add animations for:

  * Item rotation
  * Invalid placement
  * Crafting results
* Add VFX and SFX feedback

**Impact:**

* Improve player experience
* Make interactions feel more responsive

---

## 5. Save / Load System

* Persist player progress and inventory state

**Impact:**

* Essential for production-ready game
* Enables longer gameplay sessions

---

## 6. Architecture Scalability

* Introduce event-driven communication between systems
* Reduce direct dependencies between managers

**Impact:**

* Improve modularity
* Make system easier to extend

---

## 7. Code Quality & Maintainability

* Standardize naming conventions
* Add documentation and comments
* Further refactor complex systems (especially BackpackInputHandler)

**Impact:**

* Easier onboarding for new developers
* Long-term maintainability

---

## 8. Localization

* Introduce a localization system for multi-language support
* Externalize all user-facing text (UI, missions, items, etc.)
* Support dynamic language switching at runtime

**Impact:**

* Enable global release readiness
* Improve accessibility for different regions
* Separate content from code for better maintainability

---

## 9. Tutorial System

* Introduce a guided tutorial for first-time players

* Explain core mechanics:

  * Drag & drop items
  * Item rotation
  * Grid placement rules
  * Crafting flow

* Use step-by-step instructions with visual highlights (e.g., hand pointer, overlay), or simply a hint (?) button that show a popup tutorial

* Support contextual tutorial triggers instead of always-on guidance

**Impact:**

* Reduce player confusion due to non-intuitive gameplay
* Improve onboarding experience
* Increase early-stage player retention
* Ensure players understand core systems before progressing

---

## 10. Scene Structure (Home / Gameplay / Result)

* Add dedicated scenes for:

  * Home Scene (main menu, entry point)
  * Gameplay Scene
  * Result Scene (post-game summary)

* Alternatively, use a **single persistent scene** and dynamically load/unload views (Home, Gameplay, Result)

**Impact:**

* Improve UX flow between game states
* Reduce scene loading overhead
* Avoid potential GC spikes caused by frequent scene unloading
* Enable smoother transitions and better performance scalability
