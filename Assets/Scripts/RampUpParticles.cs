using System.Collections;
using UnityEngine;

public class RampUpParticles : MonoBehaviour
{
	public ParticleSystem ParticleSystem;
	public FloatReference WarpDelay;
	float _rampUpTime;
	private ParticleSystem.MainModule _main;

	private void Awake()
	{
		_rampUpTime = -WarpDelay;
		_main = ParticleSystem.main;
	}
	public void StartWarp()
	{
		StartCoroutine(coroutine());
		IEnumerator coroutine()
		{
			_main.simulationSpeed = 1;
			ParticleSystem.Play(false);
			float t = _rampUpTime;
			while(t<0)
			{
				_main.simulationSpeed = Mathf.Clamp01(Mathf.InverseLerp(_rampUpTime, 0, t));
				yield return null;
				t += Time.deltaTime;
			}
			_main.simulationSpeed = 1;
		}
	}
	public void StopWarp()
	{
		StartCoroutine(coroutine());
		IEnumerator coroutine()
		{
			float t = 0;
			while (t > _rampUpTime)
			{
				_main.simulationSpeed = Mathf.Clamp01(Mathf.InverseLerp(_rampUpTime, 0, t));
				yield return null;
				t -= Time.deltaTime;
			}
			_main.simulationSpeed = 0;
			ParticleSystem.Clear(false);
			ParticleSystem.Stop(false);
		}
	}
}
