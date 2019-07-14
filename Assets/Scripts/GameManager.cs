using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 0649
public class GameManager : MonoBehaviour
{
	[SerializeField] private FloatReference _progress;
	[SerializeField] private FloatReference _progressGoal;
	[SerializeField] private FloatReference _warpTime;
	[SerializeField] private FloatReference _preWarpDelay = new FloatReference(1);
	[SerializeField] private FloatReference _minProgressBeforeRandomEncounter = new FloatReference(10);
	[SerializeField] private FloatReference _maxProgressBeforeRandomEncounter = new FloatReference(20);
	[SerializeField] private FloatReference _minProgressBeforeOptionalEncounter = new FloatReference(5);
	[SerializeField] private FloatReference _maxProgressBeforeOptionalEncounter = new FloatReference(25);
	[SerializeField] private FloatReference _encounterSpotThreshold;
	[SerializeField] private FloatReference _encounterClickThreshold;

	public GameEvent WarpStarted;
	public GameEvent LeftWarp;
	public GameEvent RandomEncounter;
	public GameEvent OptionalEncounter;
	public GameEvent OptionalEncounterSpotted;
	public GameEvent OptionalEncounterAvailable;
	public GameEvent OptionalEncounterPassed;
	public GameEvent GameOver;
	public GameEvent GameStart;
	public GameEvent GameWon;
	public GameEvent MusicStageOne;
	public GameEvent MusicStageTwo;
	public GameEvent MusicStageThree;
	public GameEvent MusicStageFour;

	public ResourceDepletion[] ResourceRates;

	public IntReference NumberOfShips = new IntReference(5);

	private GameTime _gameTime;
	private MenuStack _menuStack;
	private Coroutine _warpCoroutine;
	private float _nextOptionalEncounterTime;

	private MusicStage _musicStage;
	private float _musicStageTwoProgressValue = 0.4f;
	private float _musicStageThreeProgressValue = 0.7f;
	private float _musicStageFourProgressValue = 0.85f;
	private float _randomEncounterTime;

	public static GameManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		var uiScene = SceneManager.GetSceneByName("UI");
		if (uiScene.buildIndex == -1)
		{
			SceneManager.LoadScene("UI", LoadSceneMode.Additive);
		}

		_gameTime = Resources.Load<GameTime>("GameTime");
		if (_gameTime == null)
			Debug.LogError("GameTime not found");

		_menuStack = Resources.Load<MenuStack>("MenuStack");
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");

		resetResources();

		foreach (var r in ResourceRates)
		{
			r.TimeRemaining = r.DepletionRate;
		}

