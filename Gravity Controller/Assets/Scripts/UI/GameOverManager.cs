using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
	public CanvasGroup _buttonsCanvasGroup;
	public float _fadeInDuration = 1.0f;
	public float _delayBeforeFade = 2.0f;

	void Start()
	{
		DontDestroyOnLoad(gameObject);

		_buttonsCanvasGroup.alpha = 0f;
		_buttonsCanvasGroup.interactable = false;
		_buttonsCanvasGroup.blocksRaycasts = false;

		Cursor.visible = true;

		StartCoroutine(FadeInButtons());
	}

	private IEnumerator FadeInButtons()
	{
		yield return new WaitForSeconds(_delayBeforeFade);

		float elapsedTime = 0f;
		while (elapsedTime < _fadeInDuration)
		{
			elapsedTime += Time.deltaTime;
			_buttonsCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / _fadeInDuration);
			yield return null;
		}

		_buttonsCanvasGroup.interactable = true;
		_buttonsCanvasGroup.blocksRaycasts = true;
	}

	public void RestartGame()
	{
		Destroy(GameManager.Instance.gameObject);
		StartCoroutine(ContinueGame());
	}

	public void GoToMainMenu()
	{
		Destroy(GameManager.Instance.gameObject);
		StartCoroutine(LoadMainMenu());
	}

	IEnumerator ContinueGame()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainSettingUI");
		while (!asyncLoad.isDone)
		{
			yield return null;
		}

		GameObject.Find("Video Canvas").transform.GetChild(0).gameObject.GetComponent<VideoEndHandler>().EndVideo();
		
		var sceneChangeHandler = GameObject.Find("ButtonManager").GetComponent<SceneChangeHandler>();
		sceneChangeHandler.FetchSaves();
		sceneChangeHandler.ContinueGameWithoutAlert();

		Destroy(gameObject);
	}

	IEnumerator LoadMainMenu()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainSettingUI");
		while (!asyncLoad.isDone)
		{
			yield return null;
		}

		GameObject.Find("Video Canvas").transform.GetChild(0).gameObject.GetComponent<VideoEndHandler>().EndVideo();

		Destroy(gameObject);
	}
}
