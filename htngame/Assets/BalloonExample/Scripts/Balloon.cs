using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdhawkApi;

public class Balloon : MonoBehaviour
{

    Vector3 startScale;

    MeshRenderer ren;
    SphereCollider col;

    private void Start()
    {
        startScale = transform.localScale;
        transform.position = Player.Instance.transform.position + (transform.position - Player.Instance.transform.position).normalized;
        ren = GetComponent<MeshRenderer>();
        col = GetComponent<SphereCollider>();
    }

    public void Grow()
    {
        transform.localScale = transform.localScale * (1 + Time.deltaTime);
        if (transform.localScale.magnitude > 1.0f)
        {
            Pop();
        }
    }
    public void Pop()
    {
        transform.localScale = startScale;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        if (ren)
        {
            ren.enabled = false;
            Debug.Log("popping?");
        }
        if (col)
        {
            col.enabled = false;
        }
        yield return new WaitForSeconds(3.0f);
        if (ren)
        {
            ren.enabled = true;
        }
        if (col)
        {
            col.enabled = true;
        }
    }
}
