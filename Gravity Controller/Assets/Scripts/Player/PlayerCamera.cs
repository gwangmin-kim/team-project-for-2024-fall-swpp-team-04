using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static GameObject Camera { get; private set; }
    // mouse sensetivity
    private static float _sensitivityMultiplier = 0.5f;
    private const float SensetivityX = 250f;
    private const float SensetivityY = 250f;
    private const float SensitivityMultiplierMin = 0.1f;
	private const float SensitivityMultiplierMax = 1.9f;

    // determine camera rotation
    private float _accumRotationX = 0;
    private float _accumRotationY = 0;
    private const float RotationLimitY = 80;

    private void Start() {
        Camera = transform.GetChild(0).gameObject;
    }

    public static void SetSensitivityMultiplier(int percentage) {
		_sensitivityMultiplier = SensitivityMultiplierMin + (SensitivityMultiplierMax - SensitivityMultiplierMin) * ((float)percentage) / 100;
	}

    public void HandleInput() {
        // player rotation
        _accumRotationX += Time.deltaTime * PlayerInput.MouseInputX * SensetivityX * _sensitivityMultiplier;
        // camera rotation
        _accumRotationY += Time.deltaTime * PlayerInput.MouseInputY * SensetivityY * _sensitivityMultiplier;
        _accumRotationY = Mathf.Clamp(_accumRotationY, -RotationLimitY, RotationLimitY);

        // reset view
		if (PlayerInput.IsViewResetPressed) {
			_accumRotationY = 0f;
		}

        Rotate();
    }

    public void Rotate() {
        transform.eulerAngles = new Vector3(0, _accumRotationX, 0);
        Camera.transform.eulerAngles = new Vector3(-_accumRotationY, _accumRotationX, 0);
    }
}
