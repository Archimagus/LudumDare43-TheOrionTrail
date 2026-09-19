using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameAnalyticsOrion : MonoBehaviour
{
	[SerializeField] private IntReference _food;
	[SerializeField] private IntReference _water;
	[SerializeField] private IntReference _fuel;
	[SerializeField] private IntReference _people;
	[SerializeField] private IntReference _fighters;

	[SerializeField] private IntReference _foodShip;
	[SerializeField] private IntReference _waterShip;
	[SerializeField] private IntReference _fuelShip;
	[SerializeField] private IntReference _peopleShip;
	[SerializeField] private IntReference _fightersShip;

	[SerializeField] private FloatReference _progress;
	[SerializeField] private FloatReference _maxProgress;


	public static GameAnalyticsOrion Instance { get; private set; }

	private void Awake()
	{
		Instance = this;

		GameAnalytics.Initialize(new AnalyticsOptions
		{
			Endpoint = AnalyticsSecrets.AnalyticsEndpoint,
			GameId = "orion-trail",
			IngestToken = AnalyticsSecrets.IngestKey,
			Environment = "production",
			BuildVersion = Application.version
		});
	}

	public void TrackProgress<T>(string eventType, T data)
	{
		GameAnalytics.TrackProgress(eventType, _progress.Value, _maxProgress.Value, data);
	}

	public void OnGameStart()
	{
		GameAnalytics.StartSession(
			GetSnapshot(),
			sessionId => Debug.Log(sessionId == null ? "Analytics unavailable" : "Analytics ready")
		);
	}

	public OrionSnapshot GetSnapshot() => new OrionSnapshot
	{
		resources = new OrionResources
		{
			fuel = _fuel.Value,
			water = _water.Value,
			food = _food.Value,
			people = _people.Value,
			fighters = _fighters.Value
		},
		fleetHealth = new OrionFleetHealth
		{
			fighters = _fightersShip.Value,
			water = _waterShip.Value,
			civilian = _peopleShip.Value,
			food = _foodShip.Value,
			fuel = _fuelShip.Value
		}
	};

	[Serializable]
	public sealed class OrionResources
	{
		public int fuel;
		public int water;
		public int food;
		public int people;
		public int fighters;
	}

	[Serializable]
	public sealed class OrionFleetHealth
	{
		public int fighters;
		public int water;
		public int civilian;
		public int food;
		public int fuel;
	}

	[Serializable]
	public sealed class OrionSnapshot
	{
		public OrionResources resources;
		public OrionFleetHealth fleetHealth;
	}
	[Serializable]
	private sealed class EncounterInfo
	{
		public string kind;
		public string id;
		public string name;
		public string description;
	}

	[Serializable]
	private sealed class EncounterShownData
	{
		public OrionResources resources;
		public OrionFleetHealth fleetHealth;
		public EncounterInfo encounter;
	}

	public void TrackEncounter(string kind, Encounter encounter)
	{
		OrionSnapshot snapshot = GetSnapshot();

		TrackProgress(
			"encounter_shown",
			new EncounterShownData
			{
				resources = snapshot.resources,
				fleetHealth = snapshot.fleetHealth,
				encounter = new EncounterInfo
				{
					kind = kind,
					id = encounter.name,
					name = encounter.name,
					description = encounter.Description,
				}
			}
		);
	}

	[Serializable]
	private sealed class ChoiceInfo
	{
		public string description;
		[SerializeField]
		public Dictionary<string, int> effects;
	}

	[Serializable]
	private sealed class ChoiceMadeData
	{
		public OrionResources resources;
		public OrionFleetHealth fleetHealth;
		public EncounterInfo encounter;
		public ChoiceInfo choice;
	}
	public void TrackChoice(string encounterKind, Encounter encounter, Choice choice)
	{
		OrionSnapshot snapshot = GetSnapshot();

		TrackProgress(
			"choice_made",
			new ChoiceMadeData
			{
				resources = snapshot.resources,
				fleetHealth = snapshot.fleetHealth,
				encounter = new EncounterInfo
				{
					kind = encounterKind,
					id = encounter.name,
					name = encounter.name,
					description = encounter.Description
				},
				choice = new ChoiceInfo
				{
					description = choice.Description,
					effects = choice.Data.Data.ToDictionary(effect => effect.Key.ToLowerInvariant(), effect => effect.Value)
				}
			}
		);
	}

	[Serializable]
	private sealed class OptionalEncounterSpottedData
	{
		public float upcomingAtProgress;
	}

	public void OnOptionalEncounterSpotted(DataVariable data)
	{
		int? target = data["TIME"];

		Debug.Log($"Optional encounter spotted at {target}");
		TrackProgress(
			"optional_encounter_spotted",
			new OptionalEncounterSpottedData
			{
				upcomingAtProgress = target ?? 0
			});
	}

	public void OnOptionalEncounterAvailable()
	{
		Debug.Log("Optional encounter available");
		TrackProgress("optional_encounter_available", new { });
	}

	public void OnOptionalEncounterPassed()
	{
		Debug.Log("Optional encounter passed");
		TrackProgress("optional_encounter_passed", new { });
	}
}