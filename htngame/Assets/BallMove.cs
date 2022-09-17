using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Rigidbody rb;
    void Start()
    {
        rb.AddForce(new Vector3(0, Random.Range(470, 480), 0) * 1.3f);
        rb.AddTorque(new Vector3(10, 0, 0));
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -12);
    }
}
