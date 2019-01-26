using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ScriptTableau : MonoBehaviour
{
	public Color highlightColor = Color.green;
	Color defaultColor;
	int nbclick = 0;
	public bool looping;
	public PlayableDirector playableDirector;
	public TimelineAsset[] listTimeline;
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

	void OnMouseDown()
	{
		if (conditionBefore && !conditionBefore.activeInHierarchy)
		{
			return;
		}
		if (nbclick >= listTimeline.Length)
		{
			if (looping==true)
			{
				nbclick = 0;
			}
			else
			{
				return;
			}
		}
		playableDirector.Play(listTimeline[nbclick]);
		nbclick++;
	}
}
