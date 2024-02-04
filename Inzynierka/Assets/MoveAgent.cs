using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveAgent : Agent
{
    [SerializeField] private Transform target;

    [SerializeField] private Transform machineSpawn;
    [SerializeField] private Transform targetSpawn;
    [SerializeField] private float distance;
 
    // Start is called before the first frame update
    public override void OnEpisodeBegin()
    {
        SpawnMachineAndTarget();
    }

    private void SpawnMachineAndTarget()
    {
        transform.position = machineSpawn.position;
        transform.rotation = machineSpawn.rotation;

        target.position = targetSpawn.position;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    { 
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveUp = actions.DiscreteActions[0];
        float moveDown = actions.DiscreteActions[1];
        float moveLeft = actions.DiscreteActions[2];
        float moveRight = actions.DiscreteActions[3];

        Vector3 moveVector = new Vector3(moveRight - moveLeft, 0, moveUp - moveDown);
        transform.position += moveVector * Time.deltaTime;
    }
    
    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        SetReward(100 - distanceToTarget);

        if (distanceToTarget < distance)  
        {
            SetReward(500); 
            EndEpisode();   
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        SetReward((100 - distanceToTarget) - 100);
        EndEpisode();
        
    }
}
