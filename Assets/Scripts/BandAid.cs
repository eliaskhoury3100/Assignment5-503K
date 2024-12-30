using UnityEngine;
using System.Collections;

public class BandAid : MonoBehaviour
{
    [SerializeField] private float healAmount = 20f; // Amount to heal the player
    [SerializeField] private float rotationSpeed = 50f; // Rotation speed for y-axis
    [SerializeField] private float respawnTime = 5f; // Time to reappear after collection

    private PlayerHealthSystem playerHealth;

    private Renderer bandAidRenderer;
    private Collider bandAidCollider;

    private void Start()
    {
        // Cache the renderer and collider for easier access
        bandAidRenderer = GetComponentInChildren<Renderer>(); // it's in the child's
        bandAidCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        // Rotate the band-aid around its y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponentInChildren<PlayerHealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount); // Heal the player
                StartCoroutine(RespawnBandAid()); // Start the respawn coroutine
            }
        }
    }

    private IEnumerator RespawnBandAid()
    {
        // Disable the renderer and collider to "hide" the band-aid
        bandAidRenderer.enabled = false;
        bandAidCollider.enabled = false;

        // Wait for the respawn time
        yield return new WaitForSeconds(respawnTime);

        // Re-enable the renderer and collider to make the band-aid reappear
        bandAidRenderer.enabled = true;
        bandAidCollider.enabled = true;
    }
}