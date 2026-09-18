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
>* [Key features]()
>* [Architecture]()
>* [Key concepts]()
>* [Use examples]()

## 2. Key features :sparkles:

## 3. Architecture :building_construction:

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
