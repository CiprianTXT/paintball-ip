using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSystem : MonoBehaviour
{
    public GameObject[] playerPrefabs; // Array of player prefabs
    private Transform model;
    private int currentPrefabIndex = 0;
    private Slider redSlider, greenSlider, blueSlider;
    //private Color color = new Color(0, 118, 82);
    public GameObject newPlayer;

    private void Awake()
    {
        Transform menu = GameObject.Find("MenuCanvas").transform;
        Transform options = GameObject.Find("OptionsCanvas").transform;

        redSlider = GameObject.Find("RedSlider").GetComponent<Slider>();
        greenSlider = GameObject.Find("GreenSlider").GetComponent<Slider>();
        blueSlider = GameObject.Find("BlueSlider").GetComponent<Slider>();

        options.gameObject.SetActive(false);
    }



    private void Start()
    {
        model = GameObject.Find("PlayerCharacterShowcase").transform;
        redSlider.value = 0;
        greenSlider.value = 118;
        blueSlider.value = 82;
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
        UpdatePlayerModel();
    }

    public void PrevPlayer()
    {
        currentPrefabIndex--;
        if (currentPrefabIndex < 0)
        {
            currentPrefabIndex = playerPrefabs.Length - 1;
        }
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
                    materials[i].color = new Color(redSlider.value / 255f, greenSlider.value / 255f, blueSlider.value / 255f);
                    break; // Exit the loop after changing the color
                }
            }
        }
        newPlayer.layer = 11;
    }

    public void Play()
    {
        newPlayer.transform.parent = null;
        GameObject.DontDestroyOnLoad(newPlayer);
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("User has quit the application.");
    }
}
