namespace Game2048.Core.DTOs;

public record TileTransition
    (
        int TileId,
        TileTransitionType Type,
        int FromX,
        int FromY,
        int ToX,
        int ToY,
        int? ParentId1 = null,
        int? ParentId2 = null
    );
