using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice; // Ajout pour le support VR

public class VRTranslate : MonoBehaviour
{
    public AudioSource walkingAudio;
    public float translate_gain = 1.0f;
    public float rotation_gain = 1.0f;
    public float curvature_gain = 1.0f;

    public float m_speed = 0.01f;
    public float r_speed = 0.1f;
    GameObject playerCam;
    GameObject playerReal;
    public float mouseSensitivity = 100f;
    private Vector3 initialCamPosition;
    public Vector3 cameraOffset = new Vector3(0, 0.8f, 0.4f);
    private Quaternion initialCamRotation;
    private Vector3 initialPlayerPosition;
    private float currentPitch = 0f;
    public Animator animator;
    private InputAction moveAction;
    private InputAction lookAction;
    private Gamepad gamepad;

    // Variables pour le tracking VR
    private Vector3 lastHeadPosition;
    private Quaternion lastHeadRotation;
    private Vector3 previousPlayerPosition;

  void Start()
    {
        playerCam = GameObject.Find("PlayerCam");
        playerReal = GameObject.Find("PlayerReal");

        initialCamPosition = playerCam.transform.position;
        initialCamRotation = playerCam.transform.rotation;
        initialPlayerPosition = playerReal.transform.position;
        previousPlayerPosition = playerReal.transform.position;
        // Détecter si une manette est connectée
        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            Debug.Log("Manette détectée : " + gamepad.GetType().Name);

            moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
            moveAction.Enable();

            lookAction = new InputAction("Look", InputActionType.Value, "<Gamepad>/rightStick");
            lookAction.Enable();
        }
        else
        {
            Debug.LogWarning("Aucune manette détectée.");
        }
    }

    void Update()
    {
        HandleControllerInput();
        HandleVRHeadTracking();
      //  smartCamDisplace();
    }

    void HandleVRHeadTracking()
    {
        InputDevice headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (headset.isValid)
        {
            // Obtenir la rotation de la tête
            if (headset.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation))
            {
                // Extraire l'angle Y (yaw) de la rotation de la tête
                Vector3 headEuler = headRotation.eulerAngles;
                
                // Mettre à jour la rotation du playerReal en fonction de la rotation de la tête
                float yawDelta = Mathf.DeltaAngle(lastHeadRotation.eulerAngles.y, headEuler.y);
                playerReal.transform.Rotate(Vector3.up * yawDelta );

                // Mettre à jour le pitch pour la caméra
                currentPitch = headEuler.x;
                if (currentPitch > 180f) currentPitch -= 360f;
                currentPitch = Mathf.Clamp(currentPitch, -90f, 90f);

                // Sauvegarder la dernière rotation
                lastHeadRotation = headRotation;
            }
        }
    }
    

 void HandleControllerInput()
{
    if (gamepad == null) return;

    // Lecture des entrées du joystick
    Vector2 moveInput = moveAction.ReadValue<Vector2>();
    Vector2 lookInput = lookAction.ReadValue<Vector2>();

    // Calcul des vecteurs avant et droite de la caméra
    Vector3 cameraForward = playerCam.transform.forward;
    Vector3 cameraRight = playerCam.transform.right;

    // Éliminer les composantes verticales pour le mouvement
    cameraForward.y = 0;
    cameraRight.y = 0;

    // Normaliser pour un déplacement cohérent
    cameraForward.Normalize();
    cameraRight.Normalize();

    // Calcul du mouvement du joueur
    Vector3 moveDirection = (cameraRight * moveInput.x + cameraForward * moveInput.y) * m_speed * translate_gain;

    // Enregistrer la position avant le déplacement


    // Déplacement du joueur basé sur l'entrée du stick gauche
    playerReal.transform.position += moveDirection;

    // Si le joueur n'a pas bougé (bloqué par un collider), la caméra ne bouge pas
   
    // Mise à jour de la caméra si le joueur a bougé
    bool isWalking = moveInput.sqrMagnitude > 0; // Vérifie si le joueur se déplace
    animator.SetBool("isWalking", isWalking);

    if (isWalking)
    {
        if (!walkingAudio.isPlaying) // Vérifie que l'audio n'est pas déjà en cours
        {
            walkingAudio.Play();
        }
    }
    else
    {
        if (walkingAudio.isPlaying) // Arrête le son si le joueur ne marche plus
        {
            walkingAudio.Stop();
        }
    }

    // Mise à jour de la position précédente
    previousPlayerPosition = playerReal.transform.position;

    // Rotation horizontale (yaw) - Rotation du playerReal
    /* float yawRotation = lookInput.x * r_speed * rotation_gain;
    playerReal.transform.Rotate(Vector3.up * yawRotation);

    // Rotation verticale (pitch) - Mise à jour de currentPitch
    float pitchDelta = -lookInput.y * r_speed * rotation_gain;
    currentPitch = Mathf.Clamp(currentPitch + pitchDelta, -90f, 90f); */
}
    void smartCamDisplace()
    {
        translateCam();
        rotateCam();
    }

    void translateCam()
    {
       // playerCam.transform.position = playerReal.transform.position + cameraOffset;
    }

    void rotateCam()
    {
        Quaternion horizontalRotation = playerReal.transform.rotation;
        Quaternion verticalRotation = Quaternion.Euler(currentPitch, 0, 0);
        playerCam.transform.rotation = horizontalRotation * verticalRotation;
    }

    void curveCam()
    {
        float R = 22f;
        float curvature_gain = 1f / R;

        Vector3 movementDirection = playerReal.transform.forward * m_speed;
        Vector3 perpendicularDirection = Vector3.Cross(movementDirection, Vector3.up).normalized;
        Vector3 curvedMovement = movementDirection + perpendicularDirection * curvature_gain;

        playerCam.transform.position += curvedMovement;
    }
}