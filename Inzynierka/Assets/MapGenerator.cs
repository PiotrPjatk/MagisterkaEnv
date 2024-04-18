using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    MapVisualization visualization;

    [SerializeField]
    int2 mapSize = new int2(20, 20);

    Tile tile;
    [SerializeField]
    private int seed;
    [SerializeField, Range(0f, 1f)]
    public float pickLastProbability, openDeadEndProbability, openArbitraryProbability;

    void Start ()
    {
        tile = new Tile(mapSize);
        new FindDiagonalPassagesJob
        {
            tile = tile
        }.ScheduleParallel(
            tile.Length, tile.SizeEW, new GenerateMazeJob
            {
                Tile = tile,
                seed = seed != 0 ? seed : Random.Range(1, int.MaxValue),
                pickLastProbability = pickLastProbability,
                openDeadEndProbability = openDeadEndProbability,
                openArbitraryProbability = openArbitraryProbability
            }.Schedule()
        ).Complete();
        
        visualization.Visualize(tile);
    }
    
    void OnDestroy ()
    {
        tile.Dispose();
    }
}
