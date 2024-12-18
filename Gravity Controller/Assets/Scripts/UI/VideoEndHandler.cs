using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoEndHandler : MonoBehaviour
{
	public VideoPlayer videoPlayer;
	public GameObject uiGroup;
	private CanvasGroup _canvasGroup;
	public float fadeDuration = 1.0f;
	private bool _videoEnded = false;

	private void Awake()
	{
		_videoEnded = false;
	}

	void Start()
	{
		_canvasGroup = uiGroup.GetComponent<CanvasGroup>();
		if (_canvasGroup == null)
		{
			_canvasGroup = uiGroup.AddComponent<CanvasGroup>();
		}

		_canvasGroup.alpha = 0f;
		uiGroup.SetActive(false);

		videoPlayer.loopPointReached += OnVideoEnd;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !_videoEnded)
		{
			EndVideo();
		}
	}
	void OnVideoEnd(VideoPlayer vp)
	{
		_videoEnded = true;
		uiGroup.SetActive(true);
		StartCoroutine(FadeInCanvasGroup(_canvasGroup, fadeDuration));
	}

	private IEnumerator FadeInCanvasGroup(CanvasGroup cg, float duration)
	{
		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			cg.alpha = Mathf.Clamp01(elapsed / duration);
			yield return null;
		}

		cg.alpha = 1f; 
	}

	public void EndVideo()
	{
		_videoEnded = true;

		//videoPlayer.Stop();
		videoPlayer.time = 30f;
		videoPlayer.Pause();
		OnVideoEnd(videoPlayer);
	}
}
