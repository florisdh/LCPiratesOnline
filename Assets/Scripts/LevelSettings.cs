using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelSettings : MonoBehaviour
{
	#region Events

	#endregion

	#region Vars

	public static LevelSettings Current;
	public List<Transform> SpawnPoints;

	#endregion

	#region Construct

	public LevelSettings() : base()
	{
		Current = this;
	}

	#endregion

	#region Methods

	#endregion
}
