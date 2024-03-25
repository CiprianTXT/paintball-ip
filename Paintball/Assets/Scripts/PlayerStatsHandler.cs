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

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        // Find the health slider in the UI
        healthSlider = GameObject.Find("UI").GetComponentInChildren<Slider>();
        if (healthSlider == null)
        {
            Debug.LogError("Health slider not found! Make sure it is a child of the Player GameObject.");
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
            pc.Drop();

        

        // Activate ragdoll effect or any other death effect
        //ActivateRagdoll();

        // You can add other death-related logic here, like respawn
    }

    // Function to activate ragdoll effect
    private void ActivateRagdoll()
    {
        // Assuming you have a Rigidbody attached to the player
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        // Disable colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Optional: Add some force to push ragdoll parts away from the center
        Vector3 forceDirection = transform.position - Camera.main.transform.position;
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddForce(forceDirection * 500f);
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
}
