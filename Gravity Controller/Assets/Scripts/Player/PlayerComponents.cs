using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    public static Rigidbody Rigid { get; private set; }
    public static GameObject Gun { get; private set; }
    public static GameObject SparkParticle { get; private set; }
    public static GameObject PlayerCamera { get; private set; }

    [SerializeField] private GameObject _sparkParticle;

    private void Start() {
        Rigid = GetComponent<Rigidbody>();
        Gun = transform.GetChild(0).GetChild(0).gameObject;
        // PlayerCamera = transform.GetChild(0).gameObject;
        PlayerCamera = GameObject.Find("PlayerCamera");
        SparkParticle = _sparkParticle;
    }
}
