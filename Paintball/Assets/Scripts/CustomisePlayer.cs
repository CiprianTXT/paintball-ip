using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CustomisePlayer : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public Transform playerHolder;
    public Mesh[] playerMeshes;

    public void Awake()
    {
        playerHolder = GameObject.Find("PlayerHolder").transform;
    }



    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        transform.parent = playerHolder;
        Debug.Log("i see you now AAAAAAAAAAAAAAAAAAAAAAAAAA");
        object[] data = info.photonView.InstantiationData;
        Debug.Log($"{data[0]} {data[1]} {data[2]} {data[3]} {data[4]}");
        // Use instantiationData to update the player model
        //UpdatePlayerModel(instantiationData);

        Transform model = PhotonView.Find((int)data[0] * 1000 + 2).gameObject.transform.Find("PlayerModel");

        MeshFilter filter = model.GetComponentInChildren<MeshFilter>();
        filter.mesh = playerMeshes[(int)data[1]];


        Renderer playerRenderer = model.GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {

            model.GetComponentInChildren<MeshCollider>().sharedMesh = playerMeshes[(int)data[1]];

            // Get the materials of the renderer
            Material[] materials = playerRenderer.materials;

            // Find and set the color of the "suit_material" instance
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name == "suit_material (Instance)")
                {
                    Color c = new Color((float)data[2], (float)data[3], (float)data[4]);
                    materials[i].color = c;
                    materials[i].SetColor("_Color", c);

                    // If data[1] is equal to 1, add another material to the array
                    if ((int)data[1] == 1)
                    {
                        // Resize the materials array
                        Material[] resizedMaterials = new Material[materials.Length + 1];

                        // Copy existing materials to the resized array
                        for (int j = 0; j < materials.Length; j++)
                        {
                            resizedMaterials[j] = materials[j];
                        }

                        // Assign the new material
                        resizedMaterials[materials.Length] = materials[0];

                        Material aux = resizedMaterials[1];
                        resizedMaterials[1] = resizedMaterials[2];
                        resizedMaterials[2] = aux;

                        // Update the materials array with the resized one
                        playerRenderer.materials = resizedMaterials;

                    } else if ((int)data[1] == 3)
                    {
                        Material aux = materials[0];
                        materials[0] = materials[1];
                        materials[1] = aux;

                        playerRenderer.materials = materials;
                    }

                    break; // Exit the loop after changing the color
                }
            }
        }
    }


}