		_musicStage = MusicStage.ONE;
		GameStart.Raise();
	}

	private void resetResources()
	{
		var tvars = Resources.LoadAll<TVariable>("");
		foreach (var v in tvars)
		{
			v.Reset();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_gameTime.MenuPause)
				_menuStack.CloseMenu();
			else
				_menuStack.OpenMenu(PauseMenu.Instance.PausePanel);
		}
	}
	public void GoToWarp()
	{
		if (_warpCoroutine != null)
		{
			stopWarp();
			return;
		}
		_warpTime.Value = -_preWarpDelay.Value;
		if(_randomEncounterTime <= 0)
			_randomEncounterTime = _progress.Value + Random.Range(_minProgressBeforeRandomEncounter, _maxProgressBeforeRandomEncounter);
		queueOptionalEncounter();
		_warpCoroutine = StartCoroutine(runWarp());
		IEnumerator runWarp()
		{
			do
			{
				yield return null;
				var p = _progress.Value;
				var t = _warpTime.Value;
				_warpTime.Value += Time.deltaTime;

				if (t < 0 && _warpTime.Value >= 0)
				{
					enterWarp();
				}

				if (_warpTime >= 0)
				{
					_progress.Value += Time.deltaTime;
					CheckMusicStage();

					if(_progress.Value >= _progressGoal.Value)
					{
						gameWon();
						StopCoroutine(_warpCoroutine);
					}

					foreach (var r in ResourceRates)
					{
						r.TimeRemaining -= Time.deltaTime;
						if (r.TimeRemaining <= 0)
						{
							r.Resource.CurrentValue--;
							r.TimeRemaining = r.DepletionRate;
						}
					}
					if (ResourceRates.Any(r => r.Resource.CurrentValue <= 0))
					{
						gameOver();
						StopCoroutine(_warpCoroutine);
					}

					var lastEncounterDistance = _nextOptionalEncounterTime - p;
					var encounterDistance = _nextOptionalEncounterTime - _progress.Value;
					if (lastEncounterDistance > _encounterSpotThreshold && encounterDistance <= _encounterSpotThreshold)
					{
						spotEncounter();
					}
					if (lastEncounterDistance > _encounterClickThreshold && encounterDistance < _encounterClickThreshold)
					{
						OptionalEncounterAvailable.Raise();
					}
					if (lastEncounterDistance >= 0 && encounterDistance < 0)
					{
						OptionalEncounterPassed.Raise();
						queueOptionalEncounter();
					}
				}

			} while (_progress < _randomEncounterTime);
			doRandomEncounter();
			_warpCoroutine = null;
		}
	}

	private void CheckMusicStage ()
	{
		float progressPercentage = _progress / _progressGoal;

		if (_musicStage == MusicStage.ONE && progressPercentage > _musicStageTwoProgressValue)
		{
			_musicStage = MusicStage.TWO;
			MusicStageTwo.Raise();
		}
		else if (_musicStage == MusicStage.TWO && progressPercentage > _musicStageThreeProgressValue)
		{
			_musicStage = MusicStage.THREE;
			MusicStageThree.Raise();
		}
		else if (_musicStage == MusicStage.THREE && progressPercentage > _musicStageFourProgressValue)
		{
			_musicStage = MusicStage.FOUR;
			MusicStageFour.Raise();
		}
	}


	private void queueOptionalEncounter()
	{
		_nextOptionalEncounterTime = _progress.Value + Random.Range(_minProgressBeforeOptionalEncounter, _maxProgressBeforeOptionalEncounter);

		var encounterDistance = _nextOptionalEncounterTime - _progress.Value;
		if (encounterDistance <= _encounterSpotThreshold)
		{
			spotEncounter();
		}
		if (encounterDistance <= _encounterClickThreshold)
		{
			OptionalEncounterAvailable.Raise();
		}
	}
	private void spotEncounter()
	{
		var ed = new EventData();
		ed.Data.Add(new KVP("TIME", (int)_nextOptionalEncounterTime));
		OptionalEncounterSpotted.Raise(ed);
	}

	private void gameWon()
	{
		GameWon.Raise();
		SceneManager.LoadScene("GameWon");
	}
	private void gameOver()
	{
		GameOver.Raise();
		SceneManager.LoadScene("GameOver");
	}

	private void enterWarp()
	{
		WarpStarted.Raise();
	}
	private void stopWarp()
	{
		StopCoroutine(_warpCoroutine);
		_warpCoroutine = null;
		_warpTime.Value = 0;
		LeftWarp.Raise();
		var encounterDistance = _nextOptionalEncounterTime - _progress.Value;
		if (encounterDistance > 0 && encounterDistance < _encounterClickThreshold)
			OptionalEncounter.Raise();
		_nextOptionalEncounterTime = 0;
	}
	private void doRandomEncounter()
	{
		_randomEncounterTime = 0;
		_warpTime.Value = 0;
		LeftWarp.Raise();
		RandomEncounter.Raise();
	}
	public void QuitToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
	public void ShipDeath()
	{
		NumberOfShips.Value--;
		if(NumberOfShips <=0)
		{
			gameOver();
		}
	}
}

[System.Serializable]
public class ResourceDepletion
{
	public IntVariable Resource;
	public FloatVariable DepletionRate;
	public float TimeRemaining { get; set; }
}
[System.Serializable]
public class ShipHealthWatcher
{
	public IntVariable ShipHealth;
	public GameEvent DestroyedEvent;
}


enum MusicStage
{
	ONE,
	TWO,
	THREE,
	FOUR
}