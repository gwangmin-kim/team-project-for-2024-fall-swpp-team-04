using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private float _moveForce = 10000f; // force moving player
    private float _maxSpeed = 20f; // limit max speed of player
    private float _groundDrag = 7f; // prevent slippery
    private float _airMultiplier = 0.3f; // lower force when player is not grounded
    
    public float _moveSpeedGun;
    public float _maxSpeedGun;

	[Header("Footstep Audio Settings")]
	[SerializeField] private float _footstepInterval = 0.5f;

	private float _footstepTimer = 0f;

	void Start() {
        _maxSpeedGun = _maxSpeed;
    }

    private void FixedUpdate() {
		Vector3 flatVelocity = new Vector3(PlayerComponents.Rigid.velocity.x, 0f, PlayerComponents.Rigid.velocity.z);
		_moveSpeedGun = flatVelocity.magnitude;

        GroundChecker.GroundCheck(transform.position);
        MovePlayer();
        HandleDrag();
	}

    void Update() {
        ControlSpeed();
		HandleFootsteps();
	}

    private void ControlSpeed()
    {
        Vector3 flatVel = new Vector3(PlayerComponents.Rigid.velocity.x, 0f, PlayerComponents.Rigid.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > _maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _maxSpeed;
            PlayerComponents.Rigid.velocity = new Vector3(limitedVel.x, PlayerComponents.Rigid.velocity.y, limitedVel.z);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        Vector3 moveDirection = transform.forward * PlayerInput.VerticalInput 
                                + transform.right * PlayerInput.HorizontalInput;

        // on ground
        if(GroundChecker.IsGrounded) {
            PlayerComponents.Rigid.AddForce(moveDirection.normalized * _moveForce, ForceMode.Force);
        } else {
            PlayerComponents.Rigid.AddForce(moveDirection.normalized * _moveForce * _airMultiplier, ForceMode.Force);
        }
    }
    
    private void HandleDrag() {
        if(GroundChecker.IsGrounded) {
            PlayerComponents.Rigid.drag = _groundDrag;
        } else {
            PlayerComponents.Rigid.drag = 0;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 5f);
    }

	private void HandleFootsteps()
	{
		Vector3 flatVel = new Vector3(PlayerComponents.Rigid.velocity.x, 0f, PlayerComponents.Rigid.velocity.z);
		bool hasInput = Mathf.Abs(PlayerInput.HorizontalInput) > 0.1f 
                        || Mathf.Abs(PlayerInput.VerticalInput) > 0.1f;
		bool isMoving = flatVel.magnitude > 0.1f;

		bool currentCondition = GroundChecker.IsGrounded && isMoving && hasInput;

		if (currentCondition)
		{
			_footstepTimer += Time.deltaTime;
			if (_footstepTimer >= _footstepInterval) {
                PlayerAudio.PlaySfxOnce(PlayerAudio.footstepSound, PlayerAudio.FootstepMaxVolume);
				_footstepTimer = 0f;
			}
		}
	}
}