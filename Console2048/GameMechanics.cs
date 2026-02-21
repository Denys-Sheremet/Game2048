using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal static class GameMechanics
{
    //creating a struct to pass all info needed in Game.cs
    public struct ProcessResult
    {
        public Tile?[] NewLine;
        public Tile[] MergedTiles;
        public int EarnedScore;
        public bool WasMoved;
    }

    public static ProcessResult ProcessLine(Tile?[] line, Func<int> generateId)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentNullException.ThrowIfNull(generateId);

        int length = line.Length;
        int totalScore = 0;

        List<Tile?> existingTiles = line.Where(t => t != null).ToList();
        List<Tile?> resultList = new List<Tile?>();

        List<Tile> mergedTiles = new List<Tile>();



        for (int i = 0; i < existingTiles.Count; i++)
        {
            if (i < existingTiles.Count - 1 && existingTiles[i]!.Value == existingTiles[i + 1]!.Value)
            {
                Tile curr = existingTiles[i]!;
                Tile next = existingTiles[i + 1]!;

                mergedTiles.Add(curr);
                mergedTiles.Add(next);

                int newValue = curr.Value * 2;

                Tile merged = new Tile(generateId(), curr.PosX, curr.PosY, curr.PosX, curr.PosY, true, newValue);
                merged.SetParents(curr.Id, next.Id);
                totalScore += newValue;
                resultList.Add(merged);
                i++;
            }
            else
            {
                existingTiles[i]!.SetMerged(false);
                existingTiles[i]!.SetParents();
                resultList.Add(existingTiles[i]);
            }
        }
        while (resultList.Count < length) resultList.Add(null);
        Tile?[] finalArray = resultList.ToArray();
        bool moved = !line.SequenceEqual(finalArray);

        return new ProcessResult
        {
            NewLine = finalArray,
            MergedTiles = mergedTiles.ToArray(),
            EarnedScore = totalScore,
            WasMoved = moved
        };
    }
}
