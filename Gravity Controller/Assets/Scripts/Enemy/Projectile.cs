using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	// issue: Trigger로 바꾸는 게 나을 듯
	// private void OnCollisionEnter(Collision collision) {
	// 	if(collision.gameObject.CompareTag("Player")) {
	// 		PlayerController playerController = collision.collider.GetComponent<PlayerController>();
	// 		if(playerController) {
	// 			playerController.OnHit(); 
	// 			Debug.Log("HitPlayer");
	// 			Destroy(gameObject);
	// 		}
	// 	}
	// }

	// issue: instantiate 즉시 투사체를 소환한 드론과 충돌이 감지되는 듯함
	// 우선 태그가 enemy이면 무시하도록 했는데, 만약 드론의 공격에 다른 적이 맞았을 때 공격받는 효과를 만들고 싶다면 수정이 필요
	private void OnTriggerEnter(Collider other) {
		if(ClimbingTagCheck(other.transform, "Enemy")|| ClimbingTagCheck(other.transform, "Turret") || other.gameObject.CompareTag("Projectile")) {
			return;
		}
		if(other.CompareTag("Player")) {
			other.GetComponent<PlayerHp>().OnHit();
		}
		Destroy(gameObject);
	}

	private bool ClimbingTagCheck(Transform tr, string tag)
	{
		if (tr.gameObject.CompareTag(tag))
		{
			return true;
		}
		while (tr.parent != null)
		{
			tr = tr.parent;
			if (tr.gameObject.CompareTag(tag))
			{
				return true;
			}
		}
		return false;
	}
}
