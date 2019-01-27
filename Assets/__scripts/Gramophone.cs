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

	public float variableAdditionnel = 0.10f;
	public float variableSoustrait = 0.10f;

	public GameObject conditionBefore;
	Material material;


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
		if (source.pitch >= 1)
		{
			source.pitch -= (variableSoustrait * Time.deltaTime); 
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
		}
		else
		{
			source.pitch += (variableAdditionnel);
		}

			}
}
