using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private float toPlayerDistance;

    private Animator anim; // AnimatorController

    [SerializeField] private float walkSpeed = 0.75f;

    [SerializeField] private float seeDistance = 15.0f;
    [SerializeField] private float runSpeed = 1.25f;

    [SerializeField] private float sprintDistance = 10.0f;
    [SerializeField] private float sprintSpeed = 2.5f;

    [SerializeField] private float attackDistance = 2.0f;

    [SerializeField] private float attackDamage = 10.0f;

    //[SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public HealthBar healthBar;

    private VisionConePlayerDetection playerDetection; // Reference to the VisionConePlayerDetection script

    [SerializeField] private Transform[] wayPoints;  // Assign waypoints in the Unity editor
    private NavMeshAgent agent;
    private int currentDestinationPoint; // index
    private bool chasingPlayer = false; // To track whether the zombie is chasing the player

    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float damageCooldown = 2.15f; // Time between receiving damage
    private float lastDamageTime = 0f; // Tracks the time of the last damage

    private AudioSource audio;

    void Start()
    {
        // For "this"
        anim = GetComponent <Animator>();

        // Look for the VisionConePlayerDetection component in the zombie's children
        playerDetection = GetComponentInChildren<VisionConePlayerDetection>();
        if (playerDetection == null)
        {
            Debug.LogError("No VisionCone component found in child objects!");
        }

        agent = GetComponent<NavMeshAgent>();
        GotoNextWaypoint();

        // To control AudioSource
        audio = GetComponent<AudioSource>();

        // Initialize health system
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    
    void Update()
    {

        if (currentHealth <= 0)
        {
            HandleDeath();
            return; // Stop further updates once the enemy is dead
        }

        if (playerDetection.playerDetected)
        {
            chasingPlayer = true;
            ChasePlayer();
        }
        else
        {
            if (chasingPlayer)
            {
                chasingPlayer = false;
                ResumePatrolling();
            }
            else
            {
                Patrolling();
            }
        }

    }

    private void HandleDeath()
    {
        // Stop the NavMeshAgent from moving
        agent.enabled = false;
        audio.enabled = false;

        anim.SetTrigger("isDying"); // Trigger either "Dying" or "WalkingToDying" animation
        //StartCoroutine(DeathParticlesWithDelay(2f));

        // Destroy enemy gameObject after 6 seconds
        Destroy(this.gameObject, 6);
    }

    /* I cut this feature out temporarily because it is not working for some reason
    private IEnumerator DeathParticlesWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for delay time before continuing coroutine

        if (deathParticles != null)
        {
            deathParticles.Play();
            //Debug.Log("Particles Playing: " + deathParticles.isPlaying);
        }
        yield return null;
    }
    */

    public void TakeDamage(float damage) // had to make public for access from ShooterSystem script
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    void GotoNextWaypoint()
    {
        if (wayPoints.Length == 0)
            return;

        // Choose a random waypoint
        currentDestinationPoint = Random.Range(0, wayPoints.Length);

        // Set the destination of the agent to the selected random waypoint
        agent.SetDestination(wayPoints[currentDestinationPoint].position);

        anim.SetBool("isWalking", true);
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

        toPlayerDistance = Vector3.Distance(this.transform.position, player.position);

        // if player is within enemy's reach for attack
        if (toPlayerDistance <= attackDistance)
        {
            anim.SetBool("isAttacking", true);
            // Check if enough time has passed since the last attack
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                StartAttack();
                lastDamageTime = Time.time; // Reset the last attack time
            }
        }
        else
        {
            anim.SetBool("isAttacking", false);
        }

        // if player is close to enemy, but not enough for him to attack
        if (toPlayerDistance <= sprintDistance)
        {
            if (!anim.GetBool("isAttacking"))
            {
                anim.SetBool("isSprinting", true);
                agent.speed = sprintSpeed;
            }
            else
            {
                anim.SetBool("isSprinting", false);
            }
        }
        else
        {
            anim.SetBool("isSprinting", false);
        }

        // if player is far but detected by enemy
        if (toPlayerDistance <= seeDistance)
        {
            if (!anim.GetBool("isSprinting") && !anim.GetBool("isAttacking"))
            {
                anim.SetBool("isRunning", true);
                agent.speed = runSpeed;
            }
            else
            {
                anim.SetBool("isRunning", false);
            }
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    private void ResumePatrolling()
    {
        // Reset the agent speed to walking speed and go to the next waypoint
        agent.speed = walkSpeed;

        // Reset any attack/sprint/run animations
        anim.SetBool("isAttacking", false);
        anim.SetBool("isSprinting", false);
        anim.SetBool("isRunning", false);

        // Return to waypoint patrol
        GotoNextWaypoint();
    }

    private void Patrolling()
    {
        // Check if the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            // Go to the next random waypoint
            GotoNextWaypoint();
        }
    }

    private void StartAttack()
    {
        Ray ray = new(attackOrigin.position, attackOrigin.forward);

        // Raycast to detect hit
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {

            // Check if the object hit is an enemy
            if (hitInfo.collider.CompareTag("Player"))
            {
                // Get PlayerHealthSystem script from the Player collider
                PlayerHealthSystem playerHealthSystem = hitInfo.collider.GetComponent<PlayerHealthSystem>();

                if (playerHealthSystem != null)
                {
                    playerHealthSystem.TakeDamage(attackDamage); // Reduce health by attackDamage
                }
            }
        }
    }
}
