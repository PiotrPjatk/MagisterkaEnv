 using TMPro;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
	[SerializeField]
	MazeVisualization visualization;

	[SerializeField]
	int2 mazeSize = int2(20, 20);

	[SerializeField, Tooltip("Use zero for random seed.")]
	int seed;

	[SerializeField, Range(0f, 1f)]
	float
		pickLastProbability = 0.5f,
		openDeadEndProbability = 0.5f,
		openArbitraryProbability = 0.5f;
	
	Maze maze;
	MazeCellObject[] cellObjects;
	
	public void GenerateMaze()
	{
		maze = new Maze(mazeSize);
		new FindDiagonalPassagesJob
		{
			maze = maze
		}.ScheduleParallel(
			maze.Length, maze.SizeEW, new GenerateMazeJob
			{
				maze = maze,
				seed = seed != 0 ? seed : Random.Range(1, int.MaxValue),
				pickLastProbability = pickLastProbability,
				openDeadEndProbability = openDeadEndProbability,
				openArbitraryProbability = openArbitraryProbability
			}.Schedule()
		).Complete();

		if (cellObjects == null || cellObjects.Length != maze.Length)
		{
			cellObjects = new MazeCellObject[maze.Length];
		}
		visualization.Visualize(maze, cellObjects);

		if (seed != 0)
		{
			Random.InitState(seed);
		}
	}
	
	public void DestroyMaze ()
	{
		if (cellObjects == null)
		{
			return;
		}
		for (int i = 0; i < cellObjects.Length; i++)
		{
			cellObjects[i].Recycle();
		}

		OnDestroy();
	}
	
	void OnDestroy ()
	{
		maze.Dispose();
	}
}
