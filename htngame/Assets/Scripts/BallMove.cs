using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Rigidbody rb;
    public BallLetter bl;
    void Start()
    {
        rb.AddForce(new Vector3(0, Random.Range(350, 370), 0) * 1.3f);
        rb.AddTorque(new Vector3(10, Random.Range(0f, 5f), Random.Range(0f, 5f)));
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -15);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground") && !bl.grabbed)
        {
            Destroy(gameObject);
        }
    }
}
