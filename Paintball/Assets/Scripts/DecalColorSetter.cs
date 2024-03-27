using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalColorSetter : MonoBehaviour
{
    public Color splashColor;
    // Start is called before the first frame update
    void Start()
    {
        DecalProjector dp = GetComponent<DecalProjector>();
        dp.material = new Material(dp.material);
        dp.material.SetColor("_Color", splashColor);
    }
}
