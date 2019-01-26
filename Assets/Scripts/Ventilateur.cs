using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilateur : MonoBehaviour
{
	float venti = 0f;
	float variableAdditionnel = 15f;
	float variableSoustrait = 4f;

	// Update is called once per frame
	void Update()
	{
		if (venti > 0)
		{
			venti -= (variableSoustrait * Time.deltaTime);
			transform.rotation *= Quaternion.AngleAxis(venti, Vector3.up);
		}
	}

	void OnMouseDown()
			{
					venti += variableAdditionnel;
					transform.rotation *= Quaternion.AngleAxis(venti, Vector3.up);
			}
}
