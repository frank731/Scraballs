using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("GameController"))
        {
            Destroy(BallAlign.Instance.gameObject);
            Destroy(GameManager.Instance.gameObject);
            SceneManager.LoadScene(0);
        }
    }
}
