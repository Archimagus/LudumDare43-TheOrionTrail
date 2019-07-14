using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsPanel : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private Slider MasterSlider;
	[SerializeField] private Slider MusicSlider;
	[SerializeField] private Slider SoundEffectsSlider;
	[SerializeField] private Slider AmbienceSlider;
	[SerializeField] private Slider DialogueSlider;
	[SerializeField] private Slider UISlider;

	private float _musicVolume;
	private float _soundEffectsVolume;
	private float _ambienceVolume;
	private float _dialogueVolume;
	private float _uIVolume;

	public float MasterVolume
	{
		get
		{
			return AudioManager.MasterVolume;
		}
		set
		{
			AudioManager.MasterVolume = value;
		}
	}
	public float MusicVolume
	{
		get
		{
			return AudioManager.MusicVolume;
		}
		set
		{
			AudioManager.MusicVolume = value;
		}
	}
	public float SoundEffectsVolume
	{
		get
		{
			return AudioManager.SoundEffectsVolume;
		}
		set
		{
			AudioManager.SoundEffectsVolume = value;
		}
	}
	public float AmbienceVolume
	{
		get
		{
			return AudioManager.AmbienceVolume;
		}
		set
		{
			AudioManager.AmbienceVolume = value;
		}
	}
	public float DialogueVolume
	{
		get
		{
			return AudioManager.DialogueVolume;
		}
		set
		{
			AudioManager.DialogueVolume = value;
		}
	}
	public float UIVolume
	{
		get
		{
			return AudioManager.InterfaceVolume;
		}
		set
		{
			AudioManager.InterfaceVolume = value;
		}
	}
	void OnEnable()
	{
		MasterSlider.value = AudioManager.MasterVolume;
		MusicSlider.value = AudioManager.MusicVolume;
		SoundEffectsSlider.value = AudioManager.SoundEffectsVolume;
		UISlider.value = AudioManager.InterfaceVolume;
	}
}
