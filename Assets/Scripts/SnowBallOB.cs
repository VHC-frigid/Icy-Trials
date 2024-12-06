using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallOB : MonoBehaviour
{
    public Rigidbody playerRigidbody;
    public float bounceForce;
    private Vector3 velocity;

    private void Update()
    {
        velocity = playerRigidbody.velocity;
    }

    public void OnCollisionEnter(Collision coll)
    {
        var speed = velocity.magnitude;
        if(coll.gameObject.tag == "Player")
        {
            print("Collided with player");
            // When the player collides with the snowball we need to launch the player away.
            Vector3 dir = (coll.gameObject.transform.position - transform.position).normalized;
            Rigidbody rb = coll.gameObject.GetComponent<Rigidbody>();
            Vector3 v = -rb.velocity + dir * bounceForce;
            rb.velocity += v;
        }
    }
}
