using UnityEngine;
using UnityEngine.XR;

public class CanvasVRFollower : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;  // Référence à la caméra VR
    [SerializeField] private float distanceFromCamera = 2f;  // Distance du Canvas par rapport à la caméra
    
    [Header("Offset Parameters")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;  // Décalage de position
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;  // Décalage de rotation

    private void Start()
    {
        // Si aucune caméra n'est assignée, on essaie de trouver la caméra principale
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogError("Aucune caméra trouvée. Veuillez assigner une caméra dans l'inspecteur.");
                enabled = false;
                return;
            }
        }
    }

    private void Update()
    {
        if (cameraTransform != null)
        {
            // Mettre à jour la position
            Vector3 newPosition = cameraTransform.position + (cameraTransform.forward * distanceFromCamera);
            newPosition += cameraTransform.TransformDirection(positionOffset);
            transform.position = newPosition;

            // Mettre à jour la rotation pour faire face à la caméra
            transform.rotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);
        }
    }

    public void changeCameraTransform(Camera camera){
        cameraTransform = camera.transform;
    }
}