using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStatsHandler : MonoBehaviourPun
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isDead = false;
    private Slider healthSlider;
    private Transform deadCamera;
    private Transform aliveCamera;
    private GameObject customModel;

    public GameObject mainCamera;

    private PhotonView view;


    // Replaces source by dst. 
    void Replace(GameObject source, GameObject dst)
    {
        

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



    // Start is called before the first frame update
    void Start()
    {
        aliveCamera = transform.parent.Find("CombatCam").transform;
        aliveCamera.gameObject.SetActive(false);

        mainCamera = transform.parent.Find("CameraHolder").Find("PlayerCam").gameObject;
        mainCamera.SetActive(false);

        deadCamera = transform.parent.Find("ThirdPersonCam").transform;
        deadCamera.gameObject.SetActive(false);

        view = transform.parent.GetComponent<PhotonView>();
        
        currentHealth = maxHealth;

        
        GameObject model = transform.Find("PlayerModel").gameObject;
        //Replace(model, customModel);

        

        if (view.IsMine)
        {
            mainCamera.SetActive(true);
            //mainCamera.GetComponent<ThirdPersonCam>().playerObj = customModel.transform;

            aliveCamera.gameObject.SetActive(true);

            

            // Find the health slider in the UI
            healthSlider = GameObject.Find("UI").GetComponentInChildren<Slider>();
            if (healthSlider == null)
            {
                Debug.LogError("error");
            }
            else
            {
                // Set the initial value of the slider
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }

        }

        
    }

    public Transform playerObj;
    public Transform orientation;

    void Update()
    {
        
        playerObj.rotation = orientation.rotation;
    }




    // Function to reduce player health and update the slider
    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            Debug.Log("im literally dead");
            return;
        }
        

        currentHealth -= damage;

        // Update the slider value
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Function to handle player death
    private void Die()
    {
        isDead = true;

        PickUpController pc = gameObject.GetComponentInChildren<PickUpController>();
        if (pc && pc.equipped)
        {
            //pc.Drop();
            view.RPC("Drop", RpcTarget.All, view.ViewID / 1000 );
            pc.transform.localScale = Vector3.one;
        }
            

        //Fix if crouched
        transform.localScale = Vector3.one;

        // Activate ragdoll effect or any other death effect
        ActivateRagdoll();

    }

    // Function to activate ragdoll effect
    private void ActivateRagdoll()
    {
        transform.GetComponent<PlayerMovementAdvanced2>().enabled = false;
        //transform.GetComponent<PickUpController>().Drop();
        transform.GetComponent<Climbing>().enabled = false;

        deadCamera.gameObject.SetActive(true);
        aliveCamera.gameObject.SetActive(false);

        mainCamera.GetComponent<ThirdPersonCam>().enabled = false;

        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        transform.GetComponent<Rigidbody>().useGravity = true;
    }

    // Function to restore player health
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Update the slider value
        UpdateHealthUI();
    }

    // Function to set player health directly
    public void SetHealth(int health)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(health, 0, maxHealth);

        // Update the slider value
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Function to update the health slider
    private void UpdateHealthUI()
    {
        if (view.IsMine)
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }
        }
        
    }

    // Optional function to check if player is dead
    public bool IsDead()
    {
        return isDead;
    }
}
