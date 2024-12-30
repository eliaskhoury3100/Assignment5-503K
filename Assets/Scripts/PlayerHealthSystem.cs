using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{

	public float maxHealth = 100;
	public float currentHealth; 

	public HealthBar healthBar;

	// Start is called before the first frame update
	void Start()
	{
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
	}

	/*
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			TakeDamage(10);
		}
	}
	*/

	public void TakeDamage(float damage)
	{
		currentHealth -= damage;

		healthBar.SetHealth(currentHealth);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	public void Heal(float regen)
    {
		currentHealth += regen;
		if (currentHealth > maxHealth)
        {
			currentHealth = 100;
        }
		healthBar.SetHealth(currentHealth);
	}

	// Handle player death
	private void Die()
	{
		Debug.Log("Player has died!");
		// Check if this GameObject has a parent
		if (this.transform.parent != null)
		{
			// Destroy the parent GameObject
			Destroy(this.transform.parent.gameObject);
		}
		else
		{
			// If there is no parent, just destroy this GameObject
			Destroy(this.gameObject);
		}
	}
}