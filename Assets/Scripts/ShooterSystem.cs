using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterSystem : MonoBehaviour
{
    [SerializeField] private Transform firingPoint;   // The position where the laser originates
    [SerializeField] private float laserDuration = 0.05f; // Duration the laser is visible
    [SerializeField] private float bulletDamage = 20f;
    private AudioSource audioSource; // Reference to the AudioSource component
    private LineRenderer lineRenderer;

    [SerializeField] private int maxAmmo = 100;
    private int currentAmmo;

    public AmmoBar ammoBar;

    void Start()
    {
        // Get the AudioSource component attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on this GameObject.");
        }

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No LineRenderer found on this GameObject.");
        }
        else
        {
            lineRenderer.enabled = false; // Initially disable the line renderer
        }

        currentAmmo = maxAmmo;
        ammoBar.SetMaxAmmo(maxAmmo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // if clicked left mouse button
        {
            // Play the shooting sound once
            if (audioSource != null)
            {
                audioSource.Play();
            }

            if (currentAmmo > 0)
            {
                currentAmmo--;
                ammoBar.SetAmmo(currentAmmo);

                // Create a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    // Enable the line renderer and set positions
                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, firingPoint.position);
                    lineRenderer.SetPosition(1, hitInfo.point);

                    StartCoroutine(DisableLaser());

                    // Check if the object hit is an enemy
                    if (hitInfo.collider.CompareTag("HeadShot"))
                    {
                        EnemyController enemyController = hitInfo.collider.GetComponentInParent<EnemyController>();
                        if (enemyController != null)
                        {
                            enemyController.TakeDamage(100); // HeadShot Kill
                        }
                    }
                    else if (hitInfo.collider.CompareTag("Enemy"))
                    {
                        EnemyController enemyController = hitInfo.collider.GetComponent<EnemyController>();
                        if (enemyController != null)
                        {
                            enemyController.TakeDamage(bulletDamage);
                        }
                    }
                    else if (hitInfo.collider.CompareTag("Truck"))
                    {
                        AudioSource truckAlarm = hitInfo.collider.GetComponent<AudioSource>();
                        if (truckAlarm != null)
                        {
                            truckAlarm.enabled = true;
                        }

                        ChangingLights lightsAlarm = hitInfo.collider.GetComponent<ChangingLights>();
                        if (lightsAlarm != null)
                        {
                            lightsAlarm.enabled = true;
                        }
                    }
                }
            }
        }
    }

    IEnumerator DisableLaser()
    {
        yield return new WaitForSeconds(laserDuration);
        lineRenderer.enabled = false;
    }

    public void Reload(int ammo)
    {
        currentAmmo += ammo;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = 100;
        }
        ammoBar.SetAmmo(currentAmmo);
    }
}