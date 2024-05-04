using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject lobby;
    public GameObject loading;
    public GameObject menu;

    public void Connect()
    {
        menu.SetActive(false);
        loading.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        lobby.SetActive(true);
        loading.SetActive(false);
    }

    public void Back()
    {
       lobby.SetActive(false);
       menu.SetActive(true);
       PhotonNetwork.Disconnect();
    }

}
