using UnityEngine;
using UnityEngine.UI;

public class FleetProgressMonitor : MonoBehaviour
{
	public FloatReference Progress;
	public FloatReference MaxProgress;
	public FloatReference EncounterClickThreshold;

	public Slider ProgressSlider;
	public Slider NextEncounterSlider;
	public Image NextEncounterFlag;
	public Image NextEncounterIcon;

	private void Awake()
	{
		ProgressSlider.maxValue = MaxProgress;
		NextEncounterSlider.maxValue = MaxProgress;
		Progress.Changed += Progress_Changed;
		HideNextEncounter();
	}

	private void Progress_Changed(ReferenceChangedEventHandler<float> obj)
	{
		ProgressSlider.value = obj.NewValue;
	}

	public void ShowEvent(DataVariable data)
	{
		NextEncounterSlider.gameObject.SetActive(true);
		NextEncounterSlider.value = data[0].Value;
		for (int i = 1; i < data.Count; i++)
		{
			// TODO: Stuff with the data
		}
		NextEncounterFlag.enabled = false;
		NextEncounterIcon.enabled = false;
	}
	public void ShowEventDetails()
	{
		NextEncounterFlag.enabled = true;
		NextEncounterIcon.enabled = true;
	}
	public void HideNextEncounter()
	{
		NextEncounterFlag.enabled = false;
		NextEncounterIcon.enabled = false;
		NextEncounterSlider.gameObject.SetActive(false);
	}
}
