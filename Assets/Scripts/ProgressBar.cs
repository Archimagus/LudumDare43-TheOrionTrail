using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class ProgressBar : MonoBehaviour
{
	[SerializeField] IntReference _min = new IntReference(0);
	[SerializeField] IntReference _max = new IntReference(1000);
	[SerializeField] IntReference _value;
	[SerializeField] Image _fillImage;

	private void Awake()
	{
		if(_value != null)
			_value.Changed += _value_Changed;
		SetValue(_value);
	}

	private void OnDestroy()
	{
		if (_value != null)
			_value.Changed -= _value_Changed;
	}
	private void _value_Changed(ReferenceChangedEventHandler<int> obj)
	{
		if(enabled)
			SetValue(obj.NewValue);
	}

	public void SetValue(int value)
	{
		_fillImage.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(_min, _max, value));
	}
}
