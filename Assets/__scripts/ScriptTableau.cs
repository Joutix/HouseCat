using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ScriptTableau : BaseInteractable
{
	int nbclick = 0;
	public bool looping;
	public TimelineAsset[] listTimeline;
	public GameObject conditionBefore;

	protected override void onInteract( Collider _collider )
	{
		base.onInteract(_collider);

		if (conditionBefore && !conditionBefore.activeInHierarchy)
		{
			return;
		}

		if (nbclick >= listTimeline.Length)
		{
			if (looping)
			{
				nbclick = 0;
			}
			else
			{
				return;
			}
		}

		director.Play(listTimeline[nbclick]);
		nbclick++;
	}
}
