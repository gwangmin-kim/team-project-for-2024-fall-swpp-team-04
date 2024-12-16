using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GoToMainMenu();
		}
	}

	public void GoToMainMenu()
	{
		Destroy(GameManager.Instance.gameObject);
		StartCoroutine(LoadMainMenu());
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
