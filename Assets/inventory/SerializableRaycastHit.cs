using System;
using UnityEngine;

[Serializable]
public struct SerializableRaycastHit
{
	public Collider collider;
	public int triangleIndex;
	public Vector3 barycentricCoordinate;
	public Vector2 textureCoord;
	public Vector3 point;
	public Vector3 normal;

	public SerializableRaycastHit( RaycastHit hit )
	{
		collider = hit.collider;
		triangleIndex = hit.triangleIndex;
		barycentricCoordinate = hit.barycentricCoordinate;
		textureCoord = hit.textureCoord;
		point = hit.point;
		normal = hit.normal;
	}
}
