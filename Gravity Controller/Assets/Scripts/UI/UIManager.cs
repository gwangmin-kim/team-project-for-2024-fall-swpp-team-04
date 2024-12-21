using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] PlayerController _playerController;

	[Header("HP")]
	[SerializeField] private List<Image> _hpSegments;

	[Header("Energy")]
	[SerializeField] private Image _energyGauge;
	[SerializeField] private float _energyGaugeDamping;

	[Header("Bullet")]
	[SerializeField] private TextMeshProUGUI _bulletText;

	[Header("Crosshair")]
	[SerializeField] private RectTransform _crossHair;
	[SerializeField] private float _defaultSize = 100f;
	[SerializeField] private float _fireSize = 200f;
	
	[Header("Color Panel")]
	[SerializeField] private Image _colorPanel;
	[SerializeField] private float _colorPanelEffectDuration;
	private float _colorPanelEffectTimer = 0f;

	[Header("Core Interaction")]
	[SerializeField] private GameObject _warningTriangle;
	[SerializeField] private Slider _progressBar;
	[SerializeField] private GameObject _coreText;
	[SerializeField] private GameObject _doorText;

	[Header("Stage Intro UI")]
	[SerializeField] private GameObject _stageIntroBackground; 
	[SerializeField] private List<GameObject> _stageTexts;

	public static UIManager Instance { get; private set; }

	private bool _isFullyCharged = false;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		_warningTriangle.SetActive(false);
		_progressBar.gameObject.SetActive(false);

		if (_stageIntroBackground != null)
			_stageIntroBackground.SetActive(false);

		if (_stageTexts != null)
		{
			foreach (var text in _stageTexts)
			{
				if (text != null)
					text.gameObject.SetActive(false);
			}
		}
		_energyGauge.gameObject.SetActive(false);
	}

	public void UpdateHP(int currentHP)
	{
		for (int i = 0; i < _hpSegments.Count; i++)
		{
			_hpSegments[i].enabled = i < currentHP;
		}
	}

	// 기존 Energy 업데이트 메서드
	public void UpdateEnergy()
	{
		float energyRatio = PlayerEnergy.GetEnergyRatio();

		_energyGauge.fillAmount = Mathf.Lerp(_energyGauge.fillAmount, energyRatio, Time.deltaTime * _energyGaugeDamping);

		if (energyRatio >= 1f)
		{
			_energyGauge.color = new Color(75 / 255f, 0, 130 / 255f);
			if (!_isFullyCharged)
			{ 
				StartCoroutine(PlayFullEnergyEffect());
				_isFullyCharged = true; 
			}
		}
		else if (energyRatio > 1f / 3f)
		{
			_energyGauge.color = new Color(0.8f, 0.6f, 1f); 
			_isFullyCharged = false; 
		}
		else
		{
			_energyGauge.color = Color.white;
			_isFullyCharged = false; 
		}
	}

	private IEnumerator PlayFullEnergyEffect()
	{
		float effectDuration = 1f;
		float elapsedTime = 0f;

		while (elapsedTime < effectDuration)
		{
			elapsedTime += Time.deltaTime;

			_energyGauge.color = Color.Lerp(new Color(75 / 255f, 0, 130 / 255f), Color.white, Mathf.PingPong(elapsedTime * 2f, 1f));

			yield return null;
		}

		_energyGauge.color = new Color(75 / 255f, 0, 130 / 255f);
	}

	public void ColorPanelEffect(Color color, float alpha) {
		_colorPanelEffectTimer = 0;
		StartCoroutine(ColorPanelEffectCoroutine(color, alpha));
	}

	private IEnumerator ColorPanelEffectCoroutine(Color color, float alpha) {
		float t = 0;
		while(_colorPanel && t < 1) {
			t = _colorPanelEffectTimer / _colorPanelEffectDuration;
			_colorPanel.color = new Color(color.r, color.g, color.b, color.a * (1 - t) * alpha);
			_colorPanelEffectTimer += Time.deltaTime;
			yield return null;
		}
	}

	public void UpdateBullet(int currentBullet)
	{
		_bulletText.text = currentBullet + " / " + PlayerFire.MaxBullet;
	}

	private void SetCrosshairSize(float size)
	{
		_crossHair.sizeDelta = new Vector2(size, size);
	}

	public void CrossHairFire()
	{
		SetCrosshairSize(_fireSize);
		StartCoroutine(ResetCrossHair());
	}

	private IEnumerator ResetCrossHair()
	{
		yield return new WaitForSeconds(0.1f);

		SetCrosshairSize(_defaultSize);
	}

	public void TriggerCoreInteractionUi()
	{
		StartCoroutine(ShowCoreInteractionUi());
	}

	public void ECoreInteractionUi()
	{
		_coreText.SetActive(true);
	}

	public void EDoorInteractionUi()
	{
		_doorText.SetActive(true);
	}

	public void HideInteractionUi()
	{
		_coreText.SetActive(false);
		_doorText.SetActive(false);
	}

	public void EnergyGaugeUi()
	{
		_energyGauge.gameObject.SetActive(true);
	}
	private IEnumerator ShowCoreInteractionUi()
	{
		_warningTriangle.SetActive(true);
		yield return new WaitForSeconds(2f);
		_warningTriangle.SetActive(false);

		_progressBar.value = 0;
		_progressBar.gameObject.SetActive(true);

		float duration = 28f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			_progressBar.value = Mathf.Clamp01(elapsed / duration);
			yield return null;
		}

		_progressBar.gameObject.SetActive(false);
	}

	public IEnumerator ShowStageIntro(int stageIndex)
	{
		if (_stageIntroBackground == null || _stageTexts == null || _stageTexts.Count == 0)
		{
			Debug.LogError("No Stage UI.");
			yield break;
		}

		foreach (var text in _stageTexts)
		{
			if (text != null)
				text.SetActive(false);
		}

		if (stageIndex >= 0 && stageIndex < _stageTexts.Count)
		{
			_stageTexts[stageIndex].SetActive(true);
			_stageIntroBackground.SetActive(true);

			Time.timeScale = 0f;

			bool enterPressed = false;
			while (!enterPressed)
			{
				if (Input.GetKeyDown(KeyCode.T))
				{
					Time.timeScale = 1f;
					enterPressed = true;
				}
				yield return new WaitForSeconds(0f);
			}

			_stageIntroBackground.SetActive(false);
			_stageTexts[stageIndex].SetActive(false);
		}
	}
}
