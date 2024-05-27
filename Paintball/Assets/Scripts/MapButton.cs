using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class MapButton : MonoBehaviour
{
    public bool press_for_next = false;
    public UpdateGameSettings updateScript;
    PhotonView view;

    private void Start()
    {
        view = transform.parent.parent.parent.GetComponent<PhotonView>();
    }


    void OnMouseDown()
    {
        if (!press_for_next)
        {
            Debug.Log("Left pressed");
            view.RPC("PrevMap", RpcTarget.All);
            //updateScript.PrevMap();
        }
        else
        {
            Debug.Log("Right pressed");
            view.RPC("NextMap", RpcTarget.All);
        }
    }
}
