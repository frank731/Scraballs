using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BallLetter : MonoBehaviour
{
    public char character;
    public bool grabbed;
    public bool inTable;
    public Rigidbody rb;
    public XRGrabInteractable grab;
    public GameManager gameManager;
    void Start()
    {
        gameManager = GameManager.Instance;
        int charInd = Random.Range(0, 25);
        GetComponent<MeshRenderer>().material = gameManager.materials[charInd];
        character = (char)(charInd + 65);
    }
    public void OnGrab()
    {
        if(!grabbed)
        {
            grabbed = true;
            //Debug.Log("grabbed");
            if (inTable)
            {
                Debug.Log("out");
                if(BallAlign.Instance.balls.Contains(gameObject)) BallAlign.Instance.balls.Remove(gameObject);
                BallAlign.Instance.AlignBalls();
                rb.isKinematic = false;
                inTable = false;
            }
        }
        
    }

    public void OnDrop()
    {
        //if (!gameManager.leftInt.IsSelecting(grab) && !gameManager.rightInt.IsSelecting(grab)) return;
        if(grabbed) grabbed = false;
        rb.isKinematic = false;
    }
   
}
