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
			var go = renderer.gameObject;

			if (!renderer.GetComponent<MeshCollider>())
			{
				Undo.AddComponent<MeshCollider>(go);
			}
			if (!renderer.GetComponent<ColliderProxy>())
			{
				Undo.AddComponent<ColliderProxy>(go);
			}


			var tex = renderer.sharedMaterial.mainTexture;
			renderer.sharedMaterial = tex
				                          ? new Material(defaultMaterial) { mainTexture = tex }
				                          : defaultMaterial;


			var staticFlags = GameObjectUtility.GetStaticEditorFlags(go);
			if (go.GetComponent<Pickable>() ||
			    go.GetComponent<ObjetInteraction>() ||
			    go.GetComponent<Animator>())
			{
				staticFlags &= ~StaticEditorFlags.LightmapStatic;
			}
			else
			{
				staticFlags |= StaticEditorFlags.LightmapStatic;
			}
			GameObjectUtility.SetStaticEditorFlags(go, staticFlags);
		}
	}
#endif
}
