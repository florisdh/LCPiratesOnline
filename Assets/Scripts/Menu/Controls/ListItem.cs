using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
	#region Vars

	[SerializeField]
	private Text[] _cells;

	#endregion

	#region Methods

	public void ApplyNewValues(string[] values)
	{
		for (int i = 0; i < _cells.Length && i < values.Length; i++)
		{
			_cells[i].text = values[i];
		}
	}

	#endregion
}
