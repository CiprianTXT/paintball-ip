using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsHandler : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isDead = false;
    private Slider healthSlider;
    private Transform deadCamera;
    private Transform aliveCamera;
    private GameObject customModel;

    private void Awake()
    {
        aliveCamera = GameObject.Find("CombatCam").transform;
        deadCamera = GameObject.Find("ThirdPersonCam").transform;
        deadCamera.gameObject.SetActive(false);


        customModel = GameObject.Find("CustomModel");
        GameObject model = GameObject.Find("PlayerModel");
        Replace(model, customModel);

        GameObject.Find("PlayerCam").GetComponent<ThirdPersonCam>().playerObj = customModel.transform;
    }

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

        
        currentHealth = maxHealth;

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

    // Function to reduce player health and update the slider
    public void TakeDamage(int damage)
    {
        if (isDead) return;

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
            pc.Drop();
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

        GameObject.Find("PlayerCam").GetComponent<ThirdPersonCam>().enabled = false;

        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

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
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    // Optional function to check if player is dead
    public bool IsDead()
    {
        return isDead;
    }
}
