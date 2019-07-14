using UnityEngine;

[CreateAssetMenu]
public class IntVariable : TVariable<int>
{
	public bool Clamp = false;

	protected override void SetValue(int val)
	{
		if (Clamp && val > Value)
			val = Value;

		base.SetValue(val);
	}

}
[System.Serializable]
public class IntReference : TReference<int, IntVariable>
{
	public IntReference(int initial) : base(initial)
	{
	}
}
