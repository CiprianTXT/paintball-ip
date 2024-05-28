using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartButton : MonoBehaviour
{
   
    PhotonView view;
    public UpdateGameSettings gameSettings;
    public GameManagerScript gameManagerScript;

    private void Start()
    {
        view = gameManagerScript.transform.GetComponent<PhotonView>();
    }

    void OnMouseDown()
    {
        view.RPC("StartGame", RpcTarget.All);
    }

    

}
