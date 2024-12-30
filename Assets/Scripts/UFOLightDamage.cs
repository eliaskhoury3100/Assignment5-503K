using UnityEngine;

public class UFOLightDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 5.0f; // Damage to deal every 5 seconds
    private PlayerHealthSystem playerHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assuming the player has the "Player" tag
        {
            Debug.Log("collided with player!");
            playerHealth = other.GetComponentInChildren<PlayerHealthSystem>(); // Look for the health system in the player's children
            if (playerHealth != null)
            {
                InvokeRepeating("DealDamage", 0f, 5f); // Deal damage every 5 seconds
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke("DealDamage"); // Stop dealing damage when player leaves
        }
    }

    private void DealDamage()
    {
        playerHealth.TakeDamage(damageAmount); // Apply damage
    }
}