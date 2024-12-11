using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallOB : MonoBehaviour
{
    public Rigidbody playerRigidbody;
    public float bounceForce;
    private Vector3 velocity;

    public void Start()
    {
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }
    private void Update()
    {
        velocity = playerRigidbody.velocity;
    }

    public void OnCollisionEnter(Collision coll)
    {
        var speed = velocity.magnitude;
        if(coll.gameObject.tag == "Player")
        {
            // When the player collides with a snowball calculate the direction & normal, And grab the player's rigid body.
            print("Collided with player");
            Vector3 dir = (coll.gameObject.transform.position - transform.position).normalized;
            Rigidbody rb = coll.gameObject.GetComponent<Rigidbody>();
            // use the rigidbody and the direction times the public bounce force to use apply velocity to the player.
            Vector3 v = -rb.velocity + dir * bounceForce;
            rb.velocity += v;
        }
    }
}
