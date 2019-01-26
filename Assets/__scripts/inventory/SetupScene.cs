using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetupScene : BaseMonoBehaviour
{
	public Material defaultMaterial;

#if UNITY_EDITOR
	[ContextMenu("Setup Scene")]
	protected void setupScene()
	{
		foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
		{
			if (!renderer.GetComponent<MeshCollider>())
			{
				Undo.AddComponent<MeshCollider>(renderer.gameObject);
			}

			var tex = renderer.sharedMaterial.mainTexture;
			renderer.sharedMaterial = tex
				                          ? new Material(defaultMaterial) { mainTexture = tex }
				                          : defaultMaterial;

			var staticFlags = GameObjectUtility.GetStaticEditorFlags(renderer.gameObject);
			if (renderer.gameObject.GetComponent<Pickable>())
			{
				staticFlags &= ~StaticEditorFlags.LightmapStatic;
			}
			else
			{
				staticFlags |= StaticEditorFlags.LightmapStatic;
			}
			GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, staticFlags);
		}
	}
#endif
}
