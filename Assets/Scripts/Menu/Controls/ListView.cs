using System.Collections.Generic;
using UnityEngine;

public class ListView : MonoBehaviour
{
	#region Vars

	[SerializeField]
	private GameObject _rowTemplate;
	private float _templateHeight;

	[SerializeField]
	private Transform _container;

	[SerializeField]
	private List<ListItem> _rows;

	#endregion

	#region Methods

	private void Start()
	{
		_templateHeight = _rowTemplate.GetComponent<RectTransform>().sizeDelta.y;
	}

	public ListItem AddRow(string[] values, int preferedIndex = -1)
	{
		// Create object
		GameObject newRow = Instantiate<GameObject>(_rowTemplate);
		RectTransform rect = newRow.GetComponent<RectTransform>();
		ListItem listItem = newRow.GetComponent<ListItem>();
		rect.SetParent(_container, false);

		// Add to row list and calc index
		int finalIndex;
		if (preferedIndex >= 0 && preferedIndex < _rows.Count)
		{
			_rows.Insert(preferedIndex, listItem);
			finalIndex = preferedIndex;
		}
		else
		{
			finalIndex = _rows.Count;
			_rows.Add(listItem);
		}

		// Position and rescale
		rect.localPosition = new Vector3(0, CalcRowY(finalIndex));
		rect.offsetMin = new Vector2(0f, rect.offsetMin.y);
		rect.offsetMax = new Vector2(0f, rect.offsetMax.y);

		// Apply values
		listItem.ApplyNewValues(values);

		// Make visible
		newRow.SetActive(true);

		ChangeContainerHeight();

		return listItem;
	}

	public void RemoveRow(int rowIndex)
	{
		Destroy(_rows[rowIndex].gameObject);
		_rows.RemoveAt(rowIndex);
	}

	public void Clear()
	{
		for (int i = _rows.Count - 1; i >= 0; i--)
		{
			RemoveRow(i);
		}
	}

	public void ApplyValues(string[][] vals)
	{
		for (int i = 0; i < _rows.Count && i < vals.Length; i++)
		{
			_rows[i].ApplyNewValues(vals[i]);
		}
		ChangeContainerHeight();
	}

	private void ChangeContainerHeight()
	{
		RectTransform rect = _container.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector3(rect.sizeDelta.x, Mathf.Abs(CalcRowY(_rows.Count)));
	}

	private float CalcRowY(int index, float spacing = 5f)
	{
		return -index * (_templateHeight + spacing);
	}

	#endregion

	#region Properties

	public ListItem[] Rows
	{
		get { return _rows.ToArray(); }
	}

	#endregion
}
