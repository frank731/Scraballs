using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Rigidbody rb;
    public BallLetter bl;
    public float dg;
    void Start()
    {
        rb.AddForce(new Vector3(0, Random.Range(350, 370), 0) * 1.32f);
        OffGaze();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if((collision.collider.CompareTag("Ground") && !bl.grabbed))// || (collision.collider.CompareTag("Table") && !bl.touched))
        {
            Destroy(gameObject);
        }
    }

    public void OnGaze()
    {
        rb.velocity = new Vector3(rb.velocity.x / 3, rb.velocity.y / 3, -1);
        rb.angularVelocity = new Vector3(3.33f, 0, 0);
    }

    public void OffGaze()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -15);
        rb.angularVelocity = new Vector3(10, 0, 0);
    }
}

