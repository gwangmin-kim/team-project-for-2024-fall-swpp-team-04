using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    private const float MaxEnergy = 300f;
    private const float EnergyRecoveryPerSec = 30f;
    public static float currentEnergy;
    private Gravity _gravity;
    

    private void Start() {
        _gravity = GetComponent<Gravity>();
    }

    public static void RecoverEnergy() {
        currentEnergy = MaxEnergy;
    }

    // 중력 능력을 사용 중이지 않을 때, 매 초마다 EnergyRecoverPerSec만큼 에너지를 회복한다.
    public void UpdateEnergy() {
        if(Gravity.IsGravityLow) {
            currentEnergy -= Gravity.GlobalLowEnergyCost * Time.deltaTime;
            if(currentEnergy < 0) {
                currentEnergy = 0;
                _gravity.GlobalGravityLowOff();
			}
        } else {
            currentEnergy += EnergyRecoveryPerSec * Time.deltaTime;
            if(currentEnergy > MaxEnergy) {
                currentEnergy = MaxEnergy;
            }
        }
        UIManager.Instance.UpdateEnergy();
    }

    public static float GetEnergyRatio() {
        return currentEnergy / MaxEnergy;
    }
}
