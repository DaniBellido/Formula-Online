using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScanner : MonoBehaviour
{
    public float raycastLength = 5f;
    public static bool takeOver = false;
  


    // Update is called once per frame
    void Update()
    {
        // Get position and direction of the FRONT Raycast
        Vector3 frontRayOrigin = this.transform.position; // Origin can be adjusted adding offset value
        Vector3 frontRayDirection = this.transform.forward;

        // Get position and direction of the RIGHT Raycast
        Vector3 rightRayOrigin = this.transform.position;
        Vector3 rightRayDirection = this.transform.right;

        // Get position and direction of the LEFT Raycast
        Vector3 leftRayOrigin = this.transform.position; 
        Vector3 leftRayDirection = -this.transform.right;



        // Raycast forward
        RaycastHit hit;
        if (Physics.Raycast(frontRayOrigin, frontRayDirection, out hit, raycastLength))
        {
            // Check if it hits objects tagged as "car"
            if (hit.collider.CompareTag("car"))
            {
                // delete this line
                Debug.Log("FRONT HIT");

                // Add behaviour here
                takeOver = true;
            }
            else
            {
                takeOver = false;
                Debug.Log("HELLOOOOOOOOOOOOOOOO");
            }
        }
        else
        {
            takeOver = false;
            Debug.Log("NOT COLLIDING");
        }
        // Drawing the ray for debbuging purposes
        Debug.DrawRay(frontRayOrigin, frontRayDirection * raycastLength, Color.red);



        // Raycast right side
        if (Physics.Raycast(rightRayOrigin, rightRayDirection, out hit, 2f))
        {
            // Check if it hits objects tagged as "car"
            if (hit.collider.CompareTag("car"))
            {
                // delete this line
                //Debug.Log("RIGHT HIT");

                // Add behaviour here
            }
        }
        // Drawing the ray for debbuging purposes
        Debug.DrawRay(rightRayOrigin, rightRayDirection * 2f, Color.red);


        // Raycast left side
        if (Physics.Raycast(leftRayOrigin, leftRayDirection, out hit, 2f))
        {
            // Check if it hits objects tagged as "car"
            if (hit.collider.CompareTag("car"))
            {
                //delete this line
                //Debug.Log("LEFT HIT");

                // Add behaviour here
            }
        }
        // Drawing the ray for debbuging purposes
        Debug.DrawRay(leftRayOrigin, leftRayDirection * 2f, Color.red);
    }
}







