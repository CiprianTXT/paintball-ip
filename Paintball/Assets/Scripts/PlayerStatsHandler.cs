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



    // Start is called before the first frame update
    void Start()
    {
        aliveCamera = transform.parent.Find("CombatCam").transform;
        aliveCamera.gameObject.SetActive(false);

        Transform camHolder = transform.parent.Find("CameraHolder");

        mainCamera = camHolder.Find("PlayerCam").gameObject;
        mainCamera.SetActive(false);

        camHolder.gameObject.SetActive(false);
        

        deadCamera = transform.parent.Find("ThirdPersonCam").transform;
        deadCamera.gameObject.SetActive(false);

        view = transform.parent.GetComponent<PhotonView>();
        
        currentHealth = maxHealth;

        
        GameObject model = transform.Find("PlayerModel").gameObject;
      
        Rigidbody rb = model.transform.parent.GetComponent<Rigidbody>();
        

        if (view.IsMine)
        {
            

            mainCamera.gameObject.SetActive(true);
            camHolder.gameObject.SetActive(true);
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

        } else
        {
            rb.isKinematic = true;
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
            PhotonView pv = pc.transform.GetComponent<PhotonView>();
            pv.RPC("Drop", RpcTarget.All, pv.ViewID, view.ViewID / 1000);
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
