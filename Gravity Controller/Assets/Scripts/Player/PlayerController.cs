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
    public static Rigidbody Rigid { get; private set; }

    private PlayerInput _playerInput;
    private PlayerCamera _playerCamera;
    private PlayerJump _playerJump;
    private PlayerFire _playerFire;
    private Gravity _playerGravity;
    private PlayerEnergy _playerEnergy;
    private PlayerInteract _playerInteract;
    private PlayerMovement _playerMovement;

    private void Start() {
        Rigid = GetComponent<Rigidbody>();
        
        _playerInput = GetComponent<PlayerInput>();
        _playerCamera = GetComponent<PlayerCamera>();
        _playerJump = GetComponent<PlayerJump>();
        _playerFire = GetComponent<PlayerFire>();
        _playerGravity = GetComponent<Gravity>();
        _playerEnergy = GetComponent<PlayerEnergy>();
        _playerInteract = GetComponent<PlayerInteract>();
        _playerMovement = GetComponent<PlayerMovement>();

        InitState();
    }

	private void FixedUpdate() {
        GroundChecker.GroundCheck(transform.position);
        _playerGravity.HandlePhysics();
        _playerMovement.SetMoveSpeedGun();
        _playerMovement.MovePlayer();
        _playerMovement.HandleDrag();
    }
    void Update() {
        if(!PlayerHp.isAlive) {
            return;
        }
        _playerInput.GetInput();
        _playerMovement.ControlSpeed();
        _playerCamera.HandleInput();
        _playerJump.HandleInput();
        _playerFire.HandleInput();
        _playerGravity.HandleInput();
        _playerEnergy.UpdateEnergy();
        _playerInteract.CheckInteraction();
        _playerMovement.HandleFootsteps();
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
