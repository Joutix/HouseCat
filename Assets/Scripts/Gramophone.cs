using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gramophone : MonoBehaviour
{
	public Color highlightColor = Color.green;
	Color defaultColor;
	public AudioClip[] soundtrack;
	AudioClip sound;
	int tempsMusic;
	public AudioSource source;

	int i = -1;
	int musicPrecedent=-1;

	float variableAdditionnel = 0.10f;
	float variableSoustrait = 0.10f;

	public GameObject conditionBefore;
	Material material;

	float venti = 0f;
	float varAdditionnel = 0.10f;
	float varSoustrait = 0.10f;

	void Awake()
	{

		material = GetComponent<Renderer>().material;
		defaultColor = material.color;
	}

	void OnMouseEnter()
	{
		material.color = highlightColor;
	}

	void OnMouseExit()
	{
		material.color = defaultColor;
	}


	void Update()
	{
		if (source.isPlaying)
		{
			venti = 0.5f;
			transform.rotation *= Quaternion.AngleAxis(venti, Vector3.up);
		}
		if (source.pitch >= 1)
		{
			source.pitch -= (variableSoustrait * Time.deltaTime); 
		}
		if(transform.rotation.y > 1)
		{
			venti -= 5f;
			transform.rotation *= Quaternion.AngleAxis(venti, Vector3.up);
		}
	}

	void OnMouseDown()
	{

		if (conditionBefore && !conditionBefore.activeInHierarchy)
		{
			return;
		}
		if (i == -1 || !source.isPlaying)
		{
			i = Random.Range(0, soundtrack.Length);
			while(i == musicPrecedent)
			{
				i = Random.Range(0, soundtrack.Length);
			}
			source.pitch = 1;
			sound = soundtrack[i];
			source.clip = sound;
			source.Play();
			venti = 0.10f;
			transform.rotation *= Quaternion.AngleAxis(venti, Vector3.up);
		}
		else
		{
			source.pitch += (variableAdditionnel);
			if(transform.rotation.y < 0.50) { 
			venti += variableAdditionnel;
			transform.rotation *= Quaternion.AngleAxis(venti, Vector3.up);
			}
		}

			}
}
