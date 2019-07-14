using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : TVariable<float>
{
	public bool Clamp = false;

	protected override void SetValue(float val)
	{
		if (Clamp && val > Value)
			val = Value;

		base.SetValue(val);
	}
}
[System.Serializable]
public class FloatReference : TReference<float, FloatVariable>
{
	public FloatReference(float initial):base(initial)
	{
	}
}