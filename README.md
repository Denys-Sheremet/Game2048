# Console 2048 Game - Focus on Logic & Architecture
---
### This is an implementation of classic 2048 game using C# and .NET, built with scalable architecture for future updates. The code in repository is being prepared to form a class library - game core, so that it will be used in GUI applications for PC, Android or IOS. However, game logic can be easily tested by running the game in console and running tests in relatable project.

---
## **Sections**

### 1. Key Features (Architectural Solutions)

* **Persistent Tile Identity** � Unique ID system that provides full tile lifecycle tracking from spawn to merge  
* **Advanced Undo System** � Implementation of the Memento pattern that guarantees stable deep undo with no tile loss  
* **Ready for Graphics** � `Tile` class already contains properties for future GUI bindings  
* **Single Responsibility Principle** � Each class has a clearly defined responsibility  

### 2. Technical Stack

* C# / .NET 8  
* LINQ  
* Generic Collections  

### 3. Architecture

* `Tile` � Basic tile object containing data such as `Id`, position, `IsMerged` flag, etc.  
* `Grid` � Main container for tiles with methods to manipulate its state  
* `TileSnapshot` � Snapshot of critical information required to restore a tile from history (Memento)  
* `StateSnapshot` � Container for `TileSnapshot` objects  
* `IReadOnlyTileRegistry` � Read-only interface for the `TileRegistry` class to provide safe external access without reference leakage  
* `TileRegistry` � Registry of tiles stored in a `Dictionary` for instant `O(1)` access by tile ID  
* `GameMechanics` � A black box containing the core game logic. The `Move()` operation is implemented as pure functions and can be easily unit tested  
* `Game` � Controller and state keeper; manages registry and grid interactions  
* `Program` � Entry point of the application; contains key bindings and game initialization  
* `MoveDirection` � `enum` representing move directions used in the `Move()` method  

### 4. How to Run

* Clone the repository and navigate to the folder:

```
git clone Denys-Sheremet/Console2048
cd Console2048
```

* Run the project using the .NET CLI:

`dotnet run`

* Or open the solution in Visual Studio and press `F5`

### Controls

* Use arrow keys for moves (Up, Down, Left, Right)  
* Press `Z` to undo your move  
* Press `ESC` to quit the game  

### 5. Future Plans

* Full unit test coverage  
* Refactor logic for future animation integration  
* Animation integration  
* Migration to a graphical framework  
* Cross-platform release (PC, Android, iOS)