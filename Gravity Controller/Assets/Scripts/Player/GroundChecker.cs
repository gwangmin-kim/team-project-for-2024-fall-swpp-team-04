using UnityEngine;

public class GroundChecker
{
    public static bool IsGrounded = true;
    private const float GroundCheckOffset = 0.1f;
    private const float GroundCheckDistance = 0.3f;

	public static void GroundCheck(Vector3 position)
	{
		Vector3 rayOrigin = position + Vector3.up * GroundCheckOffset;
		RaycastHit hit;
		bool hitSomething = Physics.Raycast(rayOrigin, Vector3.down, out hit, GroundCheckDistance);

		if (hitSomething && !hit.collider.CompareTag("platform"))
		{
			IsGrounded = true;
		}
		else
		{
			IsGrounded = false;
		}
	}
}
