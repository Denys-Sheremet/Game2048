<div align="center">
  
# Game 2048 Core

![C#](https://img.shields.io/badge/Language-C%23-68217A?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Clean-informational)

>*The UI-agnostic core for [Game 2048](..), with smart Undo / State history support and transition analyzer*

</div>

## 1. About the Project :information_source:

Game2048.Core is a pure business logic module - a UI-agnostic engine providing everything a front-end needs to run the game. It contains no rendering or platform-specific code, so it can be dropped behind any UI layer (MAUI, WPF, console, web, etc.). 
Built with Clean Architecture principles in mind, the project separates game rules, state management, and history/undo logic from presentation concerns, so the core can be tested, reused, and reasoned about independently of any specific framework.

>[!NOTE]
>To go to the main sections:
>* [Key features](#2-key-features-sparkles)
>* [Architecture](#3-architecture-building_construction)
>* [Game modes](#4-game-modes-game_die)
>* [Use examples](#5-use-examples-wrench)
>* [Testing](#6-testing-test_tube)
>* [Tech stack]()
>* [Related projects]()
>* [License]()

## 2. Key features :sparkles:

* **UI-agnostic engine:** This project has no UI dependencies, so it is completely decoupled from the presentation layer (MAUI / WPF / Console).
* **Undo option:** The Memento pattern is used to store state snapshots in the history, providing a stable and deterministic undo system.
* **Transition Analyzer:** Core provides a static class that analyzes the list of transitions for the current turn by comparing two state snapshots.
* **DI-oriented architecture:** The main class `Game.cs` is designed with the Facade pattern in mind, making it a centralized entry point that orchestrates the underlying subsystems — such as grid management, movement logic, score calculation, and state history persistence.
* **Serialization support:** `StateSnapshot` and `TileSnapshot` feature a custom `JsonConverter` for nullable tuples, making the engine state ready to be saved and loaded in JSON format.
* **Event-driven model:** Exposes core events (`OnStateChanged`, `OnScoreGained`, `OnVictory`, `OnGameOver`) to provide a clean, reactive way for the UI to subscribe to engine updates without relying on performance-heavy state polling.


## 3. Architecture :building_construction:

>[!NOTE]
>This section is divided into few sub-sections, you can go to the one you like by using links below:
>* [Project diagram](#project-diagram-bulb)
>* [Turn lifecycle](#turn-lifecycle-arrows_counterclockwise)
>* [Game Mechanics' core concepts](#game-mechanics-core-concepts-gear)
>* [Transition Analyzer's core concepts](#transition-analyzers-core-concepts-crystal_ball)
>* [Project structure](#project-structure-file_folder)

### Architecture overview
The engine is strictly divided into specialized sub-domains to enforce the **Single Responsibility Principle (SRP)**. By isolating responsibilities and communicating through interfaces, subsystems can be easily mocked, tested, or entirely replaced with custom strategies (e.g., injecting deterministic randomness for unit testing).

### Project diagram :bulb:

This class diagram illustrates the engine's decoupled nature. Rather than a monolithic structure, the core utilizes several established design patterns:

* **Facade Pattern (`Game.cs`):** Acts as the central orchestrator. Notice how it relies heavily on Dependency Injection (`uses (DI)`) rather than concrete implementations, delegating tasks to specialized managers.
* **Strategy Pattern (Interfaces):** Subsystems like `ITileSpawner` and `IHistoryManager` have multiple implementations (e.g., `DisabledHistoryManager` vs. `LimitedHistoryManager`). This makes it trivial to introduce new game modes (like a "hardcore" mode with no undo) without touching core logic.
* **Factory Pattern (`GameFactory`):** Centralizes the complex initialization of the `Game` object, ensuring that all necessary dependencies and configs (like grid size and `GameModeType`) are properly wired up before gameplay begins.
* **Memento Pattern (`StateSnapshot`):** The state models are completely separate from the active `Grid` and `Tile` entities. This guarantees that history snapshots remain immutable and safe from unintended reference mutations during gameplay.
* **Stateless Domain Services:** `GameMechanics` and `TransitionAnalyzer` handle the heavy mathematical lifting. By keeping them stateless, they remain highly testable and prevent the `Grid` class from becoming bloated.
* **Template Method:** `TileSpawner.Spawn()` is virtual, overridden by `MultipleTileSpawner` and `DelayedTileSpawner` to alter spawn cadence/quantity while reusing base placement logic.

````mermaid
classDiagram
    direction TB

    %% ==================================================
    %% DOMAIN
    %% ==================================================

    class Game {
        +Grid Grid
        +Move(direction)
        +Undo()
    }

    class Grid {
        +Width
        +Height
        +Score
        +Tiles
    }

    class Tile {
        +Id
        +Value
        +Position
    }

    Game *-- Grid
    Grid *-- Tile


    %% ==================================================
    %% GAME CREATION
    %% ==================================================

    class GameFactory {
        <<factory>>
        +CreateGame(GameConfig config)
    }

    class GameConfig {
        <<model>>
        +Rows
        +Cols
        +GameMode
        +TargetValue
    }

    class GameModeType {
        <<enum>>
        Classic
        ClassicPlus
        Compact
        Extended
        ChillZone
    }

    GameFactory ..> Game : creates
    GameFactory ..> GameConfig : uses
    GameConfig ..> GameModeType : has


    %% ==================================================
    %% DOMAIN SERVICES
    %% ==================================================

    class GameMechanics {
        <<domain logic>>
        +ProcessLine()
    }

    class TransitionAnalyzer {
        <<domain logic>>
        +Analyze()
    }

    Game ..> GameMechanics : processes moves
    Game ..> TransitionAnalyzer : analyzes transitions


    %% ==================================================
    %% TILE SPAWNING
    %% ==================================================

    class ITileSpawner {
        <<interface>>
        +Spawn()
        +TrySpawnAt()
        +Reset()
    }

    class TileSpawner
    class MultipleTileSpawner
    class DelayedTileSpawner

    ITileSpawner <|.. TileSpawner
    TileSpawner <|-- MultipleTileSpawner
    ITileSpawner <|.. MultipleTileSpawner
    TileSpawner <|-- DelayedTileSpawner
    ITileSpawner <|.. DelayedTileSpawner

    Game ..> ITileSpawner : uses (DI)


    %% ==================================================
    %% HISTORY / UNDO
    %% ==================================================

    class IHistoryManager {
        <<interface>>
        +Push()
        +Pop()
        +Clear()
    }

    class HistoryManager
    class LimitedHistoryManager
    class DisabledHistoryManager

    IHistoryManager <|.. HistoryManager
    IHistoryManager <|.. LimitedHistoryManager
    IHistoryManager <|.. DisabledHistoryManager

    Game ..> IHistoryManager : uses (DI)


    %% ==================================================
    %% TILE REGISTRY
    %% ==================================================

    class ITileRegistry {
        <<interface>>
        +Register()
        +Unregister()
        +Clear()
    }

    class TileRegistry

    ITileRegistry <|.. TileRegistry

    Game ..> ITileRegistry : uses (DI)


    %% ==================================================
    %% RANDOMNESS
    %% ==================================================

    class IRandomProvider {
        <<interface>>
        +Next()
    }

    class DefaultRandomProvider

    IRandomProvider <|.. DefaultRandomProvider

    Game ..> IRandomProvider : uses (DI)


    %% ==================================================
    %% STATE / MEMENTO
    %% ==================================================

    class StateSnapshot {
        <<model>>
        +Width
        +Height
        +Score
        +NextId
        +TileSnapshots
    }

    class TileSnapshot {
        <<model>>
        +Id
        +Value
        +Position
        +Parents
    }

    StateSnapshot *-- TileSnapshot

    HistoryManager ..> StateSnapshot : stores
    LimitedHistoryManager ..> StateSnapshot : stores
````

### Turn lifecycle :arrows_counterclockwise:

A single call to `Move(direction)` walks through several subsystems in sequence:

1. A `StateSnapshot` of the grid is taken **before** any mutation (`before`).
2. Each row/column (depending on a `MoveDirection` passed to the parameters) is extracted and handed to the stateless `GameMechanics.ProcessLine()`, which merges tiles and computes score for that line.
3. `TileRegistry` registers new tiles to reflect merged/created tiles.
4. If anything moved, the pre-move snapshot is pushed to `IHistoryManager` (enabling Undo), one or more new tiles are spawned via `ITileSpawner`, and `OnScoreGained` / `OnStateChanged` fire. Otherwise, if there were no moves/merges in the current turn, the method will return an empty `List<TileTransition>` and doesn't spawn new tiles.
5. A second `StateSnapshot` is taken **after** (`after`), and `TransitionAnalyzer.Analyze(before, after)` diffs the two to produce a `List<TileTransition>` — this is what the UI layer consumes to animate tiles frame-by-frame instead of just snapping to the new grid state.

`Undo()` on the other hand follows the sequence below:
1. Just like a `Move()` method it takes a snapshot of the state (`before`).
2. Checks if there is anything left in `IHistoryManager`, returns an empty `List<TileTransition>` if it's empty.
3. `Pop()`s the state snapshot from the `IHistoryManager`.
4. Calls `Grid.Restore(StateSnapshot)` method to apply the snapshot to the current grid. `_nextTileId` is being restored also from the `StateSnapshot`.
5. `TileRegistry` is updated per line: tiles consumed by a merge are unregistered, and the resulting merged tile is registered.
6. Sets the `IsGameOver` flag to `false` and invokes the `OnStateChanged` event.
7. Takes the second snapshot **after** (`after`), and pass two snapshots to the analyzer to get the list of transitions for UI.

>[!IMPORTANT]
> The `Undo()` calls the `TransitionAnalyzer.Analyze()` with an optional flag `isUndo` set to `true`. The codebase of `TransitionAnalyzer` will execute analysis of the reversed sequence of parameters `(after, before)` and then **reverse** the `TileTransitionType` and swaps `FromX/FromY` with `ToX/ToY` for each transition DTO.

### Game Mechanics' core concepts :gear:

`GameMechanics.ProcessLine()` is the single algorithm the entire merge system is built on - `Move()` calls it once per row or column, regardless of direction, and it knows nothing about the grid, the game mode, or where the line came from.

* **Compaction as a Byproduct:** Sliding tiles together isn't a separate step - it happens implicitly. `existingTiles = line.Where(t => t != null)` filters out empty cells while preserving order, so "gravity" falls out naturally from the filter rather than needing its own algorithm.
* **Single-Pass, Merge-Once Rule:** The loop compares each tile only to its immediate neighbor and, on a successful merge, increments `i` an extra step (`i++`). This enforces the classic 2048 rule that three equal tiles in a row merge only the first pair - a tile can never merge twice in the same move.
* **Stateless & Id-Agnostic:** The method doesn't generate tile ids itself - it receives a `Func<int> generateId` delegate from the caller. This keeps `GameMechanics` fully decoupled from `Game`'s id-counter state, making it trivially testable in isolation.
* **Explicit State Hygiene:** Tiles that *don't* merge still get `SetMerged(false)` and `SetParents()` called on them. Since `Tile` instances are mutated and reused across turns, this reset is what prevents a tile's stale `IsMerged`/`Parents` flags from a previous merge bleeding into the next turn's `TransitionAnalyzer` classification - a tile with a lingering non-null `Parents` would be misread as a fresh merge `Result`.
* **Reference-Based Move Detection:** `WasMoved` is computed via `!line.SequenceEqual(finalArray)` — since `Tile` has no overridden equality, this compares by reference. A merge always produces a *new* `Tile` instance, so this single check correctly flags both slides and merges as "moved" without needing separate logic for each.

### Transition Analyzer's core concepts :crystal_ball:

The `TransitionAnalyzer` acts as the architectural bridge between the pure game math and the front-end rendering. Instead of the core dictating UI actions, it utilizes a state-diffing approach.

* **Genealogy-Aware Diffing:** The engine compares the board's `StateSnapshot` `before` a move against the `StateSnapshot` `after`. By tracking immutable `Tile` `Id`s and their `Parents`, it distinguishes a tile that simply moved from one that was consumed by a merge - and outputs a clean `List<TileTransition>` describing exactly what happened, tile by tile.
* **Declarative Animations:** The UI framework (e.g. MAUI) remains completely blind to game rules. It simply reads the generated transaction log's `TileTransitionType` (`Spawn`, `Move`, `Merge`, `Result`, `Split`, `Respawn`, `Stay`, `Disappear`) and executes the corresponding visual animation.
* **Undo for Free:** Because transitions are derived purely from comparing two id-keyed snapshots, reversing a move requires no separate logic - `Analyze()` simply re-runs with the snapshots swapped, then inverts each transition's type and direction. The same diffing engine drives both forward and backward animation.


### Project structure :file_folder:

```text
Directory structure:
└── Game2048.Core/
    ├── README.md                                # You are here
    ├── Game.cs                                  # Entry point class (Facade pattern)
    ├── Game2048.Core.csproj                     # Core project configuration file
    ├── GlobalUsings.cs                          # Global usings among all files
    ├── DTOs/                                    
    │   └── TileTransition.cs                    # DTO for UI providers
    ├── Enums/                                   
    │   ├── GameModeType.cs                      # Enumeration of all game modes added
    │   ├── MoveDirection.cs                     # Enumeration for deterministic swipe directions
    │   └── TileTransitionType.cs                # Enumeration for deterministic transition type for UI
    ├── Factories/
    │   └── GameFactory.cs                       # Factory for different game modes
    ├── Interfaces/
    │   ├── IHistoryManager.cs                   # Interface for History Manager for state control (DI)
    │   ├── IRandomProvider.cs                   # Interface for Random Provider for different modes (DI)
    │   ├── IReadOnlyTileRegistry.cs             # Interface wrapper for TileRegistry to guarantee readonly
    │   ├── ITileRegistry.cs                     # Interface for TileRegistry for Grids Tiles control
    │   └── ITileSpawner.cs                      # Interface for TileSpawner for different modes (DI)
    ├── Logic/
    │   └── TransitionAnalyzer.cs                # Static Analyzer to identify transitions for UI
    ├── Mechanics/
    │   └── GameMechanics.cs                     # Static class for pure logic used by Game Grid calculations
    ├── Models/
    │   ├── GameConfig.cs                        # Model for storing initial and current game settings (mode etc.)
    │   ├── Grid.cs                              # Model as a main Tiles container for Game (Source of truth)
    │   ├── StateSnapshot.cs                     # Easy-weight model of a Grid to serialize and store
    │   ├── Tile.cs                              # Model for Tile object and its mandatory fields (Source of truth)
    │   └── TileSnapshot.cs                      # Easy-weight model of a Tile to serialize and store
    ├── Serialization/
    │   └── NullableIntTupleConverter.cs         # Custom JSON converter for serializing nullable tuples
    └── Services/
        ├── DefaultRandomProvider.cs             # Default implementation of IRandomProvider (DI)
        ├── DelayedTileSpawner.cs                # Implementation of ITileSpawner for 0.5 tile / turn spawn (DI)
        ├── DisabledHistoryManager.cs            # Decoy Implementation of IHistoryManager for modes without Undo (DI)
        ├── HistoryManager.cs                    # Implementation of infinite IHistoryManager (DI, Deprecated / Reference)
        ├── LimitedHistoryManager.cs             # Default implementation of limited IHistoryManager (DI)
        ├── MultipleTileSpawner.cs               # Implementation of ITileSpawner for 2+ tile / turn spawn (DI)
        ├── TileRegistry.cs                      # Default implementation of ITileRegistry (DI)
        └── TileSpawner.cs                       # Default base implementation of ITileSpawner (DI, polymorphism)

```

## 4. Game Modes :game_die:

The Game2048.Core features 5 standard game modes with different game rules. All of them are presented in a table below:

| Mode | Grid Size | Target Value | Spawn Logic (ITileSpawner) | Undo Support (IHistoryManager) |
| :--- | :---: | :---: | :--- | :--- |
| **Classic** | 4x4 | 2048 | Standard (1 tile / turn) | ❌ Disabled |
| **ClassicPlus** | 4x4 | 2048 | Standard (1 tile / turn) | ✅ Limited (5 steps) |
| **Compact** | 3x3 | 1024 | Delayed (1 tile every 2 turns) | ✅ Limited (5 steps) |
| **Extended** | 5x5 | 4096 | Multiple (2 tiles / turn) | ❌ Disabled |
| **ChillZone** | 5x5 | 4096 | Multiple (2 tiles / turn) | ✅ Limited (5 steps) |

> [!IMPORTANT]
> `Classic` and `ClassicPlus` modes' Target Value is 2048, **however** the game allows you to extend the game to 4096.
> The 2048 tile in `Compact` mode is mathematically impossible, therefore there are no extension options for it.

> [!NOTE]
> For more details on how each game mode is configured, refer to the [GameFactory](Factories/GameFactory.cs) file.

## 5. Use examples :wrench:

The basic use case of Game2048.Core is by using `GameConfig`, `Game` and `GameFactory` classes. Here is a quick C# snippet that tells the complete story of a basic game loop:

```csharp
using Game2048.Core;
using Game2048.Core.DTOs;
using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Core.Factories;

GameConfig config = new GameConfig(); //Creates a standard config for Classic game mode

config.SetConfig(GameModeType.ClassicPlus); // Sets config to selected game mode

Game game = GameFactory.CreateGame(config); // Creates a Game object using config info

game.SpawnMultipleTiles(2); // Spawns 2 new tiles for the beginning of the game

var moveTransitions = game.Move(MoveDirection.Right); // Executes move to the right, returns a list of transitions and spawns new tile

var undoTransitions = game.Undo(); // Executes undo if the game mode supports it and returns a list of transitions

game.Clear(); // Resets the whole object to default values
```

You may also want to subscribe for basic events of the `Game` class. By default the `Game` object contains 4 events:

```csharp
public event Action? OnStateChanged; // Is invoked when anything changed on a Grid
public event Action<int>? OnScoreGained; // Is invoked when Score value either increased or decreased
public event Action? OnVictory; // Is invoked when game's ended with victory
public event Action? OnGameOver; // Is invoked when game's ended in a loss (no available moves)
```

Additionally, you can manually control Grid state by using methods in `Game` class or call public `Grid` methods:

```csharp
// To get a snapshot manually
StateSnapshot snapshot1 = game.GetCurrentGridState(); // Returns a snapshot of current Grid
// Or by calling straight from the Grid
StateSnapshot snapshot2 = game.Grid.CreateSnapshot(game.GetNextTileId(withIncrement : false)); // Does the same thing but with full control

// You can restore Grid by calling Restore or ColdRestore method
game.Grid.Restore(snapshot1); // Restores the state from a snapshot WITH attention to present Parents of tiles (needed for history management and UI)
game.Grid.ColdRestore(snapshot2); // Restores the state from a snapshot straight-forward by rebuilding the Grid (perfect for quick redraw)

// You can easily get the Tile from a Grid by using indexers
// X-axis (Columns): Goes from Left (0) to Right.
// Y-axis (Rows): Goes from Top (0) to Bottom.
var tile1 = game.Grid[0, 0]; // Gets the Tile in a top-left corner of the Grid

// To iterate over the whole list of Tiles use the following construction
// It is important to use it with Count property in order to avoid IndexOutOfRangeException
for (int i = 0; i < game.Grid.Count; i++)
{
    var tile = game.Grid[i]; // This takes the tile from the list of existing tiles
}

// You can get a list of empty cells
var emptyCellsList = game.Grid.GetEmptyCells(); // Returns a List<(int x, int y)> - all positions with no tiles

// To find a tile among others by its Id
if (game.Grid.TryFindTile(id : 1, out Tile? foundTile))
{
    int value = foundTile.Value;
}

// To clear the Grid manually
game.Grid.Clear(); // However game.Clear() calls it too
```

>[!IMPORTANT]
> To show the best example of usage and to confirm Core's UI-agnostic architecture, you can view two distinct applications powered by Game2048.Core. You can check out their repositories to see full integration examples:
> * **[Game2048.Maui](../Game2048.Maui):** A full-featured, cross-platform application (Android, iOS, Windows) built with .NET MAUI. It demonstrates how to consume `TileTransition` lists to orchestrate smooth UI animations, implement the MVVM pattern, and manage game history in a modern app.
> * **[Game2048.ConsoleApp](../Game2048.ConsoleApp):** A lightweight, terminal-based implementation. It serves as a perfect example of a simple, synchronous input loop for instant screen redraws without complex animation logic.

## 6. Testing :test_tube:
