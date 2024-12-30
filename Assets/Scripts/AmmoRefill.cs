using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRefill : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 20; // Amount to ammo to refillthe player
    [SerializeField] private float rotationSpeed = 50f; // Rotation speed for y-axis
    [SerializeField] private float respawnTime = 5f; // Time to reappear after collection

    private ShooterSystem shooterSystem;

    private Renderer ammoRenderer;
    private Collider ammoCollider;

    private void Start()
    {
        // Cache the renderer and collider for easier access
        ammoRenderer = GetComponent<Renderer>();
        ammoCollider = GetComponent<Collider>();
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
            // Find the MainCamera in the scene to find the ShooterSystem on the gun point
            GameObject mainCamera = GameObject.FindWithTag("MainCamera");
            if (mainCamera != null)
            {
                // Find the ShooterSystem on the GunPoint child object
                shooterSystem = mainCamera.GetComponentInChildren<ShooterSystem>();

                if (shooterSystem != null)
                {
                    shooterSystem.Reload(ammoAmount); // Ammo the player
                    StartCoroutine(RespawnAmmo());    // Start the respawn coroutine
                }
            }
        }
    }
        
    private IEnumerator RespawnAmmo()
    {
        // Disable the renderer and collider to "hide" the band-aid
        ammoRenderer.enabled = false;
        ammoCollider.enabled = false;

        // Wait for the respawn time
        yield return new WaitForSeconds(respawnTime);

        // Re-enable the renderer and collider to make the band-aid reappear
        ammoRenderer.enabled = true;
        ammoCollider.enabled = true;
    }
}
