using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class UpdateGameSettings : MonoBehaviour
{

    public Material[] maps;
    public int mapIdx = 0;  // Use camelCase for variable names
    public MeshRenderer mr;

    public string[] gamemodes;
    public int gamemodeIdx = 0;
    public TMP_Text gamemodeText;

    // Start is called before the first frame update
    void Start()
    {
        if (maps.Length > 0)
        {
            mr.material = maps[mapIdx];
        }
        else
        {
            Debug.LogWarning("No materials assigned to the maps array.");
        }
    }

    [PunRPC]
    public void NextMap()
    {
        if (maps.Length == 0)
            return;

        mapIdx++;
        if (mapIdx >= maps.Length)
            mapIdx = 0;
        mr.material = maps[mapIdx];
    }

    [PunRPC]
    public void PrevMap()
    {
        if (maps.Length == 0)
            return;

        mapIdx--;
        if (mapIdx < 0)
            mapIdx = maps.Length - 1;
        mr.material = maps[mapIdx];
    }

    [PunRPC]
    public void NextGamemode()
    {
        if (gamemodes.Length == 0)
            return;

        gamemodeIdx++;
        if (gamemodeIdx >= gamemodes.Length)
            gamemodeIdx = 0;
        gamemodeText.SetText(gamemodes[gamemodeIdx]);
    }

    [PunRPC]
    public void PrevGamemode()
    {
        if (gamemodes.Length == 0)
            return;

        gamemodeIdx--;
        if (gamemodeIdx < 0)
            gamemodeIdx = gamemodes.Length - 1;
        gamemodeText.SetText(gamemodes[gamemodeIdx]);
    }

}
