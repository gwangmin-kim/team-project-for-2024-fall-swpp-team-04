using UnityEngine;

/// <summary>
/// Class to manage the Core object. 
/// Activates the light and enables interaction when all enemies are eliminated.
/// On interaction, it enables emission, opens the door, and activates Stage2.
/// </summary>
public class ClearCore : MonoBehaviour, IInteractable
{
	[Header("Core Settings")]
	[SerializeField] private Light _coreLight; // Core light
	[SerializeField] private Renderer _forceFieldRenderer; // Renderer for emission effect
	[SerializeField] private Door_2 _lobbyDoor; // Door object (implements IDoor interface)
	[SerializeField] private GameObject _stageDoor;
	private GameManager _gameManager; // Reference to GameManager
	[SerializeField] private CoreController _coreController;
	private bool _isInteractable = false; // Current interactable state
	private bool _hasInteracted = false;

	[SerializeField] private AudioSource _audioSource;
	[SerializeField] private AudioClip _coreSound;
	[SerializeField] private float _coreSoundMaxVolume = 1f;
	void Start()
	{
		_gameManager = FindObjectOfType<GameManager>();
		if (_gameManager == null)
		{
			Debug.LogError("GameManager is not present in the scene.");
			return;
		}

		InitializeCore();
	}

	/// <summary>
	/// Performs initial setup for the core.
	/// Checks if all enemies are eliminated and updates the state.
	/// </summary>
	private void InitializeCore()
	{
		if (_coreLight != null)
		{
			_coreLight.enabled = false;
		}
		else
		{
			Debug.LogWarning("coreLight is not assigned.");
		}

		if (_forceFieldRenderer != null)
		{
			Material forceFieldMaterial = _forceFieldRenderer.material;
			forceFieldMaterial.DisableKeyword("_EMISSION");
			forceFieldMaterial.SetColor("_EmissionColor", Color.black);
		}
		else
		{
			Debug.LogWarning("forceFieldRenderer is not assigned.");
		}
	}
	public bool IsInteractable()
	{
		return _isInteractable;
	}

	private void FixedUpdate()
	{
		CheckEnemiesCleared();
	}

	/// <summary>
	/// Checks if all enemies are eliminated and updates the state.
	/// </summary>
	private void CheckEnemiesCleared()
	{
		if (_gameManager.IsClear())
		{
			if (!_hasInteracted)
			{
				_isInteractable = true;
			}
			else
			{
				_isInteractable = false;
			}
			if (_coreLight != null)
			{
				_coreLight.enabled = true;
			}
		}
		else
		{

			// Disable interaction
			_isInteractable = false;
			_coreLight.enabled = false;
			// Hide interaction UI
			UIManager.Instance.HideInteractionUi();
		}
	}

	/// <summary>
	/// Called when the player attempts to interact with the core.
	/// Enables emission, opens the door, and activates Stage2.
	/// </summary>
	public void Interactive()
	{
		if (!_isInteractable)
		{
			return;
		}


		if (_forceFieldRenderer != null)
		{
			Material forceFieldMaterial = _forceFieldRenderer.material;
			forceFieldMaterial.EnableKeyword("_EMISSION");
			forceFieldMaterial.SetColor("_EmissionColor", Color.white);;

			if (_audioSource != null && _coreSound != null)
			{
				_audioSource.volume = _coreSoundMaxVolume * GameManager.Instance.GetSFXVolume();
				_audioSource.PlayOneShot(_coreSound);
			}
		}

		if (_stageDoor != null && _stageDoor.TryGetComponent<IDoor>(out IDoor doorComponent))
		{
			doorComponent.Open();
		}

		if (_lobbyDoor != null)
		{
			_lobbyDoor.EnableOpenFromStage();
		}

		_coreController.ResetCoreController();

		// Disable interaction and hide UI
		_hasInteracted = true;
		_isInteractable = false;
		UIManager.Instance.HideInteractionUi();
	}


}