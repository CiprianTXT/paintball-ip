using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GamemodeButton : MonoBehaviour
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
            view.RPC("PrevGamemode", RpcTarget.All);
        }
        else
        {
            Debug.Log("Right pressed");
            view.RPC("NextGamemode", RpcTarget.All);
        }
    }
}
