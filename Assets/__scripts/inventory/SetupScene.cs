using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetupScene : BaseMonoBehaviour
{
	public Material defaultMaterial;
	public List<Shader> knownShaders;

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


			var mat = renderer.sharedMaterial;
			var prefab = PrefabUtility.GetCorrespondingObjectFromSource(renderer);
			var prefabTex = prefab.sharedMaterial.mainTexture;
			if (!knownShaders.Contains(mat.shader) ||
			    (prefabTex && mat == defaultMaterial))
			{
				mat = new Material(defaultMaterial);
			}
			if (mat.mainTexture != prefabTex)
			{
				mat.mainTexture = prefabTex;
			}
			renderer.sharedMaterial = mat;


			var staticFlags = GameObjectUtility.GetStaticEditorFlags(go);
			if (go.GetComponent<BaseInteractable>() ||
			    //go.GetComponent<ObjetInteraction>() ||
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
