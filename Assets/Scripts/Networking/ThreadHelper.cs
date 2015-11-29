using UnityEngine;
using System.Collections.Generic;
using System;

public class ThreadHelper : MonoBehaviour
{
	#region Vars

	public static ThreadHelper MAIN;

	private List<Action> _actionQueu;

	#endregion

	#region Construct

	public ThreadHelper()
	{
		_actionQueu = new List<Action>();
		MAIN = this;
	}

	#endregion

	#region Methods

	public void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void InvokeOnThread(Action action)
	{
		_actionQueu.Add(action);
	}

	private void Update()
	{
		if (_actionQueu.Count > 0)
		{
			lock (_actionQueu)
			{
				foreach (Action action in _actionQueu)
				{
					action();
				}
				_actionQueu.Clear();
			}
		}
	}

	#endregion
}
