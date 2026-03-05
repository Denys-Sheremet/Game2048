using Console2048;
using System.Linq;
using Xunit;

namespace Console2048.Test;

public class GameMechanicsTest
{
    [Theory]
    [InlineData(new[] { 0, 2, 0, 4 }, new[] { 2, 4, 0, 0 }, true)]
    [InlineData(new[] { 2, 2, 0, 0 }, new[] { 4, 0, 0, 0 }, true)]
    [InlineData(new[] { 2, 2, 2, 0 }, new[] { 4, 2, 0, 0 }, true)]
    [InlineData(new[] { 2, 2, 2, 2 }, new[] { 4, 4, 0, 0 }, true)]
    [InlineData(new[] { 2, 2, 4, 4 }, new[] { 4, 8, 0, 0 }, true)]
    [InlineData(new[] { 2, 4, 2, 4 }, new[] { 2, 4, 2, 4 }, false)]
    [InlineData(new[] { 0, 0, 0, 0 }, new[] { 0, 0, 0, 0 }, false)]
    [InlineData(new[] { 2, 0, 0, 0 }, new[] { 2, 0, 0, 0 }, false)]
    [InlineData(new[] { 0, 0, 0, 2 }, new[] { 2, 0, 0, 0 }, true)]
    [InlineData(new[] { 2, 2, 4, 0}, new[] { 4, 4, 0, 0 }, true)]
    public void Basic_LogicOf_ProcessLine_Method_Works_AsExpected(int[] lineToProcess, int[] expectedLine, bool expectedWasMoved)
    {
        int nextId = 0;
        Tile?[] tilesToProcess = lineToProcess
            .Select(v => v == 0
                ? null
                : (Tile?)new Tile(nextId++, 0, 0, 0, 0, false, v))
            .ToArray();

        int newId = 0;
        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(tilesToProcess, () => newId++);

        int[] resultLine = result.NewLine.Select(t => t == null
                ? 0
                : t.Value)
            .ToArray();

        Assert.Equal(expectedLine, resultLine);
        Assert.Equal(expectedWasMoved, result.WasMoved);
        Assert.Equal(expectedLine.Length, result.NewLine.Length);
    }

    [Fact]
    public void ProcessLine_Method_SetsCorrect_Information_ForMerged_Tile()
    {
        Tile tile1 = new Tile(1, 0, 0, 0, 0, false, 2);
        Tile tile2 = new Tile(2, 0, 0, 0, 0, false, 2);
        Tile?[] line = { tile1, tile2 };

        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, () => 999);
        Tile? mergedTile = result.NewLine[0];

        Assert.NotNull(mergedTile!.Parents);
        Assert.Equal(1, mergedTile.Parents.Value.Id1);
        Assert.Equal(2, mergedTile.Parents.Value.Id2);
        Assert.True(mergedTile.IsMerged);
    }

    [Fact]
    public void IfThe_Line_IsEmpty_ThereWasNo_Move_WasMoved_IsFalse()
    {
        Tile?[] line = {null, null, null, null};
        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, () => 999);
        bool moved = result.WasMoved;
        
        Assert.False(moved);
    }

    [Fact]
    public void IfNo_PossibleMoves_IsMoved_IsFalse()
    {
        Tile tile1 = new Tile(1, 0, 0, 2);
        Tile tile2 = new Tile(2, 1, 0, 4);
        Tile tile3 = new Tile(3, 2, 0, 8);
        Tile tile4 = new Tile(4, 3, 0, 16);
        Tile?[] line = { tile1, tile2, tile3, tile4 };

        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, () => 999);
        bool moved = result.WasMoved;

        Assert.False(moved);
    }

    [Fact]
    public void ProcessLine_CreatesUnique_Tiles_WhileMerging_AndNot_ChangingOld()
    {
        Tile tile1 = new Tile(1, 0, 0, 2);
        Tile tile2 = new Tile(2, 1, 0, 2);
        Tile?[] line = { tile1, tile2};

        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, () => 3);

        Assert.DoesNotContain(tile1, result.NewLine);
        Assert.DoesNotContain(tile2, result.NewLine);
        Assert.True(result.NewLine.Length == 1);
        Assert.True(result.NewLine.Where(t => t!.Id == 3).Any());
        Assert.NotEqual(tile1, result.NewLine[0]);
    }

    [Fact]
    public void ProcessLine_Method_SetsParents_ToNull_IfThere_WasNo_Merge()
    {
        Tile tile = new Tile(3, 0, 0, 0, 0, true, 4);
        tile.SetParents(1, 2);
        Tile?[] line = { null, tile };

        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, () => 999);
        Tile? mergedTile = result.NewLine[0];

        Assert.False(mergedTile!.IsMerged);
        Assert.Null(mergedTile!.Parents);
    }

    [Theory]
    [InlineData(new[] { 2, 0, 2, 0 }, 4)]
    [InlineData(new[] { 2, 2, 2, 2 }, 8)]
    [InlineData(new[] { 8, 8, 2, 2 }, 20)]
    [InlineData(new[] { 2, 4, 8, 16 }, 0)]
    public void ProcessLine_Method_SetsCorrect_Score(int[] lineToProcess, int expectedScore)
    {
        int nextId = 0;
        Tile?[] tilesToProcess = lineToProcess
            .Select(v => v == 0
                ? null
                : (Tile?)new Tile(nextId++, 0, 0, 0, 0, false, v))
            .ToArray();

        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(tilesToProcess, () => 999);
        int score = result.EarnedScore;

        Assert.Equal(expectedScore, score);
    }

    [Fact]
    public void ProcessLine_Method_ReturnsValid_ListOf_MergedTiles_ToUnregister()
    {
        Tile tile1 = new Tile(1, 0, 0, 0, 0, false, 2);
        Tile tile2 = new Tile(2, 0, 0, 0, 0, false, 2);

        Tile?[] line = { tile1, tile2 };

        GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, () => 999);
        Tile[] expectedMerged = result.MergedTiles;

        Assert.Equal((tile1, tile2), (expectedMerged[0], expectedMerged[1]));
    }

    [Fact]
    public void ProcessLine_ThrowsArgumentNullException_WhenLineIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => GameMechanics.ProcessLine(null!, () => 1));
    }

    [Fact]
    public void ProcessLine_ThrowsArgumentNullException_WhenDelegateIsNull()
    {
        Tile?[] line = new Tile?[4];
        Assert.Throws<ArgumentNullException>(() => GameMechanics.ProcessLine(line, null!));
    }
}
