using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public GameObject playerPrefab;
    public GameObject[] playerModels;
    private GameObject player;
    private GameObject customPlayer;

    private void Start()
    {
        
        // Instantiate the player prefab with custom initialization data
        player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 3, 0), Quaternion.identity);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("i see you");
        object[] instantiationData = info.photonView.InstantiationData;
        // Use instantiationData to update the player model
        UpdatePlayerModel(instantiationData);
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
        Replace(player, newPlayer);
    }

    void Replace(GameObject source, GameObject dst)
    {
        Debug.Log("Started replace");
        // The Transform defines the position of a game object in the scene hierarchy. 
        Transform tf_src = source.transform;
        Transform tf_dst = dst.transform;

        // Make dst a child of the same parent. 
        tf_dst.parent = tf_src.parent;

        // If necessary, copy over things like local position, rotation and/or scale here. 
        dst.transform.localPosition = source.transform.localPosition;
        dst.transform.localRotation = source.transform.localRotation;


        // Move child transforms from src to dst. 
        int count = tf_src.childCount;
        for (int i = 0; i < count; i++)
        {
            // Note that we are MOVING child transforms from src to dst. 
            // Hence the zero here instead of i. 
            tf_src.GetChild(0).parent = tf_dst;
        }

        dst.name = "PlayerModel";
        dst.tag = "Player";
        dst.layer = 10; // Player

        source.transform.parent = null;
        Destroy(source);

    }

}
