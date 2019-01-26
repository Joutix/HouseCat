using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class light_flicker : MonoBehaviour
{
	public float speed = 1;
	public float intensityMin = 0.7f;
	public float shadowStrengthMin = 0.8f;

	new Light light;

	float baseIntensity;
	float minIntensity;

	float baseShadowStrength;
	float minShadowStrength;

	void Awake()
	{
		light = this.GetComponent<Light>();

		baseIntensity = light.intensity;
		minIntensity = baseIntensity * intensityMin;

		baseShadowStrength = light.shadowStrength;
		minShadowStrength = baseShadowStrength * shadowStrengthMin;
	}

	void Update()
	{
		float f = Time.time * speed;
		float intensity = Mathf.PerlinNoise(f, f);
		light.intensity = Mathf.Lerp(minIntensity, baseIntensity, intensity);
		light.shadowStrength = Mathf.Lerp(minShadowStrength, baseShadowStrength, intensity);
	}
}
