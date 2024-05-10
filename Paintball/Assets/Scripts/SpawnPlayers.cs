using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviourPun
{
    public GameObject playerPrefab;
    public GameObject[] playerModels;
    private GameObject player;
    private GameObject customPlayer;
    public Mesh[] playerMeshes;

    private void Start()
    {
        GameObject customModel = GameObject.Find("CustomModel");

        Color c = new Color(0,0,0);
        Renderer playerRenderer = customModel.GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            // Get the materials of the renderer
            Material[] materials = playerRenderer.materials;

            // Find and set the color of the "suit_material" instance
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name == "suit_material (Instance)")
                {
                    c = materials[i].color;
                    break;
                }
            }
        }


        // Get the PhotonView ID of the local player
        int photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;

        object[] data = new object[5]; // Increase data array size to accommodate the PhotonView ID
        data[0] = photonViewID; // Store the PhotonView ID
        data[1] = ExtractPrefabIndex(customModel.GetComponent<MeshFilter>().mesh.name);
        data[2] = c.r;
        data[3] = c.g;
        data[4] = c.b;

        Debug.Log($"{data[0]} {data[1]} {data[2]} {data[3]} {data[4]}");


        // Instantiate the player prefab with custom initialization data
        player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 3, 0), Quaternion.identity, 0, data);
    }


    private int ExtractPrefabIndex(string meshName)
    {
        for (int i = 0; i < playerMeshes.Length; i++)
        {
            string mesh = playerMeshes[i].name + " Instance";
            if (mesh == meshName)
            {
                return i; // Return the index of the matching mesh
            }
        }
        // If no match is found, return -1 or any value indicating failure
        return -1;
    }



    private void UpdatePlayerModel(object[] data)
    {
        // Cast the data array into the expected types
        int prefabIndex = (int)data[0];
        float redValue = (float)data[1];
        float greenValue = (float)data[2];
        float blueValue = (float)data[3];
        GameObject newPlayer = Instantiate(playerModels[prefabIndex], player.transform.position, player.transform.rotation, player.transform.parent);

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
                    Color c = new Color(redValue / 255f, greenValue / 255f, blueValue / 255f);
                    materials[i].color = c;
                    materials[i].SetColor("_Color", c);

                    break; // Exit the loop after changing the color
                }
            }
        }
        newPlayer.layer = 11; //UIPlayer

        // Use the data to update the player model as needed
        // For example, you can find the player object by its PhotonView ID and update its properties
        //Replace(player, newPlayer);
    }


}
