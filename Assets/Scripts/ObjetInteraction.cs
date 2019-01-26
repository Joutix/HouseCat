using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ObjetInteraction : MonoBehaviour
{
	public Color highlightColor = Color.green;
	Color defaultColor;
	public bool unefois; // L'animation doit être éxécuté une fois si true
	bool active=false;
	public GameObject conditionBefore;
	public GameObject conditionAfter;
	public PlayableDirector animationObjet;

	Material material;
	void Awake()
	{
		animationObjet = GetComponent<PlayableDirector>();
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
		if (unefois) { //Si on veut executé l'action qu'une fois
			if (!active) // Si on l'a pas déjà éxécuté
			{
				animationObjet.Play();
				active = true;
				conditionAfter.SetActive(true);
			}
			else { return;  }
		}
		else { // Si c'est une action qui peut se répéter en boucle
		animationObjet.Play();

		conditionAfter.SetActive(true);
		}
	}
}

