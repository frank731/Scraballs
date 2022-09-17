using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ball;
    private void Start()
    {
        StartCoroutine(SpawnBall());
    }

    private IEnumerator SpawnBall()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
        Instantiate(ball, transform.position + new Vector3(Random.Range(-1f, 1f), 0, 0), Quaternion.Euler(transform.eulerAngles + new Vector3(0, Random.Range(-5f, 5f), 0)));
        StartCoroutine(SpawnBall());
    }
}
