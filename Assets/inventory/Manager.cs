using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : BaseMonoBehaviour
{
	public Camera mainCamera;

	public static Manager instance;
	protected override void Awake()
	{
		base.Awake();

		instance = this;
	}
}
