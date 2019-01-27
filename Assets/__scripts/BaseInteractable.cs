using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BaseInteractable : BaseMonoBehaviour
							  , IColliderListener
{
	/// <summary>
	/// collider on self, for enabling/disabling raycast etc
	/// </summary>
	public new Collider collider;
	/// <summary>
	/// collider on self or external, for listening
	/// </summary>
	public Collider[] colliders;
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

		if (colliders == null || colliders.Length == 0)
		{
			colliders = GetComponentsInChildren<Collider>();
		}
		ColliderProxy.register(this, colliders);

		if (!rigidbody)
		{
			rigidbody = GetComponent<Rigidbody>();
		}

		if (!director)
		{
			director = GetComponent<PlayableDirector>();
		}
	}

	void IColliderListener.onInteract( Collider _collider )
	{
		onInteract(_collider);
	}
	protected virtual void onInteract( Collider _collider )
	{
		Debug.Log($"onInteract {name} ({_collider.name})");
		onInteract();
	}
	protected virtual void onInteract() { }
}
