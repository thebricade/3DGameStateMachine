using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple movement controller for a 3rd person player 
//
public class Player : MonoBehaviour
{

    private Vector3 movementVector;
    private float speed;
    private Rigidbody rb;

    private Animator animator; 
    // Start is called before the first frame update
    void Start()
    {
        //from the first child let's get the animator 
        animator = transform.GetChild(0).GetComponent<Animator>(); //expensive, so we're only doing it once! 
        rb = GetComponent<Rigidbody>();
        speed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        
        //if there is some movement happening let's have the player avatar rotate
        if (movementVector != Vector3.zero)
        {
            Quaternion movementRotation = Quaternion.LookRotation(movementVector); //calculate the rotation we're looking at 
            transform.rotation = Quaternion.Slerp(transform.rotation,movementRotation,0.25f );
        }
        animator.SetBool("Walking",movementVector !=Vector3.zero);  //SetBool allows you to set an animator parameter
        
        
    }

    void CalculateMovement()
    {
        movementVector = new Vector3(
            Input.GetAxis("Horizontal"), //ad left/right
            rb.velocity.y,
            Input.GetAxis("Vertical") //ws  up and down
        );
        rb.velocity = new Vector3(
            movementVector.x *speed,
            movementVector.y,
            movementVector.z *speed
            );
    }
}
