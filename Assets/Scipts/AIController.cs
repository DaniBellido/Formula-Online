using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // Reference to the Drive script attached to the AI car
    Drive ds;

    // Reference to the circuit containing waypoints for the AI car to follow
    public Circuit circuit;

    // Sensitivity parameters for controlling acceleration, braking, and steering
    public float accelSensitivity = 0.3f;
    public float brakingSensitivity = 1.1f;
    public float steeringSensitivity = 0.01f;

    // Initialise brake and acceleration values
    float brake = 0;
    float accel = 1.0f;

    // Distance ahead to look for the next waypoint
    public float lookAhead = 10.0f;

    // Current target waypoint position
    Vector3 target;

    // Position of the next waypoint in the circuit
    Vector3 nextTarget;

    // Index of the current waypoint in the circuit
    int currentWP = 0;

    // Total distance to the current target waypoint
    float totalDistanceToTarget;

    // GameObject used as a tracker to predict the next waypoint
    GameObject tracker;

    // Index of the current waypoint being tracked by the AI
    int currentTrackerWP = 0;

    bool turn1 = true;
   

    // Start is called before the first frame update
    void Start()
    {
        // Get the Drive component attached to this GameObject
        ds = this.GetComponent<Drive>();

        // Set the initial target and nextTarget positions to the first waypoint's position and the next waypoint's position
        target = circuit.waypoints[currentWP].transform.position;
        nextTarget = circuit.waypoints[currentWP + 1].transform.position;

        // Calculate the initial distance to the target waypoint
        totalDistanceToTarget = Vector3.Distance(target, ds.rb.transform.position);

        // Create a tracker GameObject and set its initial position and rotation to match the car's position and rotation
        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        // Remove the collider component from the tracker GameObject
        DestroyImmediate(tracker.GetComponent<Collider>());
        // Disable the mesh renderer of the tracker GameObject to hide it
        tracker.GetComponent<MeshRenderer>().enabled = false;
        // Set the tracker's initial position to match the car's position
        tracker.transform.position = ds.rb.gameObject.transform.position;
        // Set the tracker's initial rotation to match the car's rotation
        tracker.transform.rotation = ds.rb.gameObject.transform.rotation;
    }


    // ProgressTracker method os responible for updating the tracker's position along the circuit waypoints
    void ProgressTracker() 
    {
        // Draw a debug line from car's position to tracker's position
        Debug.DrawLine(ds.rb.gameObject.transform.position, tracker.transform.position);

        // Check if the distance between the car and the tracker exceeds the lookahead distnce
        if (Vector3.Distance(ds.rb.gameObject.transform.position, tracker.transform.position) > lookAhead) 
        {
            // If so, return without updating the tracker's position
            return;
        }

        // Make the tracker look at the current waypoint's position
        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);

        // Move the tracker forward by a constant speed
        tracker.transform.Translate(0, 0, 1.0f);

        // Check if the tracker has reached the current waypoint
        if (Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWP].transform.position) < 1) // 1 or threshold
        {
            // If so, move to the next waypoint
            currentTrackerWP++;

            // If all waypoints have been visited, reset to the first waypoint
            if (currentTrackerWP >= circuit.waypoints.Length)
                currentTrackerWP = 0;
        }

    }

    // Update is called once per frame
    void Update()
    {
       
        // Progress tracker along the position relative to the car
        ProgressTracker();

        // Calculate local target position relative to the car
        Vector3 localTarget = ds.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);

        // Calculate target angle for steering
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        // Clamp steering angle between -1 and 1
        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ds.currentSpeed);

        // Calculate speed factor relative to maximum speed
        float speedFactor = ds.currentSpeed / ds.maxSpeed;

        // Calculate corner angle and factor
        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90.0f;

        //Debug.DrawLine(circuit.highSpeedCurve.transform.position, ds.rb.transform.position);
       // Debug.Log("Wall Distance: " + Vector3.Distance(circuit.highSpeedCurve[0].transform.position, ds.rb.transform.position));
        Debug.Log("Speed: " + ds.currentSpeed);




        //brake = 0;
        //accel = 1.0f;

     

        switch (turn1) 
        {
            case true:
                HighSpeedTurnOneSolution(); break;

            case false: 
                HighSpeedTurnTwoSolution(); break;

        }
        


        // Smooth cornering: gradually reduce speed as the car approaches a curve
        if (corner > 10 && speedFactor > 0.1f) 
        {
            // Calculate brake based on corner angle and speed factor
            brake = Mathf.Lerp(0, 1 + speedFactor * brakingSensitivity, cornerFactor);
        }

        // Predictive steering: adjust acceleration for smooth navigation of curves
        if (corner > 20 && speedFactor > 0.2f) 
        {
            // Calculate acceleration based on corner angle and corner factor
            accel = Mathf.Lerp(0, 1 * accelSensitivity, 1 - cornerFactor);
        }

        //Apply the calculated steering, acceleration, and brake values to the car
        ds.Go(accel, steer, brake);

        
        //Check for skidding and calculate engine sound
        ds.CheckForSkid();
        ds.CalculateEngineSound();

        
    }

    void HighSpeedTurnOneSolution() 
    {
        // Vector desde el coche hacia el punto en la curva
        Vector3 toCurve = circuit.highSpeedCurve[0].transform.position - ds.rb.transform.position;

        // Vector de dirección del coche
        Vector3 carDirection = ds.rb.transform.forward;

        // Normalizamos los vectores para obtener solo la dirección
        toCurve.Normalize();
        carDirection.Normalize();

        // Calculamos el ángulo entre los dos vectores
        float angle = Vector3.Angle(toCurve, carDirection);

        //##CURVE SOLUTION NEEDS TO BE FIXED
        if (Vector3.Distance(circuit.highSpeedCurve[0].transform.position, ds.rb.transform.position) < 120.0f &&
            Vector3.Distance(circuit.highSpeedCurve[0].transform.position, ds.rb.transform.position) > 20.0f && ds.currentSpeed > 65.0f)
        {

            brake = 1.0f;
            accel = 0;

            if (ds.currentSpeed < 70.0f)
                turn1 = false;

        }
        else
        {
            brake = 0;
            accel = 1.0f;
        }
    }

    void HighSpeedTurnTwoSolution()
    {
        // Vector desde el coche hacia el punto en la curva
        Vector3 toCurve = circuit.highSpeedCurve[1].transform.position - ds.rb.transform.position;

        // Vector de dirección del coche
        Vector3 carDirection = ds.rb.transform.forward;

        // Normalizamos los vectores para obtener solo la dirección
        toCurve.Normalize();
        carDirection.Normalize();

        // Calculamos el ángulo entre los dos vectores
        float angle = Vector3.Angle(toCurve, carDirection);

        //##CURVE SOLUTION NEEDS TO BE FIXED
        if (Vector3.Distance(circuit.highSpeedCurve[1].transform.position, ds.rb.transform.position) < 120.0f &&
            Vector3.Distance(circuit.highSpeedCurve[1].transform.position, ds.rb.transform.position) > 20.0f && ds.currentSpeed > 65.0f)
        {

            brake = 1.0f;
            accel = 0;
            if (ds.currentSpeed < 70.0f)
                turn1 = true;


        }
        else
        {
            brake = 0;
            accel = 1.0f;

        }
    }

}


