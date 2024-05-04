using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MainMenuSystem : MonoBehaviour
{
    public GameObject[] playerPrefabs; // Array of player prefabs

    private Transform model;
    public int currentPrefabIndex = 0;

    public UnityEngine.UI.Slider redSlider, greenSlider, blueSlider;
    //private Color color = new Color(0, 118, 82);
    public GameObject newPlayer;
    private Transform favColorText;

    private void Awake()
    {
        Transform menu = GameObject.Find("MenuCanvas").transform;
        Transform options = GameObject.Find("OptionsCanvas").transform;
        Transform networking = GameObject.Find("NetworkCanvas").transform;
        Transform hostCanvas = GameObject.Find("HostLobby").transform;
        Transform clientCanvas = GameObject.Find("ClientLobby").transform;

        redSlider = GameObject.Find("RedSlider").GetComponent<UnityEngine.UI.Slider>();
        greenSlider = GameObject.Find("GreenSlider").GetComponent<UnityEngine.UI.Slider>();
        blueSlider = GameObject.Find("BlueSlider").GetComponent<UnityEngine.UI.Slider>();

        favColorText = GameObject.Find("ColorText").transform;


        options.gameObject.SetActive(false);
        networking.gameObject.SetActive(false);
        hostCanvas.gameObject.SetActive(false);
        clientCanvas.gameObject.SetActive(false);
    }

    public struct PlayerData: INetworkSerializable
    {
        public byte _red;
        public byte _green;
        public byte _blue;
        public byte playerModelIndex;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _red);
            serializer.SerializeValue(ref _green);
            serializer.SerializeValue(ref _blue);
            serializer.SerializeValue(ref playerModelIndex);
        }
    }



    public PlayerData GetPlayerData()
    {
        PlayerData data;
        
        data._red = (byte)redSlider.value;
        data._green = (byte)greenSlider.value;
        data._blue = (byte)blueSlider.value;
        data.playerModelIndex = (byte)currentPrefabIndex;

        return data;

    }


    private void Start()
    {
        model = GameObject.Find("PlayerCharacterShowcase").transform;
        redSlider.value = 0;
        greenSlider.value = 118;
        blueSlider.value = 82;
        model.rotation = Quaternion.Euler(0, 180f, 0);
        UpdatePlayerModel();
    }

    private void Update()
    {
        // Rotate the model around the y-axis
        model.Rotate(Vector3.up, Time.deltaTime * 30f); // Adjust rotation speed if needed
    }

    public void NextPlayer()
    {
        currentPrefabIndex++;
        if (currentPrefabIndex >= playerPrefabs.Length)
        {
            currentPrefabIndex = 0;
        }
        model.rotation = Quaternion.Euler(0, 130f, 0);
        UpdatePlayerModel();
    }

    public void PrevPlayer()
    {
        currentPrefabIndex--;
        if (currentPrefabIndex < 0)
        {
            currentPrefabIndex = playerPrefabs.Length - 1;
        }
        model.rotation = Quaternion.Euler(0, 130f, 0);
        UpdatePlayerModel();
    }


    public void UpdatePlayerModel()
    {
        // Destroy previous player model
        foreach (Transform child in model)
        {
            Destroy(child.gameObject);
        }

        // Instantiate and set the new player prefab as a child of model
        newPlayer = Instantiate(playerPrefabs[currentPrefabIndex], model.position, model.rotation, model);
        newPlayer.transform.localPosition = Vector3.zero;
        newPlayer.transform.localRotation = Quaternion.identity;

        // Get the Renderer component of the new player prefab
        Renderer playerRenderer = newPlayer.GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            // Get the materials of the renderer
            Material[] materials = playerRenderer.materials;

            // Find and set the color of the "suit_material" instance
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name == "suit_material (Instance)")
                {
                    Color c = new Color(redSlider.value / 255f, greenSlider.value / 255f, blueSlider.value / 255f);
                    materials[i].color = c;
                    materials[i].SetColor("_Color", c);

                    favColorText.GetComponent<TextMeshProUGUI>().color = c;

                    break; // Exit the loop after changing the color
                }
            }
        }
        newPlayer.layer = 11; //UIPlayer
    }

    public void Play()
    {
        newPlayer.transform.parent = null;
        newPlayer.name = "CustomModel";
        GameObject.DontDestroyOnLoad(newPlayer);
        //SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
        UnityEngine.Application.Quit();
        Debug.Log("User has quit the application.");
    }
}
