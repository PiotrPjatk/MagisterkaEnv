using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance;
    [SerializeField] private float moveSpeed = 1;
    
    [SerializeField]
    private MapGenerator MapGenerator;

    [SerializeField] private float rewardScalingFactor = 1;

    // Start is called before the first frame update
    public override void OnEpisodeBegin()
    {
        MapGenerator.DestroyMaze();
        MapGenerator.GenerateMaze();
        SpawnMachineAndTarget();
    }

    private void SpawnMachineAndTarget()
    {
        Vector3 agentPosition = GenerateRandomOddPosition();
        transform.position = agentPosition;

        Vector3 targetPosition;
        do
        {
            targetPosition = GenerateRandomOddPosition();
        } while (Vector3.Distance(agentPosition, targetPosition) < 5);

        target.position = targetPosition;
    }

    private Vector3 GenerateRandomOddPosition()
    {
        // Generates odd numbers between -9 and 9
        int x = Random.Range(-4, 5) * 2 + 1; 
        int z = Random.Range(-4, 5) * 2 + 1; 
        return new Vector3(x, 1, z); 
    }
    
    public override void CollectObservations(VectorSensor sensor)
    { 
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extract continuous actions
        float moveX = actions.ContinuousActions[0]; // Horizontal movement
        float moveZ = actions.ContinuousActions[1]; // Vertical movement
        
        // Calculate the distance to the target before moving
        float distanceToTargetBeforeMovement = Vector3.Distance(transform.position, target.position);

        // Since the continuous actions can range from -1 to 1, we can use them directly
        // to determine the direction and magnitude of movement
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;

        // Apply the movement
        transform.position += movement * moveSpeed * Time.fixedDeltaTime;

        // Calculate the distance to the target after moving
        float distanceToTargetAfterMovement = Vector3.Distance(transform.position, target.position);

        // Check if the agent got closer to the target
        if (distanceToTargetAfterMovement < distanceToTargetBeforeMovement)
        {
            // Calculate how much closer the agent got
            float distanceReduction = distanceToTargetBeforeMovement - distanceToTargetAfterMovement;

            // Reward the agent based on the reduction in distance
            // You can adjust the reward scaling factor as needed
            float reward = distanceReduction * rewardScalingFactor;
            Debug.Log(reward);
            AddReward(reward);
        }
    }


    
    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        SetReward(100 - distanceToTarget);

        // if (distanceToTarget < distance)  
        // {
        //     SetReward(500); 
        //     EndEpisode();   
        // }
    }
    
    // Optional: Define a heuristic for manual testing and debugging
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        // Assume arrow keys control the movement, map them to continuous actions
        continuousActionsOut[0] = Input.GetAxis("Horizontal"); // Left/Right arrows
        continuousActionsOut[1] = Input.GetAxis("Vertical");   // Up/Down arrows
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("target"))
        {
            AddReward(10);
        }
        else
        {
            AddReward(-10);
        }
        EndEpisode();
    }
}
