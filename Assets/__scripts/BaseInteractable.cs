using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BaseInteractable : BaseMonoBehaviour
{
	public new Collider collider;
	public new Rigidbody rigidbody;
	public PlayableDirector director;

	protected override void Awake()
	{
		base.Awake();

		initComponents();
	}
	protected override void OnValidate()
	{
		base.OnValidate();

		initComponents();
	}
	void initComponents()
	{
		if (!collider)
		{
			collider = GetComponent<Collider>();
		}

		if (!rigidbody)
		{
			rigidbody = GetComponent<Rigidbody>();
		}

		if (!director)
		{
			director = GetComponent<PlayableDirector>();
		}
	}

	//protected void OnMouseUp()
	//{
	//	onInteract();
	//}
	protected void OnMouseUpAsButton()
	{
		onInteract();
	}
	protected virtual void onInteract()
	{
		Debug.Log($"onInteract {name}");
	}
}
