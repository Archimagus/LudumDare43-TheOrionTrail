using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudioHandler : MonoBehaviour
{
	public void ButtonHover(BaseEventData eventData)
	{
		AudioManager.PlaySound(null, AudioClips.menu_button_hover_01, SoundType.Interface);
	}

	public void ButtonClick(BaseEventData eventData)
	{
		AudioManager.PlaySound(null, AudioClips.menu_button_click_01, SoundType.Interface);
	}
}
