using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetupScene : BaseMonoBehaviour
{
#if UNITY_EDITOR
	[ContextMenu("Create Colliders")]
	protected void createColliders()
	{
		foreach (var renderer in GetComponentsInChildren(typeof(MeshRenderer)))
		{
			if (!renderer.GetComponent<MeshCollider>())
			{
				Undo.AddComponent<MeshCollider>(renderer.gameObject);
			}
		}
	}
#endif
}
