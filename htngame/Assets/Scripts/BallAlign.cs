using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAlign : Singleton<BallAlign>
{
    public List<GameObject> balls;
    public Transform ballHolder;
    private float ballSize = 0.32f;
    public void AlignBalls()
    {
        Vector3 curPos = ballHolder.position;
        curPos.x -= (balls.Count - 1) * ballSize / 2;
        foreach(GameObject ball in balls)
        {
            //ball.transform.SetParent(ballHolder);
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.transform.position = curPos;
            ball.transform.rotation = Quaternion.Euler(0, 0, 0);
            curPos.x += ballSize;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball")) {
            BallLetter bl = collision.gameObject.GetComponent<BallLetter>();
            if (!bl.grabbed)
            {
                balls.Add(collision.gameObject);
                bl.inTable = true;
                AlignBalls();
            }
        }
    }

}
