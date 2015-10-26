using UnityEngine;
using System.Collections.Generic;

public class LevelSettings : MonoBehaviour
{
	public static LevelSettings Current;
	#region Vars

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
