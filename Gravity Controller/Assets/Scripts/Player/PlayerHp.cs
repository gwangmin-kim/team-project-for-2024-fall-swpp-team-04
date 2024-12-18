using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHp : MonoBehaviour
{
    public const int MaxHP = 5;
    private static Color OnHitPanelColor = Color.red;
    private const float OnHitPanelAlpha = 0.3f;
    public static int currentHP { get; private set; }
    public static bool isAlive = true;
    
    public static void RecoverHp() {
        currentHP = MaxHP;
        UIManager.Instance.UpdateHP(MaxHP);
    }

    // 피격 시 호출(외부에서)
	public static void OnHit() {
        if(!isAlive) {
            return;
        }
        PlayerAudio.PlaySfxOnce(PlayerAudio.hitSound, PlayerAudio.HitSoundMaxVolume);
        UIManager.Instance.UpdateHP(currentHP);
        UIManager.Instance.ColorPanelEffect(OnHitPanelColor, OnHitPanelAlpha);
        if(--currentHP <= 0) {
            OnDie();
        }
    }

    private static void OnDie() {
        isAlive = false;
        SceneManager.LoadScene("GameOverScene");
    }
}
