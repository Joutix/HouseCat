using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonoBehaviour : MonoBehaviour
{
	protected virtual void OnValidate() { }
	protected virtual void Awake() { }
	protected virtual void Start() { }
	protected virtual void Update() { }

	protected Manager manager => Manager.instance;
	protected new Camera camera => Manager.instance.mainCamera;
	protected Inventory inventory => Inventory.instance;
}
