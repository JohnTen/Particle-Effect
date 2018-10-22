using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PrefabLibrary : ScriptableObject
{
	public ForceZone GravityZone;
	public ForceZone PositiveZone;
	public ForceZone NegativeZone;
	public ColorZone ColorZone;

	static PrefabLibrary _instance;
	public static PrefabLibrary Instance
	{
		get
		{
			if (_instance != null)
				return _instance;

			_instance = Resources.Load("PrefabLibrary") as PrefabLibrary;
			return _instance;
		}
	}
}
