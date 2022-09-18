using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    public XRDirectInteractor leftInt;
    public XRDirectInteractor rightInt;
    public Material[] materials;

    public void Start()
    {
        materials = Resources.LoadAll("Materials/Materials").Cast<Material>().ToArray();
    }
}
