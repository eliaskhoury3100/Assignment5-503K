using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionConePlayerDetection : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange = 15f;
    public float VisionAngle = 120f;
    public LayerMask VisionObstructingLayer; // Layer with objects that obstruct the enemy view, like walls, for example
    public LayerMask PlayerLayer; // Layer mask for the player
    public int VisionConeResolution = 120; // Vision cone will be made up of triangles, higher value is prettier

    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    //public bool playerDetected; // Flag to indicate if player is detected
    public bool playerDetected { get; private set; } // Make it accessible via a property


    // Start is called before the first frame update
    void Start()
    {
        // To show the cone vision mesh, enable the next line and disable the 2 lines after !!!
        //transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshRenderer meshRenderer = transform.AddComponent<MeshRenderer>();
        meshRenderer.enabled = false; // Disable the renderer to make it invisible

        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle *= Mathf.Deg2Rad; // Convert angle to radians
    }

    // Update is called once per frame
    void Update()
    {
        playerDetected = DetectedPlayerVisionCone(); // Update the vision cone each frame (need to run it here to work and import it in other script)
        //Debug.Log(DetectedPlayerVisionCone());
    }

    bool DetectedPlayerVisionCone()
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] vertices = new Vector3[VisionConeResolution + 1];
        vertices[0] = Vector3.zero;

        float currentAngle = -VisionAngle / 2;
        float angleIncrement = VisionAngle / (VisionConeResolution - 1);
        float sine;
        float cosine;
        bool playerDetected = false; // Initialize detection flag

        for (int i = 0; i < VisionConeResolution; i++)
        {
            sine = Mathf.Sin(currentAngle);
            cosine = Mathf.Cos(currentAngle);
            Vector3 raycastDirection = (transform.forward * cosine) + (transform.right * sine);
            Vector3 vertForward = (Vector3.forward * cosine) + (Vector3.right * sine);

            if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, VisionRange/*, VisionObstructingLayer | PlayerLayer*/))
            {
                if (hit.collider.CompareTag("Player")) // Check if the hit object is the player
                {
                    playerDetected = true; // Mark player as detected
                }

                vertices[i + 1] = vertForward * hit.distance;
            }
            else
            {
                vertices[i + 1] = vertForward * VisionRange;
            }

            currentAngle += angleIncrement;
        }

        // Create triangles for the vision cone
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        VisionConeMesh.Clear();
        VisionConeMesh.vertices = vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;

        // Return whether the player is detected
        return playerDetected;
    }
}
