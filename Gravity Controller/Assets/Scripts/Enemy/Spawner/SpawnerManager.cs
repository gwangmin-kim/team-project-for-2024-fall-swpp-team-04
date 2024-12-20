using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
	[SerializeField] private List<GameObject> _spawnerObjects;
	[SerializeField] private List<float> _spawnTimes;
	[SerializeField] private List<int> _enemyCounts;
	[SerializeField] private List<float> _customDelays;
	private List<IEnemyFactory> _spawners = new List<IEnemyFactory>();

	void Start()
	{
		// 스포너 객체 초기화
		foreach (var obj in _spawnerObjects)
		{
			var spawner = obj.GetComponent<IEnemyFactory>();
			if (spawner != null)
			{
				_spawners.Add(spawner);
			}
		}
		StartCoroutine(SpawnEnemiesCoroutine());
	}
	private IEnumerator SpawnEnemiesCoroutine()
	{
		float previousTime = 0f;
		for (int i = 0; i < _spawnTimes.Count; i++)
		{
			float waitTime = _spawnTimes[i] - previousTime;
			yield return new WaitForSeconds(waitTime);
			previousTime = _spawnTimes[i];

			int spawnerIndex = i % _spawners.Count;
			int enemyCount = _enemyCounts[i % _enemyCounts.Count];
			float delay = _customDelays[i % _customDelays.Count];

			for (int c = 0; c < enemyCount; c++)
			{
				_spawners[spawnerIndex].SpawnEnemy();
				if (c < enemyCount - 1)
					yield return new WaitForSeconds(delay);
			}
		}
	}
}