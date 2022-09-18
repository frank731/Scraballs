using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public XRDirectInteractor leftInt;
    public XRDirectInteractor rightInt;
    public Material[] materials;
    public GameObject seen;
    public int score;
    public string[] words;
    public float timeLeft = 120;
    public Dictionary<char, int> scores;
    public Text HUD;
    public GameObject spawner;
    public ButtonController buttonController;
    public GameObject homeButton;

    public void Start()
    {
        materials = Resources.LoadAll("Materials/Materials").Cast<Material>().ToArray();
        words = System.IO.File.ReadAllLines("Assets/Resources/scrabble_word_list.txt");
        scores = new Dictionary<char, int>()
        {
            {'a',1},{'e',1}, {'i',1},{'l',1},{'n',1},{'o',1},{'r',1},{'s',1},{'t',1},{'u',1},
            {'d',2},{'g',2},
            {'b',3},{'c',3},{'m',3},{'p',3},
            {'f',4}, {'h',4},{'v',4},{'w',4},{'y',4},
            {'k',5},
            {'j',8}, {'x',8},
            {'q',10},{'z',10}
        };
    }

    public void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateDisplay();
        }
        else
        {
            Destroy(spawner);
            buttonController.enabled = false;
            HUD.text = "Game Over!\nFinal score: " + score.ToString() + "\nPress the red button to return to return to the home screen";
            HUD.gameObject.transform.parent.transform.position = new Vector3(0, HUD.gameObject.transform.parent.transform.position.y, HUD.gameObject.transform.parent.transform.position.z);
            homeButton.SetActive(true);
        }
    }

    public void UpdateDisplay()
    {
        float minutes = Mathf.FloorToInt(timeLeft / 60);
        float seconds = Mathf.FloorToInt(timeLeft % 60);
        HUD.text = string.Format("Time: {0:00}:{1:00}\nScore: {2}", minutes, seconds, score);
    }
}

