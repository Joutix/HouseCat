using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public interface IColliderListener
{
	void onInteract( Collider _collider );
}

public class ColliderProxy : MonoBehaviour
{
	public List<IColliderListener> listeners = new List<IColliderListener>();

	new Collider collider;
	protected void OnEnable()
	{
		collider = GetComponent<Collider>();
	}

	protected void OnMouseUpAsButton()
	{
		foreach (var listener in listeners)
		{
			listener?.onInteract(collider);
		}
	}

	public static void register( IColliderListener _listener, IList<Collider> _colliders )
	{
		foreach (var collider in _colliders)
		{
			var proxy = collider.GetComponent<ColliderProxy>();
			if (!proxy)
			{
				proxy = collider.gameObject.AddComponent<ColliderProxy>();
			}

			if (!proxy.listeners.Contains(_listener))
			{
				proxy.listeners.Add(_listener);
			}
		}
	}
}
