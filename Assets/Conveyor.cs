using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Conveyor : MonoBehaviour
{
    public float speed;
    public float visualSpeedScalar;

    private Vector3 direction;
    private float currentScroll;

    private void Update()
    {
        // Scroll texture to fake it moving
        currentScroll = currentScroll + Time.deltaTime * speed * visualSpeedScalar;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0, currentScroll);
    }

    // Anything that is touching will move
    // This function repeats as long as the object is touching
    private void OnCollisionStay(Collision otherThing)
    {
        // Get the direction of the conveyor belt 
        // (transform.forward is a built in Vector3 
        // which is used to get the forward facing direction)
        // * Remember Vector3's can used for position AND direction AND rotation
        direction = otherThing.transform.position - transform.parent.parent.parent.position;

        Debug.DrawLine(otherThing.transform.position, transform.parent.parent.parent.position, Color.red, 5);


        direction = Vector3.Cross(direction,new Vector3(0,1,-1));
        Debug.DrawLine(otherThing.transform.position, otherThing.transform.position + direction, Color.green,5);
            /*Quaternion.AngleAxis(90, Vector3.forward) * direction;*/
        direction = direction * speed;

        // Add a WORLD force to the other objects
        // Ignore the mass of the other objects so they all go the same speed (ForceMode.Acceleration)
        otherThing.rigidbody.AddForce(direction, ForceMode.Acceleration);
    }
}