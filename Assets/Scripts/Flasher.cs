using System.Collections;
using UnityEngine;

#pragma warning disable 0649
public class Flasher : MonoBehaviour
{
	[SerializeField] private float _flashTime;
	[SerializeField] private MonoBehaviour _component;
	private WaitForSeconds _flashWait;
	private void OnEnable()
	{
		_flashWait = new WaitForSeconds(_flashTime);
		_component.enabled = true;
		 StartCoroutine(doFlashing());
		IEnumerator doFlashing()
		{
			while(true)
			{
				yield return _flashWait;
				_component.enabled = !_component.enabled;
			}
		}
	}
	private void OnDisable()
	{
		StopAllCoroutines();
	}
}
