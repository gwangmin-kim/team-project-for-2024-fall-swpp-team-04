using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    private PlayerEnergy _energy;

    // local gravity control(right click)
    public const float LocalGravityCooldown = 0.5f;
    public const float LocalGravityForce = 2f;
    public const float LocalEnergyCost = 100f;
    private bool _isTargetable = true;

    // global gravity control(mouse wheel)
    public const float WheelInputThreshold = 0.05f;
    public const float GravityForceHigh = 2f;
    public const float GravityForceLow = 0.05f;
    public const float GlobalLowEnergyCost = 60f;
    public const float GlobalHighEnergyCost = 300f;
    public static bool IsGravityLow { get; private set; } = false;

    public void HandleInput() {
        int stage = PlayerController.Stage;
        if(stage >= 2 && PlayerInput.IsTargetGravityPressed) {
            TargetGravity();
        }
        if(stage >= 3) {
            float wheelInput = PlayerInput.MouseInputWheel;
            if(wheelInput > WheelInputThreshold) {
                GlobalGravityLowOn();
            } else if(wheelInput < -WheelInputThreshold) {
                if(IsGravityLow) {
                    GlobalGravityLowOff();
                } else if(stage >= 4) {
                    List<GameObject> activeEnemies = GameManager.Instance.GetActiveEnemies();
                    GlobalGravityHigh(activeEnemies);
                }
            }
        }
    }

    public void HandlePhysics() {
        // 낮아진 전체 중력에 대한 처리, 플레이어에게만 영향을 줌
        if(IsGravityLow) {
            PlayerController.Rigid.AddForce(Physics.gravity * (GravityForceLow - 1f), ForceMode.Impulse);
        }
    }

    // 우클릭 입력 처리(대상 지정 중력 강화)
    public void TargetGravity() {
        if(!_isTargetable || PlayerEnergy.currentEnergy < LocalEnergyCost) {
            return;
        }
        Transform cameraTransform = PlayerCamera.Camera.transform;
        RaycastHit hit;
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit)) {
			ISkillReceiver targetSkillReceiver =  hit.collider.gameObject.GetComponent<ISkillReceiver>();

			if (targetSkillReceiver != null) {
                _isTargetable = false;
                StartCoroutine(ReTargetable());
                PlayerEnergy.currentEnergy -= LocalEnergyCost;
                PlayerAudio.PlaySfxOnce(PlayerAudio.localGravitySound, PlayerAudio.LocalGravitySoundMaxVolume);

				// 여기서 피격된 대상의 오브젝트를 불러올 수 있음
				targetSkillReceiver.ReceiveSkill();
				PlayerMovement.Gun.SendMessage("HideGunOnSkill", SendMessageOptions.DontRequireReceiver);
			}
        }
    }

    // 대상 지정 중력 강화 딜레이
    private IEnumerator ReTargetable() {
        yield return new WaitForSeconds(LocalGravityCooldown);
        if(!_isTargetable) {
            _isTargetable = true;
        }
    }

    // 전체 중력 약화
    public void GlobalGravityLowOn() {
        IsGravityLow = true;
        PlayerAudio.PlaySfx(PlayerAudio.globalGravityLowSound, PlayerAudio.GlobalGravityLowSoundMaxVolume);
    }
    public void GlobalGravityLowOff() {
        IsGravityLow = false;
        PlayerAudio.StopSfx();
    }

    // 전체 중력 강화
    // 여러 오브젝트에 대해 루프를 돌며 작업을 수행해야 하므로 프레임 드랍을 막기 위해 코루틴으로 실행
    public void GlobalGravityHigh(List<GameObject> gameObjects) {
        if(PlayerEnergy.currentEnergy < GlobalHighEnergyCost) {
            return;
        }
        PlayerEnergy.currentEnergy -= GlobalHighEnergyCost;
        PlayerAudio.PlaySfxOnce(PlayerAudio.globalGravityHighSound, PlayerAudio.GlobalGravityHighSoundMaxVolume);
		PlayerMovement.Gun.SendMessage("HideGunOnSkill", SendMessageOptions.DontRequireReceiver);
		
        StartCoroutine(SendGravitySkill(gameObjects));
    }

    private IEnumerator SendGravitySkill(List<GameObject> gameObjects) {
        foreach (GameObject obj in gameObjects) {
            ISkillReceiver targetSkillReceiver;
            if(obj && obj.activeSelf && obj.TryGetComponent<ISkillReceiver>(out targetSkillReceiver)) {
                targetSkillReceiver.ReceiveSkill();
            }
            yield return null;
        }
    }

}
