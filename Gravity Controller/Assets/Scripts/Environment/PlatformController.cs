using UnityEngine;

public class PlatformController : MonoBehaviour
{
	[SerializeField] private float _descentSpeed = 5f;
	private float _initialY;
	private float _targetY;
	private bool _isMoving = false;
	private Rigidbody _rb;

	private ParticleSystem[] _dust;
	[SerializeField] private float _unitParticleRate = 100;
	[SerializeField] private float _emissionPower;
	[SerializeField] private float _unitSpeed = 1;
	[Header("Audio")]
	[SerializeField] private float _crackingSfxMaxVolume;
	[SerializeField] private AudioClip _crackingSfx;
	[SerializeField] private AudioSource _breakingAudioSource;
	[SerializeField] private float _breakingSfxMaxVolume;
	[SerializeField] private AudioClip _breakingSfx;
	private AudioSource _audioSource;
	private Vector3 _previousPosition;

	void Start()
	{
		_initialY = transform.position.y;
		_targetY = _initialY - 5f;

		_rb = GetComponent<Rigidbody>();
		if (_rb == null)
		{
			_rb = gameObject.AddComponent<Rigidbody>();
			_rb.isKinematic = true;
		}

		_rb.interpolation = RigidbodyInterpolation.Interpolate;

		_previousPosition = _rb.position;

		_dust = transform.Find("Dust").GetComponentsInChildren<ParticleSystem>();

		_audioSource = GetComponent<AudioSource>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Player") && !_isMoving) // 처음 밟았을 때만 동작
		{
			_isMoving = true;

			if (!_audioSource.isPlaying)
			{
				_audioSource.clip = _crackingSfx;
				_audioSource.volume = _crackingSfxMaxVolume * GameManager.Instance.GetSFXVolume();
				_audioSource.Play();
			}
		}
	}

	void Update()
	{
		if (_isMoving)
		{
			foreach (ParticleSystem dustParticle in _dust)
			{
				dustParticle.Play();
			}
			MovePlatform();
		}
		else
		{
			foreach (ParticleSystem dustParticle in _dust)
			{
				dustParticle.Stop();
			}
		}

		_previousPosition = _rb.position;
	}

	private void MovePlatform()
	{
		float step = _descentSpeed * Time.deltaTime;
		Vector3 targetPosition = new Vector3(transform.position.x, _targetY, transform.position.z);
		Vector3 newPosition = Vector3.MoveTowards(_rb.position, targetPosition, step);
		_rb.MovePosition(newPosition);

		foreach (ParticleSystem dustParticle in _dust)
		{
			var main = dustParticle.main;
			main.startColor = IsEmergency() ? (Color)new Color32(0x60, 0x37, 0x3e, 0xff) : (Color)new Color32(0x67, 0x63, 0x5d, 0xff);

			var mult = main.startSpeedMultiplier;
			var em = dustParticle.emission;
			mult = _unitSpeed * (7 + _targetY - transform.position.y);
			em.rateOverTime = _unitParticleRate * Mathf.Pow((7 + _targetY - transform.position.y), _emissionPower);
		}

		if (newPosition.y <= _targetY)
		{
			_rb.position = targetPosition;
			_isMoving = false;

			var sparkle = transform.parent.Find("SparkleDust");
			sparkle.position = newPosition;
			var main = sparkle.GetComponent<ParticleSystem>().main;
			main.startColor = IsEmergency() ? (Color)new Color32(0x60, 0x37, 0x3e, 0xff) : (Color)new Color32(0x67, 0x63, 0x5d, 0xff);
			sparkle.gameObject.SetActive(true);

			_breakingAudioSource.volume = _breakingSfxMaxVolume * GameManager.Instance.GetSFXVolume();
			_breakingAudioSource.PlayOneShot(_breakingSfx);

			gameObject.SetActive(false);
		}
	}

	private bool IsEmergency()
	{
		return GameObject.Find("Stage_third").transform.Find("FUSION_CORE").GetComponent<CoreInteraction>().IsEmergency;
	}
}
