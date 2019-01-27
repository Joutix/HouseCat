using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ObjetInteraction : BaseInteractable
{
	public bool unefois; // L'animation doit être éxécuté une fois si true
	bool active;
	public GameObject conditionBefore;
	public GameObject conditionAfter;
	public Pickable unlockWithPickable;

	/*public Color highlightColor = Color.green;
	Color defaultColor;
	Material material;

	protected override void Awake()
	{
		base.Awake();

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
	}*/

	public GameObject[] objectsToToggle;
	protected override void onInteract( Collider _collider )
	{
		base.onInteract(_collider);

		if (conditionBefore && !conditionBefore.activeInHierarchy)
		{
			return;
		}

		if (unlockWithPickable && Pickable.current != unlockWithPickable)
		{
			return;
		}

		if (unefois && active) //Si on veut executer l'action qu'une fois
		{
			return; // déjà executé
		}

		active = true;
		if (director)
		{
			director.Play();
		}
		if (conditionAfter)
		{
			conditionAfter.SetActive(true);
		}

		foreach (var objectToToggle in objectsToToggle)
		{
			if (objectToToggle)
			{
				objectToToggle.SetActive(!objectToToggle.activeSelf);
			}
		}
	}
}
