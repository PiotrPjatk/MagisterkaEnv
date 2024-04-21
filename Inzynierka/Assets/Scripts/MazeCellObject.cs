using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour
{
	static List<Stack<MazeCellObject>> pools;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void ClearPools ()
	{
		if (pools == null)
		{
			pools = new();
		}
		else
		{
			for (int i = 0; i < pools.Count; i++)
			{
				pools[i].Clear();
			}
		}
	}
	 
	[System.NonSerialized]
	Stack<MazeCellObject> pool;

	public MazeCellObject GetInstance ()
	{
		if (pool == null)
		{
			pool = new();
			pools.Add(pool);
		}
		if (pool.TryPop(out MazeCellObject instance))
		{
			instance.gameObject.SetActive(true);
		}
		else
		{
			instance = Instantiate(this);
			instance.pool = pool;
		}
		return instance;
	}

	public void Recycle ()
	{
		pool.Push(this);
		gameObject.SetActive(false);
	}
}
