using UnityEngine;

[CreateAssetMenu]
public class BoolVariable : TVariable<bool>
{
}
[System.Serializable]
public class BoolReference : TReference<bool, BoolVariable>
{
	public BoolReference(bool initial) : base(initial)
	{
	}
}
