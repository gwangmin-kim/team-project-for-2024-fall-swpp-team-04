using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : MonoBehaviour, IEnemy, IAttackReceiver
{
	private Animator _animator;
	[Header("Target")]
	private GameObject _player;
	[SerializeField] private float _heightOffset;
	[SerializeField] private float _playerHeightOffset;

	[Header("Wander")]
	[SerializeField] private float _wanderSpeed;
	[SerializeField] private float _wanderRange;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private float _changeDirectionInterval;
	[SerializeField] private float _obstacleDetectionRange;
	[SerializeField] private float _changeDirectionIntervalMin;
	[SerializeField] private float _changeDirectionIntervalMax;
	private Vector3 _spawnPoint;
	private Vector3 _currentDirection;
	private float _timer = 0f;

	[Header("Chase")]
	[SerializeField] private float _chaseSpeed;
	[SerializeField] private float _chaseRange;
	[SerializeField] private float _attackRange;
	[SerializeField] private float _attackHitRange;
	private bool _isAttacking = false;

	[SerializeField] private float _sightRange;
	[SerializeField] private float _awarenessCoolDown;
	private float _awarenessCoolDownTimer;
	private Vector3 _lastSeenPosition;
	[SerializeField] private float _frontOffset;

	[SerializeField] private float _attackAnimationLength = 1.25f;
	private bool _attackSuccess = false;

	[SerializeField] private int _maxHp;
	private int _hp;

	[Header("Death")]
	[SerializeField] public float _beforeDeathTime; //length of 'Die' animation
	[SerializeField] public float _dissolveSpeed;
	[SerializeField] public float _dissolveTime;
	private bool _isDead = false;

	[Header("Audio")]
	[SerializeField] private AudioSource _audioSource;
	[SerializeField] private AudioClip _walkingSound;
	[SerializeField] private AudioClip _followingSound;
	[SerializeField] private AudioClip _attackingSound;

	[SerializeField] private float _walkingSoundMaxVolume = 1f;
	[SerializeField] private float _followingSoundMaxVolume = 1f;
	[SerializeField] private float _attackingSoundMaxVolume = 1f;

	public EnemyState State { get; private set; }

	void Awake()
	{
		_hp = _maxHp;

		State = EnemyState.Idle;
	}

	void Start()
	{
		_player = GameObject.Find("Player");

		_animator = transform.GetChild(0).GetComponent<Animator>();
		_spawnPoint = transform.position;

		_audioSource.clip = _walkingSound;
		_audioSource.loop = true;
		_audioSource.volume = _walkingSoundMaxVolume * GameManager.Instance.GetSFXVolume();
		_audioSource.Play();

		SetRandomDirection();
		BeforeWander();
	}

	void FixedUpdate()
	{
		if (_isDead) return;

		var relativePosition = _player.transform.position + _playerHeightOffset * Vector3.up - transform.position - _heightOffset * Vector3.up;
		float distanceHorizontal = new Vector3(relativePosition.x, 0, relativePosition.z).magnitude;
		float distanceVertical = transform.position.y - _player.transform.position.y;
		RaycastHit hit;
		bool playerInSight = false;

		_awarenessCoolDownTimer -= Time.fixedDeltaTime;

		// Raycast to check if player is in sight
		Vector3 rayOrigin = transform.position + _heightOffset * Vector3.up + _frontOffset * (transform.forward);
		Vector3 rayDirection = new Vector3(relativePosition.x, 0, relativePosition.z).normalized;
		if (Physics.Raycast(rayOrigin, rayDirection, out hit, _sightRange))
		{
			playerInSight = hit.collider.gameObject.CompareTag("Player");
			if (playerInSight)
			{
				_awarenessCoolDownTimer = _awarenessCoolDown;
				_lastSeenPosition = _player.transform.position;
			}
			Debug.Log("Hit:" + hit.collider.name);
		}

		switch (State)
		{
			case EnemyState.Idle:
				if (playerInSight && distanceHorizontal <= _chaseRange)
				{
					// Idle -> Aware
					State = EnemyState.Aware;
					_animator.SetBool("IsRunning", true);
					_animator.SetBool("IsWalking", false);
					Chase();
				}
				else
				{
					Wander();
				}
				break;

			case EnemyState.Aware:
				if (!playerInSight || distanceHorizontal > _chaseRange)
				{
					if (_awarenessCoolDownTimer > 0)
					{
						// Aware -> Follow
						State = EnemyState.Follow;
						_animator.SetBool("IsRunning", true);
						_animator.SetBool("IsWalking", false);
						_audioSource.volume = _followingSoundMaxVolume * GameManager.Instance.GetSFXVolume();
						_audioSource.PlayOneShot(_followingSound);
						Follow();
					}
					else
					{
						// Aware -> Idle
						State = EnemyState.Idle;
						_animator.SetBool("IsRunning", false);
						BeforeWander();
						Wander();
					}
				}
				else
				{
					Chase();
				}
				break;

			case EnemyState.Follow:

				_animator.SetBool("IsRunning", true);
				Follow();
				break;
		}

		Debug.Log(State);
	}

	private void BeforeWander()
	{
		_animator.SetBool("IsWalking", true);
		SetRandomInterval();
		_timer = 0f;
	}

	private void Wander()
	{
		_timer += Time.deltaTime;
		RaycastHit hit;

		// Raycast in the current direction to detect obstacles
		Vector3 rayOrigin = transform.position + _heightOffset * Vector3.up + _frontOffset * transform.forward;
		if (Physics.Raycast(rayOrigin, _currentDirection, out hit, _obstacleDetectionRange))
		{
			// Detected an obstacle while moving
			Debug.Log("Detected an obstacle: " + hit.collider.name);
			SetRandomDirection();
			_timer = 0f;
		}
		else if (_timer > _changeDirectionInterval)
		{
			// Time to change direction
			if (Vector3.Distance(_spawnPoint, transform.position) > _wanderRange)
			{
				// Too far from spawn point, move back towards it
				_currentDirection = new Vector3(_spawnPoint.x - transform.position.x, 0, _spawnPoint.z - transform.position.z).normalized;
			}
			else
			{
				// Choose a new random direction
				SetRandomDirection();
			}
			SetRandomInterval();
			_timer = 0f;
		}

		// Rotate smoothly towards the current direction
		Quaternion targetRotationHorizontal = Quaternion.LookRotation(new Vector3(_currentDirection.x, 0, _currentDirection.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationHorizontal, Time.fixedDeltaTime * _rotationSpeed);

		// Move forward
		transform.Translate(Vector3.forward * Time.fixedDeltaTime * _wanderSpeed);
	}

	private void SetRandomInterval()
	{
		_changeDirectionInterval = Random.Range(_changeDirectionIntervalMin, _changeDirectionIntervalMax);
	}

	private void SetRandomDirection()
	{
		float angle = Random.Range(0, 360);
		_currentDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
	}

	// Modified Chase method to ensure consistent behavior
	private void Chase()
	{
		if (_isAttacking)
		{
			return;
		}

		float distanceToPlayer = new Vector3(transform.position.x - _player.transform.position.x, 0, transform.position.z - _player.transform.position.z).magnitude;

		if (distanceToPlayer < _attackRange)
		{
			Attack();
			return;
		}

		Vector3 direction = new Vector3(_player.transform.position.x - transform.position.x, 0, _player.transform.position.z - transform.position.z).normalized;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.fixedDeltaTime * _rotationSpeed);
		transform.Translate(Vector3.forward * _chaseSpeed * Time.deltaTime);
		_spawnPoint = transform.position;
	}

	private void Attack()
	{
		if (_isAttacking)
		{
			return;
		}

		_isAttacking = true;
		_animator.SetBool("Attack", true);

		StartCoroutine(ResetAttack());
	}

	IEnumerator ResetAttack()
	{
		yield return new WaitForSeconds(_attackAnimationLength);

		_isAttacking = false;
		_animator.SetBool("Attack", false);
	}

	// Modified AttackHitCheck to remove player controller validation
	public void AttackHitCheck()
	{
		_audioSource.volume = _attackingSoundMaxVolume * GameManager.Instance.GetSFXVolume();
		_audioSource.PlayOneShot(_attackingSound);
		if (_attackSuccess)
		{
			Debug.Log("Attack Successful");
			_player.GetComponent<PlayerHp>().OnHit();
		}
		else
		{
			Debug.Log("Attack Fail");
		}
	}

	public void SetAttackSuccess(bool success)
	{
		_attackSuccess = success;
	}

	private void Follow()
	{
		if (_isAttacking)
		{
			return;
		}

		Vector3 directionToLastSeen = _lastSeenPosition - transform.position;
		Vector3 directionToLastSeenHorizontal = new Vector3(directionToLastSeen.x, 0, directionToLastSeen.z).normalized;

		RaycastHit hit;
		Vector3 rayOrigin = transform.position + _heightOffset * Vector3.up + _frontOffset * transform.forward;

		if (Physics.Raycast(rayOrigin, directionToLastSeenHorizontal, out hit, _obstacleDetectionRange))
		{
			if (!hit.collider.gameObject.CompareTag("Player"))
			{
				Debug.Log("Obstacle detected while following: " + hit.collider.name);

				directionToLastSeenHorizontal = Quaternion.Euler(0, Random.Range(30, 60) * (Random.value > 0.5f ? 1 : -1), 0) * directionToLastSeenHorizontal;
			}
		}

		Quaternion targetRotationHorizontal = Quaternion.LookRotation(directionToLastSeenHorizontal);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationHorizontal, Time.fixedDeltaTime * _rotationSpeed);

		transform.Translate(directionToLastSeenHorizontal * _chaseSpeed * Time.fixedDeltaTime, Space.World);

		float distanceToPlayer = new Vector3(transform.position.x - _player.transform.position.x, 0, transform.position.z - _player.transform.position.z).magnitude;

		if (distanceToPlayer < _attackRange)
		{
			Attack();
		}
	}


	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position + _heightOffset * Vector3.up + _frontOffset * transform.forward, _currentDirection * _obstacleDetectionRange);
		Gizmos.DrawWireSphere(transform.position, _attackRange);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(_spawnPoint, _wanderRange);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, _chaseRange);
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(transform.position + _heightOffset * Vector3.up + _frontOffset * transform.forward, transform.forward * _obstacleDetectionRange);
	}

	public void OnHit(int damage)
	{
		_hp -= damage;
		if (_hp <= 0)
		{
			OnDeath();
		}
		// hit effect goes here: particle, knockback, etc.
	}

	public void OnDeath()
	{
		if (_isDead)
		{
			return;
		}
		_isDead = true;
		var childrenColliders = gameObject.GetComponentsInChildren<Collider>();
		foreach (Collider collider in childrenColliders)
		{
			collider.enabled = false;
		}
		_animator.SetBool("Die", true);
		// death animation goes here; must wait till the animation to be finished before destroying
		GameManager.Instance.UnregisterEnemy(gameObject);
		StartCoroutine("Die");
	}

	private IEnumerator Die()
	{
		yield return new WaitForSeconds(_beforeDeathTime);

		foreach (var rend in gameObject.GetComponentsInChildren<Renderer>())
		{
			rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}

		float timer = 0;
		while (timer < _dissolveTime)
		{
			timer += Time.deltaTime;
			foreach (var rend in gameObject.GetComponentsInChildren<Renderer>())
			{
				foreach (var mat in rend.materials)
				{
					mat.SetFloat("_DissolveProgress", timer * _dissolveSpeed);
				}
			};
			yield return null;
		}
		Destroy(gameObject);
	}
}