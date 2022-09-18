using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdhawkApi;

public class GazeCast : MonoBehaviour
{
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void FixedUpdate()
    {
        CheckSight();
    }
    private void LateUpdate()
    {
        CheckSight();
    }

    private void CheckSight()
    {
        RaycastHit hit;
        Ray ray = new Ray(Player.Instance.EyeCenter.position, EyeTrackerAPI.Instance.GazeVector.normalized);
        //Debug.DrawRay(Player.Instance.EyeCenter.position, EyeTrackerAPI.Instance.GazeVector.normalized, Color.green);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6, QueryTriggerInteraction.Collide))
        {
            hit.collider.gameObject.GetComponentInParent<BallMove>().OnGaze();
            if(gameManager.seen != null && hit.collider.gameObject != gameManager.seen)
            {
                gameManager.seen.GetComponentInParent<BallMove>().OffGaze();
                
            }
            gameManager.seen = hit.collider.gameObject;
        }
        
    }
}
