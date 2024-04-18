using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct GenerateMazeJob : IJob
{
    public Tile Tile;
    public int seed;
    public float pickLastProbability, openDeadEndProbability, openArbitraryProbability;

    public void Execute()
    {
        var random = new Random((uint)seed);
        var scratchpad = new NativeArray<(int, TileFlags, TileFlags)>(
            4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
        );

        var activeIndices = new NativeArray<int>(
            Tile.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory
        );
        int firstActiveIndex = 0, lastActiveIndex = 0;
        activeIndices[firstActiveIndex] = random.NextInt(Tile.Length);

        while (firstActiveIndex <= lastActiveIndex)
        {
            bool pickLast = random.NextFloat() < pickLastProbability;
            int randomActiveIndex, index;
            if (pickLast)
            {
                randomActiveIndex = 0;
                index = activeIndices[lastActiveIndex];
            }
            else
            {
                randomActiveIndex = random.NextInt(firstActiveIndex, lastActiveIndex + 1);
                index = activeIndices[randomActiveIndex];
            }


            var availablePassageCount = FindAvailablePassages(index, scratchpad);
            if (availablePassageCount <= 1)
            {
                if (pickLast)
                {
                    lastActiveIndex -= 1;
                }
                else
                {
                    activeIndices[randomActiveIndex] = activeIndices[firstActiveIndex++];
                }
            }
            if (availablePassageCount > 0)
            {
                var passage =
                    scratchpad[random.NextInt(0, availablePassageCount)];
                Tile.Set(index, passage.Item2);
                Tile[passage.Item1] = passage.Item3;
                activeIndices[++lastActiveIndex] = passage.Item1;
            }
        }
        
        if (openDeadEndProbability > 0f)
        {
            random = OpenDeadEnds(random, scratchpad);
        }
        
        if (openArbitraryProbability > 0f)
        {
            random = OpenArbitraryPasssages(random);
        }
    }

    private int FindAvailablePassages(int index, NativeArray<(int, TileFlags, TileFlags)> scratchpad)
    {
        var coordinates = Tile.IndexToCoordinates(index);
        var count = 0;
        if (coordinates.x + 1 < Tile.SizeEW)
        {
            var i = index + Tile.StepE;
            if (Tile[i] == TileFlags.Empty) scratchpad[count++] = (i, TileFlags.PassageE, TileFlags.PassageW);
        }

        if (coordinates.x > 0)
        {
            var i = index + Tile.StepW;
            if (Tile[i] == TileFlags.Empty) scratchpad[count++] = (i, TileFlags.PassageW, TileFlags.PassageE);
        }

        if (coordinates.y + 1 < Tile.SizeNS)
        {
            var i = index + Tile.StepN;
            if (Tile[i] == TileFlags.Empty) scratchpad[count++] = (i, TileFlags.PassageN, TileFlags.PassageS);
        }

        if (coordinates.y > 0)
        {
            var i = index + Tile.StepS;
            if (Tile[i] == TileFlags.Empty) scratchpad[count++] = (i, TileFlags.PassageS, TileFlags.PassageN);
        }

        return count;
    }
    
    int FindClosedPassages (
        int index, NativeArray<(int, TileFlags, TileFlags)> scratchpad, TileFlags exclude
    )
    {
        int2 coordinates = Tile.IndexToCoordinates(index);
        int count = 0;
        if (exclude != TileFlags.PassageE && coordinates.x + 1 < Tile.SizeEW)
        {
            scratchpad[count++] = (Tile.StepE, TileFlags.PassageE, TileFlags.PassageW);
        }
        if (exclude != TileFlags.PassageW && coordinates.x > 0)
        {
            scratchpad[count++] = (Tile.StepW, TileFlags.PassageW, TileFlags.PassageE);
        }
        if (exclude != TileFlags.PassageN && coordinates.y + 1 < Tile.SizeNS)
        {
            scratchpad[count++] = (Tile.StepN, TileFlags.PassageN, TileFlags.PassageS);
        }
        if (exclude != TileFlags.PassageS && coordinates.y > 0)
        {
            scratchpad[count++] = (Tile.StepS, TileFlags.PassageS, TileFlags.PassageN);
        }
        return count;
    }
    Random OpenDeadEnds (
        Random random, NativeArray<(int, TileFlags, TileFlags)> scratchpad
    )
    {
        for (int i = 0; i < Tile.Length; i++)
        {
            TileFlags cell = Tile[i];
            if (cell.HasExactlyOne() && random.NextFloat() < openDeadEndProbability)
            {
                int availablePassageCount = FindClosedPassages(i, scratchpad, cell);
                (int, TileFlags, TileFlags) passage =
                    scratchpad[random.NextInt(0, availablePassageCount)];
                Tile[i] = cell.With(passage.Item2);
                Tile.Set(i + passage.Item1, passage.Item3);
            }
        }
        return random;
    }
    
    Random OpenArbitraryPasssages (Random random)
    {
        for (int i = 0; i < Tile.Length; i++)
        {
            int2 coordinates = Tile.IndexToCoordinates(i);
            if (coordinates.x > 0 && random.NextFloat() < openArbitraryProbability)
            {
                Tile.Set(i, TileFlags.PassageW);
                Tile.Set(i + Tile.StepW, TileFlags.PassageE);
            }
            if (coordinates.y > 0 && random.NextFloat() < openArbitraryProbability)
            {
                Tile.Set(i, TileFlags.PassageS);
                Tile.Set(i + Tile.StepS, TileFlags.PassageN);
            }
        }
        return random;
    }
    
}