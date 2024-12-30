using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBobbing : MonoBehaviour
{
    [SerializeField] private float bobbingSpeed = 0.05f; // Speed of the bobbing
    [SerializeField] private float bobbingAmount = 0.05f; // Amount the gun moves up and down

    private float defaultPosY;
    private float timer = 0.0f;

    void Start()
    {
        defaultPosY = transform.localPosition.y; // Initial Y position
    }

    void Update()
    {
        // Get input axis values
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f; // No movement, reset timer
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed;

            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2); // Reset timer once full cycle completes
            }
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // Ensure movement direction doesn't affect speed
            translateChange = totalAxes * translateChange;
            Vector3 localPosition = transform.localPosition;
            localPosition.y = defaultPosY + translateChange;
            transform.localPosition = localPosition;
        }
        else
        {
            // Return gun to default position when idle
            Vector3 localPosition = transform.localPosition;
            localPosition.y = defaultPosY;
            transform.localPosition = localPosition;
        }
    }
}