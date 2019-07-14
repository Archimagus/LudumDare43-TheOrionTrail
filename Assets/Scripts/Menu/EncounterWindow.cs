using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EncounterWindow : MonoBehaviour
{
	public EncounterReference _Encounter;
	public TextMeshProUGUI FlavorText;
	public int MaxChoices = 3;
	public List<ChoiceButton> Buttons;

	void Awake()
	{
		_Encounter.Changed += _Encounter_Changed;
		updateEncoutner();
	}

	private void _Encounter_Changed(ReferenceChangedEventHandler<Encounter> obj)
	{
		updateEncoutner();
	}

	private void updateEncoutner()
	{
		Encounter encounter = _Encounter.Value;
		FlavorText.text = encounter.Description;
		var choices = PickChoices(encounter).ToList();

		for (int i = 0, n = MaxChoices; i < n; ++i)
		{
			Buttons[i].TheChoice = choices[i];
		}
	}

	private IEnumerable<Choice> PickChoices(Encounter encounter)
	{
		var choices = encounter.Choices.Where(c => c.IsDefault).Take(MaxChoices).ToList();
		while (choices.Count < MaxChoices)
		{
			var pickFrom = encounter.Choices.Except(choices);
			choices.Add(pickFrom.Random());
		}
		return choices.Shuffle();
	}
}