using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    static List<Stack<MapTile>> tilesPools;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ClearPools ()
    {
        if (tilesPools == null)
        {
            tilesPools = new();
        }
        else
        {
            for (int i = 0; i < tilesPools.Count; i++)
            {
                tilesPools[i].Clear();
            }
        }
    }

    Stack<MapTile> tilePool;
    public MapTile GetInstance ()
    {
        if (tilePool == null)
        {
            tilePool = new();

            tilesPools.Add(tilePool);

        }
        if (tilePool.TryPop(out MapTile instance))
        {
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Instantiate(this);
            instance.tilePool = tilePool;
        }
        return instance;
    }

    public void Recycle ()
    {
        tilePool.Push(this);
        gameObject.SetActive(false);
    }
}
