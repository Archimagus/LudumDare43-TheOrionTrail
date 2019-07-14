using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeMusic : MonoBehaviour
{
	public AudioClips testClip1;
	public AudioClips testClip2;

	private bool _switch = true;

	public void ChangeMusic ()
	{
		AudioClips c = (_switch) ? testClip1 : testClip2;
		AudioManager.PlayMusic(AudioClips.theme);
		_switch = !_switch;
	}
}
