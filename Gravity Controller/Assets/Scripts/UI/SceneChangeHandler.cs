using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneChangeHandler : MonoBehaviour
{
	public Text start;
	public Button startButton;

	private SettingsSave _settingsSave;
	private GameSave _gameSave;

	[SerializeField] private GameObject _textCanvas;
	[SerializeField] private GameObject _loadingCanvas;

	[SerializeField] private GameObject _alert;
	private bool _hasResponded = false;
	private bool _responce;

	delegate void AfterAlert();
	private AfterAlert _afterAlert;

	readonly string _alertTextSaveExist = "save file detected.\r\nStarting a new game overwrites your existing save file. \r\nproceed?";
	readonly string _alertTextSaveDoesntExist = "save file has no progress or does not exist.\r\nStart a new game instead?";

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
	}

	public void LoadGame()
	{
		FetchSaves();
		if (_gameSave.HasProgressed())
		{
			// savefile exists
			if (!_hasResponded)
			{
				_afterAlert = LoadGame;
				Alert(_alertTextSaveExist);
				return;
			}
			else
			{
				_hasResponded = false;
				if (!_responce)
				{
					// No
					return;
				}
				// Yes
			}
		}
		PlayerPrefs.SetString("GameStartTime", DateTime.Now.ToString());
		_loadingCanvas.SetActive(true);
		_textCanvas.SetActive(false);
		_loadingCanvas.GetComponent<LoadingCanvas>().StartLoading();
		StartCoroutine(LoadGameCoroutine("UnitedScene"));
	}

	public void ContinueGame()
	{
		FetchSaves();
		if (!_gameSave.HasProgressed())
		{
			// save is empty
			if (!_hasResponded)
			{
				_afterAlert = ContinueGame;
				Alert(_alertTextSaveDoesntExist);
				return;
			}
			else
			{
				_hasResponded = false;
				if (!_responce)
				{
					// No
					return;
				}
				// Yes
			}
		}

		ContinueGameWithoutAlert();
	}

	public void ContinueGameWithoutAlert()
	{
		PlayerPrefs.SetString("GameStartTime", DateTime.Now.ToString());
		_loadingCanvas.SetActive(true);
		_textCanvas.SetActive(false);
		_loadingCanvas.GetComponent<LoadingCanvas>().StartLoading();
		StartCoroutine(ContinueGameCoroutine("UnitedScene"));
	}

	private void InitGameSave()
	{
		var player = GameObject.Find("Player");
		GameObject summonPoint = null;
		if (_gameSave.atLobby) { 
			summonPoint = GameObject.Find("Summon Point").transform.GetChild(0).gameObject;
			BGMManager.Instance.SetStageBGM(0);
		}
		else {
			summonPoint = GameObject.Find("Summon Point").transform.GetChild(1).GetChild(_gameSave.stage - 1).gameObject;
			BGMManager.Instance.SetStageBGM(_gameSave.stage);
		}
		player.transform.position = summonPoint.transform.position;
		player.transform.rotation = summonPoint.transform.rotation;

		var coreController = StageManager.Instance.gameObject.GetComponent<CoreController>();
		if (_gameSave.atLobby)
		{
			coreController.OnLoad(_gameSave.stage - 1);
			StageManager.Instance.InitIsCleared(_gameSave.stage - 1);
			player.GetComponent<PlayerController>().UpdateStage(_gameSave.stage);
			UIManager.Instance.EnergyGaugeUi();
			StageManager.Instance.LoadStage(_gameSave.stage - 1);
		}
	}

	private void InitSettingsSave()
	{
		var player = GameObject.Find("Player");
		PlayerCamera.SetSensitivityMultiplier(_settingsSave.sensitivity);
		GameManager.Instance.SetBGMVolume(_settingsSave.backGroundVolume/100f);
		GameManager.Instance.SetSFXVolume(_settingsSave.effectVolume/100f);

		// (Temporary) Set initial position
		GameObject summonPoint = GameObject.Find("Summon Point").transform.GetChild(1).GetChild(0).gameObject;
		player.transform.position = summonPoint.transform.position;
	}

	IEnumerator LoadGameCoroutine(string scene)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
		//var mainMenu = SceneManager.GetActiveScene();
		//SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
		InitSettingsSave();
		//SceneManager.UnloadSceneAsync(mainMenu);

		Autosave.SaveGameSave(false, 1);

		Destroy(_loadingCanvas);
		Destroy(gameObject);
	}

	IEnumerator ContinueGameCoroutine(string scene)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
		//var mainMenu = SceneManager.GetActiveScene();
		//SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
		InitSettingsSave();
		InitGameSave();
		//SceneManager.UnloadSceneAsync(mainMenu);

		Destroy(_loadingCanvas);
		Destroy(gameObject);
	}

	public void FetchSaves()
	{
		_gameSave = CanvasSwitcher.Instance.gameSave;
		_settingsSave = CanvasSwitcher.Instance.settingsSave;
	}

	private void Alert(string text)
	{
		_alert.transform.Find("TextPanel").transform.GetChild(0).GetComponent<Text>().text = text;
		_alert.SetActive(true);
	}

	public void Yes()
	{
		//Debug.Log("Yes");
		_hasResponded = true;
		_responce = true;
		_alert.SetActive(false);
		_afterAlert();
	}

	public void No()
	{
		//Debug.Log("No");
		_hasResponded = true;
		_responce = false;
		_alert.SetActive(false);
		_afterAlert();
	}
}
