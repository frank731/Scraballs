using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneOnButtonPress : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (ControllerInputHandler.Instance.AnyPressed())
        {
            SceneManager.LoadScene(1);
        }
    }
}
