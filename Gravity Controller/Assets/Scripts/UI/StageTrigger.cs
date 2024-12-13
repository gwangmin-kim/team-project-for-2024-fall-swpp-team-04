using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTrigger : MonoBehaviour
{
	[SerializeField] private int _stageNumber;
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (BGMManager.Instance.CurrentStage != _stageNumber)
			{
				BGMManager.Instance.SetStageBGM(_stageNumber);
			}
		}
	}
}
