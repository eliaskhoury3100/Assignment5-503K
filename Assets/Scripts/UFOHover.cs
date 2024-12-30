using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOHover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;      // Speed of movement
    [SerializeField] private float bounceHeight = 1.5f;   // How high the object "bounces"
    [SerializeField] private float bounceSpeed = 1.0f;    // Speed of the bounce effect

    private Vector3 initialPosition;
    private Rigidbody rb;

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        // Disable gravity to simulate the moon environment
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    void FixedUpdate()
    {
        // Horizontal movement
        float horizontal = Mathf.Sin(Time.time * moveSpeed);
        float vertical = Mathf.Cos(Time.time * moveSpeed);

        // Simulate a bouncing motion on the y-axis
        float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        // Update position with smooth, floating-like movement
        transform.position = new Vector3(initialPosition.x + horizontal, initialPosition.y + bounce, initialPosition.z + vertical);
    }
}