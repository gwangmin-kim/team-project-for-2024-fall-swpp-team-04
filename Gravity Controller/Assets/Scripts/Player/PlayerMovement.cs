using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public static GameObject Gun { get; private set; }

    private float _moveForce = 10000f; // force moving player
    private float _maxSpeed = 20f; // limit max speed of player
    private float _groundDrag = 7f; // prevent slippery
    private float _airMultiplier = 0.3f; // lower force when player is not grounded
    
    public float MoveSpeedGun { get; private set; }

    private const float FootStepInterval = 0.3f;
	private float _footstepTimer = 0f;

    private void Start() {
        Gun = transform.GetChild(0).GetChild(0).gameObject;
    }
    public void SetMoveSpeedGun() {
        Vector3 flatVelocity = new Vector3(PlayerController.Rigid.velocity.x, 0f, PlayerController.Rigid.velocity.z);
		MoveSpeedGun = flatVelocity.magnitude;
    }

    public void ControlSpeed()
    {
        Vector3 flatVel = new Vector3(PlayerController.Rigid.velocity.x, 0f, PlayerController.Rigid.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > _maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _maxSpeed;
            PlayerController.Rigid.velocity = new Vector3(limitedVel.x, PlayerController.Rigid.velocity.y, limitedVel.z);
        }
    }

    public void MovePlayer()
    {
        // calculate movement direction
        Vector3 moveDirection = transform.forward * PlayerInput.VerticalInput 
                                + transform.right * PlayerInput.HorizontalInput;

        // on ground
        if(GroundChecker.IsGrounded) {
            PlayerController.Rigid.AddForce(moveDirection.normalized * _moveForce, ForceMode.Force);
        } else {
            PlayerController.Rigid.AddForce(moveDirection.normalized * _moveForce * _airMultiplier, ForceMode.Force);
        }
    }
    
    public void HandleDrag() {
        if(GroundChecker.IsGrounded) {
            PlayerController.Rigid.drag = _groundDrag;
        } else {
            PlayerController.Rigid.drag = 0;
        }
    }

	public void HandleFootsteps()
	{
		Vector3 flatVel = new Vector3(PlayerController.Rigid.velocity.x, 0f, PlayerController.Rigid.velocity.z);
		bool hasInput = Mathf.Abs(PlayerInput.HorizontalInput) > 0.1f 
                        || Mathf.Abs(PlayerInput.VerticalInput) > 0.1f;
		bool isMoving = flatVel.magnitude > 0.1f;

		bool currentCondition = GroundChecker.IsGrounded && isMoving && hasInput;

		if (currentCondition)
		{
			_footstepTimer += Time.deltaTime;
			if (_footstepTimer >= FootStepInterval) {
                PlayerAudio.PlaySfxOnce(PlayerAudio.footstepSound, PlayerAudio.FootstepMaxVolume);
				_footstepTimer = 0f;
			}
		}
	}
}