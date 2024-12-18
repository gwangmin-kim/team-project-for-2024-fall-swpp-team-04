using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static int Stage { get; private set; } = 1;
    private PlayerInput _playerInput;
    private PlayerCamera _playerCamera;
    private PlayerJump _playerJump;
    private PlayerFire _playerFire;
    private Gravity _playerGravity;
    private PlayerEnergy _playerEnergy;
    private PlayerInteract _playerInteract;

    private void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _playerCamera = GetComponent<PlayerCamera>();
        _playerJump = GetComponent<PlayerJump>();
        _playerFire = GetComponent<PlayerFire>();
        _playerGravity = GetComponent<Gravity>();
        _playerEnergy = GetComponent<PlayerEnergy>();
        _playerInteract = GetComponent<PlayerInteract>();

        InitState();
    }

	private void FixedUpdate() {
        _playerGravity.HandlePhysics();
        GroundChecker.GroundCheck(transform.position);
    }
    void Update() {
        if(!PlayerHp.isAlive) {
            return;
        }
        _playerInput.GetInput();
        _playerCamera.HandleInput();
        _playerJump.HandleInput();
        _playerFire.HandleInput();
        _playerGravity.HandleInput();

        _playerEnergy.UpdateEnergy();
        _playerInteract.CheckInteraction();
    }


    public void UpdateStage(int stage) {
	    Stage = stage;
    }

    public void InitState()
    {
	    PlayerHp.RecoverHp();
	    PlayerFire.RecoverBullet();
        PlayerEnergy.RecoverEnergy();
	}
}
