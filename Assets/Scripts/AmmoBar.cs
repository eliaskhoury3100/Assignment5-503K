using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{

	public Slider slider;
	public Gradient gradient;
	public Image fill;

	public void SetMaxAmmo(float ammo)
	{
		slider.maxValue = ammo;
		slider.value = ammo;

		fill.color = gradient.Evaluate(1f);
	}

	public void SetAmmo(float ammo)
	{
		slider.value = ammo;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

}