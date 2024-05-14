using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject trainingGround;
    void Start()
    {
        Instantiate(trainingGround);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
