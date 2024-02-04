using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class WarAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform antyTarget;
    [SerializeField] private float rotationSpeed = 10;
    private int rotate = 0;
    
    
    [SerializeField] private float spawnRadius = 10f; // The radius of the circle
    
    public override void OnEpisodeBegin()
    {
        // Spawn the target at a random point on the border of the circle
        SpawnTargetAtRandomBorderPoint();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    { 
        sensor.AddObservation(target.position.x - transform.position.x);
        sensor.AddObservation(target.position.z - transform.position.z);
        
        sensor.AddObservation(target.rotation.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions[0] == 0)
        {
            rotate = -1;
        }
        else
        {
            rotate = 1;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotate * rotationSpeed * Time.deltaTime);
        
        Ray ray = new Ray(transform.position, transform.forward);

        // Draw a line in the Scene view, green if the ray hits something, red otherwise
        if (Physics.Raycast(ray, out RaycastHit hit, 15))
        {
            if (hit.collider.gameObject.CompareTag("target"))
            {
                SetReward(10);
                EndEpisode();
            }
            else
            {
                SetReward(-10);
                EndEpisode();
            }
            
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 15, Color.yellow); // Length of the ray is 100 units
        }
        
    }

    float CalculateAngle()
    {
        // Get the direction vectors from the positions of the objects
        Vector3 directionToObject1 = transform.forward;
        Vector3 directionToObject2 = target.position - transform.position;

        // Normalize the direction vectors (to ensure they are length 1)
        directionToObject1.Normalize();
        directionToObject2.Normalize();

        // Calculate the dot product of the vectors
        float dotProduct = Vector3.Dot(directionToObject1, directionToObject2);

        // Calculate the angle in degrees
        float angleInDegrees = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        // Output the angle to the console
        Debug.Log("The angle between the objects is: " + angleInDegrees + " degrees");
        return angleInDegrees;
    }
    
    private void SpawnTargetAtRandomBorderPoint()
    {
        // Generate a random angle
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

        // Calculate x and z coordinates using the angle
        float x = transform.position.x + spawnRadius * Mathf.Cos(angle);
        float z = transform.position.z + spawnRadius * Mathf.Sin(angle);
        float y = 1; // Assuming the ground is at y = 0, or set to any specific value

        // Set the target's position to the calculated coordinates
        Vector3 targetPosition = new Vector3(x, y, z);
        target.position = targetPosition;

        // Calculate position for antyTarget on the opposite side of the circle
        float xAntiTarget = transform.position.x + spawnRadius * Mathf.Cos(angle + Mathf.PI);
        float zAntiTarget = transform.position.z + spawnRadius * Mathf.Sin(angle + Mathf.PI);

        Vector3 antyTargetPosition = new Vector3(xAntiTarget, y, zAntiTarget);
        antyTarget.position = antyTargetPosition;

        // Make target and antyTarget face towards the center of the circle
        target.LookAt(new Vector3(transform.position.x, y, transform.position.z));
        antyTarget.LookAt(new Vector3(transform.position.x, y, transform.position.z));
    }

}
