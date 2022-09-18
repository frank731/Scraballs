using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private Dictionary<string, int> wwords = new Dictionary<string, int>();
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        foreach (string s in gameManager.words)
        {
            wwords.Add(s, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("GameController"))
        {
            string createdWord = "";
            foreach (GameObject ball in BallAlign.Instance.balls)
            {
                createdWord += ball.GetComponent<BallLetter>().character;
            }
            if (wwords.ContainsKey(createdWord))
            {
                foreach (char character in createdWord)
                {
                    gameManager.score += gameManager.scores[character];
                    gameManager.UpdateDisplay();
                }
            }
            foreach (GameObject ball in BallAlign.Instance.balls)
            {
                Destroy(ball);
            }
            BallAlign.Instance.balls.Clear();
        }
    }
}
