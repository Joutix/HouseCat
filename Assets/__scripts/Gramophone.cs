using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gramophone : BaseInteractable
{
	public AudioClip[] soundtrack;
	AudioClip sound;
	int tempsMusic;
	public AudioSource source;

	int i = -1;
	int musicPrecedent=-1;

	public float variableAdditionnel = 0.10f;
	public float variableSoustrait = 0.10f;

	public GameObject conditionBefore;

	protected override void Update()
	{
		base.Update();

		if (source.pitch >= 1)
		{
			source.pitch -= (variableSoustrait * Time.deltaTime); 
		}
	}

	protected override void onInteract( Collider _collider )
	{
		base.onInteract(_collider);

		if (conditionBefore && !conditionBefore.activeInHierarchy)
		{
			return;
		}

		if (i == -1 || !source.isPlaying)
		{
			i = Random.Range(0, soundtrack.Length);
			while (i == musicPrecedent)
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
