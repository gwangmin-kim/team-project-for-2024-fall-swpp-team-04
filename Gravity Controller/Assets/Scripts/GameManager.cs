using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	private List<GameObject> _activeEnemies = new List<GameObject>();
	private int _enemyNumber = 0;
	private float _sfxVolume = 1f;
	private float _bgmVolume = 1f;

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

		Cursor.visible = false;
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
		// 코루틴에서 리스트 처리 중 리스트에 변경이 가해질 수 있음
		// 이로 인한 문제를 막기 위해 리스트의 요소를 모두 옮겨담아 새로운 리스트를 만들어 전달한다.
		List<GameObject> copiedList = new List<GameObject>();
		foreach (GameObject obj in _activeEnemies) {
			copiedList.Add(obj);
		}
		return copiedList;
	}

	public float GetSFXVolume()
	{
		return _sfxVolume;
	}

	public void SetSFXVolume(float vol)
	{
		_sfxVolume = vol;
	}

	public float GetBGMVolume()
	{
		return _bgmVolume;
	}

	public void SetBGMVolume(float vol)
	{
		_bgmVolume = vol;
	}

	public bool IsClear()
	{
		return _enemyNumber==0;
	}
}

