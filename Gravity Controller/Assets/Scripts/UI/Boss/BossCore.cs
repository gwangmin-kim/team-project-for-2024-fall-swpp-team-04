using System.Collections;
using UnityEngine;

public class BossCore : MonoBehaviour, IInteractable
{
	[SerializeField] private Light _coreLight;
	[SerializeField] private GameObject _textCanvas;
	[SerializeField] private GameObject _videoCanvas;

	private VideoManager _videoManager;
	private bool _isInteractive= true;

	void Start()
	{
		_textCanvas.SetActive(false);
		_videoCanvas.SetActive(false);
		_coreLight.enabled = false;
		_videoManager = _videoCanvas.GetComponent<VideoManager>();
	}

	public void Interactive()
	{
		StartInteraction();
	}

	public bool IsInteractable()
	{
		return _isInteractive;
	}
	private void StartInteraction()
	{
		_isInteractive = false;
		_coreLight.enabled = true;

		// 영상이 아닌 텍스트 먼저 노출
		StartCoroutine(ShowTextFirstCoroutine());
	}

	private IEnumerator ShowTextFirstCoroutine()
	{
		// 텍스트 캔버스 활성화
		_textCanvas.SetActive(true);
		yield return new WaitForSeconds(3f); // 3초 노출
		_textCanvas.SetActive(false);		

		// 텍스트 노출 후 스테이지 이동 시작
		if (_videoManager._bossStageController != null)
			_videoManager._bossStageController.StartMoving();
	}
}
