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
    [SerializeField] private Slider healthSlider;
    private Transform deadCamera;
    private Transform aliveCamera;
    private GameObject customModel;

    public GameObject mainCamera;

    private PhotonView view;
    private PhotonView player_view;



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
        player_view = transform.GetComponent<PhotonView>();

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

    public void UpdateRefs()
    {
        Debug.Log("Updated health ref");
        healthSlider = GameObject.Find("UI").GetComponentInChildren<Slider>();
        // Set the initial value of the slider
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
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
            if (view.IsMine)
            {
                player_view.RPC("UpdateDeath", RpcTarget.All, view.ViewID / 1000 - 1);
            }        
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


    public void Revive()
    {
        if (view.IsMine)
        {

            if (!isDead) return;

            isDead = false;

            // Reset health
            currentHealth = maxHealth;
            UpdateHealthUI();

            // Disable the dead camera and enable the alive camera
            deadCamera.gameObject.SetActive(false);
            aliveCamera.gameObject.SetActive(true);

            // Enable the main camera and its components
            mainCamera.GetComponent<ThirdPersonCam>().enabled = true;

            // Enable player movement and climbing scripts
            transform.GetComponent<PlayerMovementAdvanced2>().enabled = true;
            transform.GetComponent<Climbing>().enabled = true;

            // Reset the Rigidbody constraints and gravity
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Or whatever constraints you had initially
            rb.useGravity = false; // Or true if the player should have gravity enabled initially

            // If the player was crouched, ensure the scale is reset
            transform.localScale = Vector3.one;
            Debug.Log("Player revived");
        }

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


    [PunRPC]
    public void UpdateDeath(int sender_id)
    {
        GameManagerScript gms = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        gms.player_dead_stats[sender_id] = true;
        if (view.IsMine)
        {
            int winner = -1;
            for (int i = 0; i < gms.player_dead_stats.Length; i++)
            {
                if (gms.player_dead_stats[i] == false && winner == -1)
                {
                    winner = i;
                }
                else if (gms.player_dead_stats[i] == false && winner != -1)
                {
                    winner = -2;
                    break;
                }
            }
            if (winner >= 0)
            {
                Transform pl_holder = GameObject.Find("PlayerHolder").transform;
                foreach(Transform pl in pl_holder.transform)
                {
                    PhotonView pv = pl.transform.GetChild(1).GetComponent<PhotonView>();
                    pv.RPC("UpdateScore", RpcTarget.All, winner);
                }
                
            }
        }
    }

    [PunRPC]
    public void UpdateScore(int winner)
    {
        if (view.IsMine)
        {
            Debug.Log($"Winner is {winner} and im {view.ViewID}");
            GameManagerScript gms = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            gms.game_scores[winner] += 1;
            gms.first_game = false;

            Debug.Log(gameObject.transform.GetChild(1));
            Debug.Log(gameObject.transform.GetChild(0).GetChild(0));

            PickUpController pc = gameObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<PickUpController>();

            if (pc && pc.equipped)
            {
                //pc.Drop();
                PhotonView pv = pc.transform.GetComponent<PhotonView>();
                pv.RPC("Drop", RpcTarget.All, pv.ViewID, view.ViewID / 1000);
                pc.transform.localScale = Vector3.one;
                Debug.Log("I dropped the gun");
            }

            gms.UpdateAllScores();

            Transform player_holder = GameObject.Find("PlayerHolder").transform;
            foreach (Transform child in player_holder.transform)
                if (child.transform.GetChild(1).GetComponent<PlayerStatsHandler>().IsDead() == true)
                {
                    child.transform.GetChild(1).GetComponent<PlayerStatsHandler>().Revive();
                }

            if (view.ViewID == 1001)
                gms.ResetGame();
        }
        

    }

}
