using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListner : MonoBehaviour
{
	public GameEvent Event;
	public DataEvent Response;
	private DataVariable _dataVariable;

	private void Awake()
	{
		_dataVariable = ScriptableObject.CreateInstance<DataVariable>();
		_dataVariable.name = name + "Data";
	}
	private void OnEnable()
	{
		if(Event == null)
			Debug.LogWarning("No Event", this);
		Event?.RegisterListner(this);
	}
	private void OnDisable()
	{
		Event?.UnregisterListner(this);
	}

	public void OnEventRaised()
	{
		if (Response == null)
			Debug.LogWarning("No Response", this);
		_dataVariable.Data = null;
		Response?.Invoke(_dataVariable);
	}
	public void OnEventRaised(EventData data)
	{
		if (Response == null)
			Debug.LogWarning("No Response", this);
		_dataVariable.Data = data;
		Response?.Invoke(_dataVariable);
	}
}

[Serializable]
public class DataEvent : UnityEvent<DataVariable>
{

}