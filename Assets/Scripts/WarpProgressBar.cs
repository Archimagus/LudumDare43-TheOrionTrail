using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class WarpProgressBar : MonoBehaviour
{
	[SerializeField] FloatReference _preWarpTime;
	[SerializeField] FloatReference _warpTime;
	[SerializeField] ProgressBar _progressBar;

	private void Awake()
	{
		_preWarpTime.Changed += _value_Changed;
		_warpTime.Changed += _value_Changed;
		setValue(_warpTime);
	}
	private void OnDestroy()
	{
		_preWarpTime.Changed -= _value_Changed;
		_warpTime.Changed -= _value_Changed;
	}
	private void _value_Changed(ReferenceChangedEventHandler<float> obj)
	{
		setValue(_warpTime);
	}

	private void setValue(float value)
	{
		if (_warpTime.Value < 0)
		{
			_progressBar.gameObject.SetActive(true);
			_progressBar.SetValue((int)(Mathf.Clamp01(Mathf.InverseLerp(-_preWarpTime.Value, 0, value)) * 1000));
		}
		else
		{
			_progressBar.gameObject.SetActive(false);
		}
	}
}
