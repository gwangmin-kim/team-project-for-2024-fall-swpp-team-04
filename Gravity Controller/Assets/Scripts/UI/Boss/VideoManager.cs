using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using System;
using TMPro;

public class VideoManager : MonoBehaviour
{
	public VideoPlayer _videoPlayer;
	public BossStageController _bossStageController;
	[SerializeField] private CanvasGroup _videoCanvasGroup;
	[SerializeField] private float _fadeDuration = 1.5f;
	[SerializeField] private GameObject _bossStage;

	[Header("Grade Canvas")]
	[SerializeField] private GameObject _gradeCanvas; 
	[SerializeField] private TextMeshProUGUI _clearTimeText; 
	[SerializeField] private TextMeshProUGUI _gradeText;

	void Start()
	{
		_videoPlayer.loopPointReached += HandleVideoEnded;
		_videoPlayer.playOnAwake = false;
		_gradeCanvas.SetActive(false);
	}

	// 스테이지 이동 완료 후 BossStageController에서 호출할 예정
	public void PlayVideo()
	{
		if (_videoPlayer != null)
		{
			StartCoroutine(FadeInAndPlay());
		}
	}

	private IEnumerator FadeInAndPlay()
	{
		yield return StartCoroutine(FadeCanvasGroup(_videoCanvasGroup, 0f, 1f, _fadeDuration));
		_videoPlayer.Play();
	}

	private void HandleVideoEnded(VideoPlayer vp)
	{
		_bossStage.gameObject.SetActive(false);

		BossStageController stage = FindObjectOfType<BossStageController>();
		if (stage != null && stage._finalCanvas != null)
		{
			stage._finalCanvas.SetActive(true);
			DisplayClearTime();
			_gradeCanvas.SetActive(true); 
			CanvasGroup finalCanvasGroup = stage._finalCanvas.GetComponent<CanvasGroup>();
			StartCoroutine(FadeCanvasGroup(finalCanvasGroup, 0f, 1f, _fadeDuration));
		}
	}
	private void DisplayClearTime()
	{
		string startTimeString = PlayerPrefs.GetString("GameStartTime", null);
		if (string.IsNullOrEmpty(startTimeString))
		{
			Debug.LogError("GameStartTime is not set!");
			return;
		}

		DateTime startTime = DateTime.Parse(startTimeString);
		TimeSpan elapsedTime = DateTime.Now - startTime;
		_clearTimeText.text = $"Time: {FormatTime(elapsedTime)}";
		_gradeText.text = $"Grade: {GetGrade(elapsedTime.TotalSeconds)}";
	}

	private string FormatTime(TimeSpan elapsedTime)
	{
		return string.Format("{0:D2}:{1:D2}:{2:D2}", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
	}

	private string GetGrade(double elapsedSeconds) 
	{ 
		if (elapsedSeconds < 180) return "SSS";
		if (elapsedSeconds < 210) return "SS";
		if (elapsedSeconds < 240) return "S";
		if (elapsedSeconds < 270) return "A+"; 
		if (elapsedSeconds < 300) return "A"; 
		if (elapsedSeconds < 330) return "A-";
		if (elapsedSeconds < 360) return "B+"; 
		if (elapsedSeconds < 390) return "B"; 
		if (elapsedSeconds < 420) return "B-";
		if (elapsedSeconds < 450) return "C+";
		if (elapsedSeconds < 480) return "C"; 
		if (elapsedSeconds < 510) return "C-";
		return "D+"; 
	}

	private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
	{
		float elapsed = 0f;
		canvasGroup.alpha = startAlpha;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / duration);
			canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
			yield return null;
		}

		canvasGroup.alpha = endAlpha;
	}
}
