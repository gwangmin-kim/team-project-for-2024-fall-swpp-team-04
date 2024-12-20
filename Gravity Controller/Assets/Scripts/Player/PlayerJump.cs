using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private const float JumpForce = 1250f;
    private const float JumpMultiplier = 1.3f; // adjust the jump force when the gravity low skill is active

    public void HandleInput() {
        GroundChecker.GroundCheck(transform.position);
        if(GroundChecker.IsGrounded && PlayerInput.IsJumpPressed) {
            Jump();
        }
    }

    private void Jump() {
        Rigidbody rigid = PlayerController.Rigid;
        GroundChecker.IsGrounded = false;
        float jumpMultiplier = Gravity.IsGravityLow ? JumpMultiplier : 1f;
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(transform.up * JumpForce * jumpMultiplier, ForceMode.Impulse);
    }
}
