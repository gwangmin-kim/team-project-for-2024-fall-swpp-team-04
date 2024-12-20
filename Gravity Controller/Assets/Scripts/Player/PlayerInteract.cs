using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private const float InteractiveRange = 5f;
    
    public void CheckInteraction() {
        Transform cameraTransform = PlayerCamera.Camera.transform;
		RaycastHit hit;
		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, InteractiveRange)) {
			GameObject targetObject = hit.collider.gameObject;
			if (targetObject.CompareTag("Lever")
                && targetObject.TryGetComponent<DoorLever>(out DoorLever lever)) {
                if (!lever._isInteractable) {
                    UIManager.Instance.HideInteractionUi();
                } else {
                    UIManager.Instance.EDoorInteractionUi();
                }
            } else if (targetObject.CompareTag("Core")
                       && targetObject.TryGetComponent<IInteractable>(out IInteractable coreInteraction)) {
                if (!coreInteraction.IsInteractable()) {
                    UIManager.Instance.HideInteractionUi();
                    return;
                }
                UIManager.Instance.ECoreInteractionUi();
			}
            
            if (targetObject.TryGetComponent<IInteractable>(out IInteractable interactable)
                && PlayerInput.IsInteractPressed) {
				interactable.Interactive();
				UIManager.Instance.HideInteractionUi();
			}
		} else {
			UIManager.Instance.HideInteractionUi();
		}
	}
}
