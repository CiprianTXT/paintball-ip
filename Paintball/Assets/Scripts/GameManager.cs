using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("PlayerInfo")]
    public GameObject[] playerIcons;
    public GameObject[] players;

    [Header("MapInfo")]
    public Texture[] mapImages;
    public string[] maps;
    public Transform mapShowcaseImage;
    private int currentMapIndex = 0;


    [Header("GamemodeInfo")]
    public Transform gamemodeText;
    public string[] gamemodes;
    private int currentGamemodeIndex = 0;


    void Start()
    {
        
        gamemodeText.GetComponent<TMP_Text>().SetText(gamemodes[0]);
        mapShowcaseImage.GetComponent<RawImage>().texture = mapImages[0];

    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void NextGamemode()
    {
        currentGamemodeIndex++;
        if (currentGamemodeIndex >= gamemodes.Length)
        {
            currentGamemodeIndex = 0;
        }
        gamemodeText.GetComponent<TMP_Text>().SetText(gamemodes[currentGamemodeIndex]);
    }

    public void PrevGamemode()
    {
        currentGamemodeIndex--;
        if (currentGamemodeIndex < 0)
        {
            currentGamemodeIndex = gamemodes.Length - 1;
        }
        gamemodeText.GetComponent<TMP_Text>().SetText(gamemodes[currentGamemodeIndex]);
    }

    public void NextMap()
    {
        currentMapIndex++;
        if (currentMapIndex >= mapImages.Length)
        {
            currentMapIndex = 0;
        }
        mapShowcaseImage.GetComponent<RawImage>().texture = mapImages[currentMapIndex];
    }

    public void PrevMap()
    {
        currentMapIndex--;
        if (currentMapIndex < 0)
        {
            currentMapIndex = mapImages.Length - 1;
        }
        mapShowcaseImage.GetComponent<RawImage>().texture = mapImages[currentMapIndex];
    }

    public void StartGame()
    {

    }


}
