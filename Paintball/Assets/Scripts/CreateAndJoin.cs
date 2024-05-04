using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public MainMenuSystem menuScript;
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public void CreateRoom()
    {
        Debug.Log(createInput.text);
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        //GameObject.DontDestroyOnLoad();
        PhotonNetwork.LoadLevel("LobbyScene");
    }


}
