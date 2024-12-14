using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawner : MonoBehaviour, IEnemyFactory
{
	public GameObject toSpawn;
	[SerializeField] float _delay;

	void Start()
	{
		transform.localScale = Vector3.zero;
	}

	public void SetTimer()
	{
		Invoke("SpawnEnemy", _delay);
	}

	public void SetTimer(float customDelay)
	{
		Invoke("SpawnEnemy", customDelay);
	}

	public void SpawnEnemy()
	{
		GameManager.Instance.RegisterEnemy(Instantiate(toSpawn, transform.position, transform.rotation));
	}
}
