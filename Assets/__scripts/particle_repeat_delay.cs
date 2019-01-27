using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_repeat_delay : MonoBehaviour
{
	new public ParticleSystem particleSystem;
	public float delay = 2f;
	float t = 0f;

	void Update()
	{
		t -= Time.deltaTime;

		if(t <= 0)
		{
			t += delay;
			particleSystem.Play();
		}
	}
}
