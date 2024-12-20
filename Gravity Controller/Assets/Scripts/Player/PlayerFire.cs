using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [SerializeField] private GameObject _sparkParticle;

    public const int MaxBullet = 10;
    private const float ShootCooldown = 0.5f;
    private const float ReloadTime = 2f;

    public static int currentBullet;
    public bool _isShootable = true;
    public bool _isReloading = false;

    public static void RecoverBullet() {
        currentBullet = MaxBullet;
        UIManager.Instance.UpdateBullet(MaxBullet);
    }

    public void HandleInput() {
        if(PlayerInput.IsFirePressed) {
            Fire();
        }
        if(PlayerInput.IsReloadPressed) {
            Reload();
        }
    }
    
    // 좌클릭 입력 처리(총)
    private void Fire() {
        if(!_isShootable) {
            return;
        }
        if(currentBullet-- > 0) {
            _isShootable = false;
			PlayerAudio.PlaySfxOnce(PlayerAudio.fireSound, PlayerAudio.FireSoundMaxVolume);
            PlayerMovement.Gun.SendMessage("HandleRecoil", SendMessageOptions.DontRequireReceiver);
			Transform cameraTransform = PlayerCamera.Camera.transform;
            RaycastHit hit;
            if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit)) {
                // 여기에서 맞은 대상의 오브젝트 가져올 수 있음
				Instantiate(_sparkParticle, hit.point, Quaternion.identity);
				var targetAttackReceiver = hit.collider.gameObject.GetComponent<IAttackReceiver>();
				if (targetAttackReceiver != null)
				{
					targetAttackReceiver.OnHit();
                }
            }
            UIManager.Instance.UpdateBullet(currentBullet);
            UIManager.Instance.CrossHairFire();
            StartCoroutine(ReShootable());
        } else {
            Reload();
        }
    }
    // 재장전
    private void Reload() {
		if (_isReloading) {
            return;
        }
        _isReloading = true;
        _isShootable = false;

        PlayerAudio.PlaySfxOnce(PlayerAudio.reloadSound, PlayerAudio.ReloadSoundMaxVolume);
        PlayerMovement.Gun.SendMessage("HideGunOnReload", SendMessageOptions.DontRequireReceiver);
		StartCoroutine(ReShootable());
    }

    // 발포 딜레이, 재장전 딜레이 처리
    private IEnumerator ReShootable() {
        if(_isReloading) {
            yield return new WaitForSeconds(ReloadTime);
            _isReloading = false;
            currentBullet = MaxBullet;
            UIManager.Instance.UpdateBullet(currentBullet);
        } else {
            yield return new WaitForSeconds(ShootCooldown);
        }
        if(!_isReloading)
            _isShootable = true;
    }
}
