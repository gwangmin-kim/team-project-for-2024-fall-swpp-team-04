using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	private List<GameObject> _activeEnemies = new List<GameObject>();
	private int _enemyNumber = 0;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void RegisterEnemy(GameObject enemy)
	{
		if (!_activeEnemies.Contains(enemy))
		{
			_activeEnemies.Add(enemy);
		}

		if (!enemy.CompareTag("Turret")) _enemyNumber++;
	}

	public void UnregisterEnemy(GameObject enemy)
	{
		if (_activeEnemies.Contains(enemy))
		{
			_activeEnemies.Remove(enemy);
		}

		if (!enemy.CompareTag("Turret")) _enemyNumber--;
	}

	public List<GameObject> GetActiveEnemies()
	{
		return _activeEnemies;
	}

	public float GetSFXVolume()
	{
		return 1f;
	}

	public float GetBGMVolume()
	{
		return 1f;
	}

	public bool IsClear()
	{
		return _enemyNumber==0;
	}
}

