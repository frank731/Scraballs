using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (cam)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }
    }
}
