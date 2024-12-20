using UnityEngine;

public class GroundChecker
{
    public static bool IsGrounded = true;
    private const float GroundCheckOffset = 0.1f;
    private const float GroundCheckDistance = 0.3f;

	public static void GroundCheck(Vector3 position)
	{
		IsGrounded = Physics.Raycast(position + Vector3.up * GroundCheckOffset, Vector3.down, GroundCheckDistance);
	}
}
