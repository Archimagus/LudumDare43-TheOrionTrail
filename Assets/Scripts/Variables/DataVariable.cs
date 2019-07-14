using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataVariable : ScriptableObject
{
	public EventData Data;

	public int Count { get { return Data.Count; } }
	public KVP this[int index]
	{
		get { return Data[index]; }
		set { Data[index] = value; }
	}
	public int? this[string key]
	{
		get { return Data.Data.FirstOrDefault(k => k.Key == key)?.Value; }
	}
	public IEnumerator GetEnumerator()
	{
		return Data.GetEnumerator();
	}
}
[System.Serializable]
public class EventData : IEnumerable
{
	public List<KVP> Data = new List<KVP>();
	public int Count { get { return Data.Count; } }
	public KVP this[int index]
	{
		get { return Data[index]; }
		set { Data[index] = value; }
	}
	public int? this[string key]
	{
		get { return Data.FirstOrDefault(k => k.Key == key)?.Value; }
	}
	public IEnumerator GetEnumerator()
	{
		return Data.GetEnumerator();
	}
}


[System.Serializable]
public class KVP
{
	public string Key;
	public int Value;
	public KVP()
	{

	}
	public KVP(string key, int value)
	{
		Key = key;
		Value = value;
	}
}