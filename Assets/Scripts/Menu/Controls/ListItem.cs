using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
	#region Vars

	public object State;

	[SerializeField]
	private Text[] _cells;

	private string[] _values;

	#endregion

	#region Methods

	public void ApplyNewValues(string[] values)
	{
		_values = values;
		for (int i = 0; i < _cells.Length && i < values.Length; i++)
		{
			_cells[i].text = values[i];
		}
	}

	#endregion

	#region Properties

	public string[] Values
	{
		get { return _values; }
	}

	#endregion
}
