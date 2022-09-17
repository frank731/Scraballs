using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdhawkApi;

public class SaccadeSelect : MonoBehaviour
{
    GazeDetector gd;
    public Image[] colors;

    public Image image;

    private int nextcol = 0;

    private float timer = 0.0f;
    private float maxTimeForSaccade = 0.1f;

    public void SetSelectedColor(int index)
    {
        nextcol = index;
        timer = maxTimeForSaccade;
    }

    private void Start()
    {
        gd = GetComponent<GazeDetector>();
        gd.OnGazeEnter.AddListener(GazeEntered);
    }

    void GazeEntered()
    {
        if (timer > 0.0f)
        {
            image.color = colors[nextcol].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = 0.0f;
            }
        }

    }
}
