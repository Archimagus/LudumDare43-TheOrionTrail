using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChoiceButton : MonoBehaviour
{
	private struct ItemData
	{
		public string item;
		public bool isPositive;
	}

	public TextMeshProUGUI Description;
	public TextMeshProUGUI Results;

	private Choice _choice;
	public Choice TheChoice
	{
		set
		{
			_choice = value;
			Description.text = _choice.Description;
			var effectsData = _choice.Data.Data;

			var visibleEffects = effectsData;

			int itemCount = 0;
			string outputString = string.Empty;
			string chanceString = string.Empty;

			List<string> chanceStrings = new List<string>();
			List<string> resourceStrings = new List<string>();


			foreach (var effect in visibleEffects)
			{
				if (effect.Key.Contains("chance"))
				{
					chanceString = $"(<sprite name=\"{effect.Key.Capitalize()}\"> {effect.Value.ToString()}%)  ";
				}
				else if (chanceString != string.Empty)
				{
					outputString += $"<sprite name=\"{effect.Key.Capitalize()}\"> {effect.Value.ToString("+#;-#;0")}" + chanceString;
					chanceString = string.Empty;
					chanceStrings.Add(outputString);
				}
				else
				{
					outputString += $"<sprite name=\"{effect.Key.Capitalize()}\"> {effect.Value.ToString("+#;-#;0")}  ";
					resourceStrings.Add(outputString);
				}

				outputString = string.Empty;
			}

			outputString = string.Empty;

			foreach (string text in chanceStrings)
				outputString += text + "\r\n";

			foreach (string text in resourceStrings)
			{
				if (itemCount % 2 != 0)
				{
					outputString += text + "\r\n";
				}
				else
				{
					outputString += text;
				}

				itemCount++;
			}


			Results.SetText(outputString);

			//.Select(data => new ItemData
			//{
			//	item = $"<sprite name=\"{data.Key.Capitalize()}\"> {data.Value.ToString("+#;-#;0")}",
			//	isPositive = data.Value >= 0
			//}).ToList();

			//var positiveEffects = visibleEffects.Where(e => e.isPositive).ToList();
			//var negativeEffects = visibleEffects.Where(e => !e.isPositive).ToList();

			//var effectStrings = negativeEffects.Interleve(positiveEffects).Select(e => e.item).ToList();
			//Results.SetText(string.Join("\r\n", effectStrings));
		}

		get
		{
			return _choice;
		}
	}

	public void HandleClick()
	{
		if (_choice != null) _choice.Activate();
	}
}