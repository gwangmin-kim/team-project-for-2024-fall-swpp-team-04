using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IEnemyFactory
{
	public GameObject toSpawn;
	// Start is called before the first frame update
	void Start()
	{
		transform.localScale = Vector3.zero;
	}

	public void SpawnEnemy()
	{
		GameManager.Instance.RegisterEnemy(Instantiate(toSpawn, transform.position, transform.rotation));
	}
}
