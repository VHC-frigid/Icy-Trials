using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceForce;
    
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            print("Collided with player");
            // when the player collides with the bounce pad grab the player's rigidboy and apply an up force to it, btw the force is whatever the bounce force is.
            other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }
}
