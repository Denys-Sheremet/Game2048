\#### Console 2048 game - focus on logic



\## Implementation of the classic 2048 game on C# with an accent on scalability of architecture, object tracking system and history of states (Memento)



\### Sections



\## 1 Key features (architectural solutions)



\# Persistent Tile Identity - unique id system provides tile life cycle tracking from spawn to merge

\# Advanced Undo System - implementation of Memento pattern, guarantees stable deep undo with no tiles loss

\# Ready for Graphics - Tile class already contains properties for future GUI binds

\# Single responsibility of each class



\## 2 Technical Stack



\# C# / .NET 8

\# LINQ

\# Generic collections



\## 3 Architecture



\# `Tile` - basic tile object's data like `id`, position, `isMerged` flag etc.

\# `Grid` - the main container for tiles with methods to manipulate its state

\# `TileSnapshot` - snapshot of critical information to restore tile from history (memento)

\# `StateSnapshot` - container for `TileSnapshot` objects

\# `IReadOnlyTileRegistry - read only interface for `TileRegistry` class for external access without pointer leak

\# `TileRegistry` - register of tiles in a `Dictionary` for instant access to any tile on the grid by its id with `O(1)` complexity

\# `GameMechanics` - a black box for main logic of the game, here the "move" is being processed. It is pure functions that can be easily tested

\# `Game` - is a controller and a keeper, makes all the manipulations with registry and game grid

\# `Program` - the start file, all the key bindings and game initialization are there

\# `MoveDirection` - is an `Enum` for moves to identify them in `Move()` method



\## 4 How to Run



\# Clone the repository and navigate to the folder:

```

git clone Denys-Sheremet/Console2048

cd Console2048

```

\# Run the project using the .NET CLI:

`dotnet run`

\# Or simply open the solution in Visual Studio and press `F5`



\# Controls:

\# Use arrow keys for moves (Up, Down, Left, Right)

\# Press `Z` to undo your move

\# Press `ESC` to quit the game



\## 5 Future plans



\# full unit-tests coverage

\# fix logic for future animation integration

\# animation integration

\# transfer to graphic framework

\# cross platform release (PC, android, IOS)

