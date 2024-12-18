using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHp : MonoBehaviour
{
    public const int MaxHP = 5;
    private static Color _onHitPanelColor = Color.red;
    private const float OnHitPanelAlpha = 0.3f;
    public static int CurrentHP { get; private set; }
    public static bool isAlive = true;
    
    public static void RecoverHp() {
        isAlive = true;
        CurrentHP = MaxHP;
        UIManager.Instance.UpdateHP(MaxHP);
    }

    // 피격 시 호출(외부에서)
	public void OnHit() {
        if(!isAlive) {
            return;
        }
        CurrentHP--;
        PlayerAudio.PlaySfxOnce(PlayerAudio.hitSound, PlayerAudio.HitSoundMaxVolume);
        UIManager.Instance.UpdateHP(CurrentHP);
        UIManager.Instance.ColorPanelEffect(_onHitPanelColor, OnHitPanelAlpha);
        if(CurrentHP <= 0) {
            StartCoroutine(OnDie());
        }
    }

    private IEnumerator OnDie() {
        isAlive = false;
        yield return null;
        SceneManager.LoadScene("GameOverScene");
    }
}
