using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseControl : MonoBehaviour
{
    CursorLockMode lockMode;

    void Awake()
    {
        lockMode = CursorLockMode.Locked;
        Cursor.lockState = lockMode;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
