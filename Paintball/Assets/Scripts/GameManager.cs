using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    // Start is called before the first frame update
    [Header("PlayerInfo")]
    public GameObject playerIconPrefab;
    public Transform contentShowcase;
    public List<GameObject> playerIcons = new List<GameObject>();
    public Texture[] modelIcons;
    public List<MainMenuSystem.PlayerData> iconsData = new List<MainMenuSystem.PlayerData>();

    [Header("MapInfo")]
    public Transform mapShowcaseImage;
    public Texture[] mapImages;
    public string[] maps;
    [SerializeField] private int currentMapIndex = 0;

    [Header("GamemodeInfo")]
    public Transform gamemodeText;
    public string[] gamemodes;
    [SerializeField] private int currentGamemodeIndex = 0;

    [Header("NetworkInfo")]
    public NetworkManager networkManager;

    public MainMenuSystem menuSys;
    ulong actualId = 0;
    public byte maxPlayers = 16;



    void Start()
    {
        gamemodeText.GetComponent<TMP_Text>().SetText(gamemodes[0]);
        mapShowcaseImage.GetComponent<RawImage>().texture = mapImages[0];

        // Subscribe to network events
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from network events
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    // Callback when a client connects to the server
    private void HandleClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log("Client connected: " + clientId);
            Debug.Log(NetworkManager.ConnectedClientsIds.Count);
            if (NetworkManager.ConnectedClientsIds.Count > maxPlayers)
            {
                Debug.Log("Max player number reached. Disconnecting client.");
                NetworkManager.Singleton.DisconnectClient(clientId);
                return;
            }

            if (clientId == 0) {
                CreateNewIconOnServerRpc(menuSys.GetPlayerData(), new RpcParams());
                playerIcons[0].GetComponent<PlayerIconInfo>().crown.SetActive(true);
                playerIcons[0].GetComponent<PlayerIconInfo>().border.SetActive(true);
            }
        }
        else {
            Debug.Log("Im a client");
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            MainMenuSystem.PlayerData data =  menuSys.GetPlayerData();
            CreateNewIconOnServerRpc(data, new RpcParams());
        }
    }

    // Callback when a client disconnects from the server
    private void HandleClientDisconnect(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log($"Client left {clientId}");
            //RemovePlayerIcon(clientId);
        }
    }


    [Rpc(SendTo.SpecifiedInParams)]
    void SetIdRpc(ulong id, RpcParams senderParams)
    {
        actualId = id;
    }

    [Rpc(SendTo.Server)]
    public void CreateNewIconOnServerRpc(MainMenuSystem.PlayerData data, RpcParams senderParams)
    {
        if (senderParams.Receive.SenderClientId >= maxPlayers)
        {
            return;
        }
        SetIdRpc(senderParams.Receive.SenderClientId, RpcTarget.Single(senderParams.Receive.SenderClientId, RpcTargetUse.Temp));
        if (senderParams.Receive.SenderClientId != 0)
        {
            Debug.Log($"Update {iconsData.Count}");
            for (int i = 0; i < iconsData.Count; i++)
            {
                UploadDataOnNewClientRpc(iconsData[i], RpcTarget.Single(senderParams.Receive.SenderClientId, RpcTargetUse.Temp));
            }

        }

        UpdateIconsOnClientsRpc(data, senderParams.Receive.SenderClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateIconsOnClientsRpc(MainMenuSystem.PlayerData data, ulong senderId)
    {
        Debug.Log($"Im {actualId} and was sent by {senderId}");
        Color col = new Color(data._red / 255f, data._green / 255f, data._blue / 255f);
        GameObject icon = Instantiate(playerIconPrefab, contentShowcase);
        icon.transform.Find("ModelImage").GetComponent<RawImage>().texture = modelIcons[data.playerModelIndex];
        icon.transform.Find("ColorBackground").GetComponent<RawImage>().color = col;
        icon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-470 + 150 * playerIcons.Count, -80);
        playerIcons.Add(icon);
        iconsData.Add(data);

        if (actualId == senderId)
        {
            icon.GetComponent<PlayerIconInfo>().border.SetActive(true);
        }

    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void UploadDataOnNewClientRpc(MainMenuSystem.PlayerData data, RpcParams senderParams)
    {
        
        Color col = new Color(data._red / 255f, data._green / 255f, data._blue / 255f);
        GameObject icon = Instantiate(playerIconPrefab, contentShowcase);
        icon.transform.Find("ModelImage").GetComponent<RawImage>().texture = modelIcons[data.playerModelIndex];
        icon.transform.Find("ColorBackground").GetComponent<RawImage>().color = col;
        icon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-470 + 150 * playerIcons.Count, -80);
        playerIcons.Add(icon);
        iconsData.Add(data);

        //If its the host, activate crown marker
        if (iconsData.Count == 1)
        {
            icon.GetComponent<PlayerIconInfo>().crown.SetActive(true);
        }
    }

    public void NextGamemode()
    {
        currentGamemodeIndex = (currentGamemodeIndex + 1) % gamemodes.Length;
        UpdateGamemodeIndexRpc((byte)currentGamemodeIndex);
    }

    public void PrevGamemode()
    {
        currentGamemodeIndex = (currentGamemodeIndex - 1 + gamemodes.Length) % gamemodes.Length;
        UpdateGamemodeIndexRpc((byte)currentGamemodeIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateGamemodeIndexRpc(byte game)
    {

        gamemodeText.GetComponent<TMP_Text>().SetText(gamemodes[game]);
        currentGamemodeIndex = game;
    }

    public void NextMap()
    {
        currentMapIndex = (currentMapIndex + 1) % mapImages.Length;
        UpdateMapIndexRpc((byte)currentMapIndex);
    }

    public void PrevMap()
    {
        currentMapIndex = (currentMapIndex - 1 + mapImages.Length) % mapImages.Length;
        UpdateMapIndexRpc((byte)currentMapIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateMapIndexRpc(byte map)
    {

        mapShowcaseImage.GetComponent<RawImage>().texture = mapImages[map];
        currentMapIndex = map;
    }

    public void StartGame()
    {
        Debug.Log("Game started");
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();    
    }
    public void DisconectClient()
    {
        NetworkManager.Singleton.DisconnectClient(actualId);
    }

}
