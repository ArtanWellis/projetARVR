using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class manageTenues : MonoBehaviour
{
    public AudioSource menuAudio;
    public GameObject tenuesHomme;
    public GameObject tenuesFemme;
    public GameObject menuHomme;
    public GameObject menuFemme;
    public Image tenueChoisie;
    public ScrollRect scrollRect;
    public GameObject playerReal;
    public GameObject menuVetement;
    public GameObject menuFerme;
    public Camera cameraPlayer;
    public Camera cameraVetement;
    public GameObject flecheButton;
    public VRTranslate vRTranslate;
    public Image contour;

    public GameObject tenueCliquee;

    public List<GameObject> tenuesListeHomme;
    public List<GameObject> tenuesListeFemme;

    private RectTransform tenueCliqueeRect;
    private RectTransform tenueChoisieRect;
    private RectTransform contourRect;
    private bool isMenuOpen = false;
    private Vector3 positionInitPlayer;
    private Quaternion rotationInitPlayer;
    private Vector3 positionCurrentPlayer;
    private Quaternion rotationCurrentPlayer;

    private int currentIndex = 0;
    private List<GameObject> currentList;
    private bool isNavigatingMale = true;
    private float navigationCooldown = 0.2f;
    private float lastNavigationTime;

    public CanvasVRFollower canvasVRFollower;
    public VRManageScripts manageMenu;
    public VRPlaceObjects vRPlaceObjects;
    public VRManipulateObecjts vRManipulateObecjts;
    public ObjectSelector objectSelector;

    public Image controlImage;
    public Sprite controlVetement;
    public Sprite controlManipuler;
    void Start()
    {
        InitializeUI();
        SetupInitialState();
    }

    void InitializeUI()
    {
        tenuesFemme.SetActive(false);
        tenuesHomme.SetActive(true);
        menuHomme.SetActive(true);
        menuFemme.SetActive(false);

        currentList = tenuesListeHomme;

        if (currentList.Count > 0)
        {
            tenueCliquee = currentList[0];
            UpdateVisualElements();
        }

        contour.gameObject.SetActive(false);
    }

    void SetupInitialState()
    {
        cameraPlayer.enabled = true;
        cameraVetement.enabled = false;
        menuVetement.SetActive(false);

        positionInitPlayer = playerReal.transform.position;
        rotationInitPlayer = playerReal.transform.rotation;
    }

    void Update()
    {   
        if(isMenuOpen){
            objectSelector.HideInfoPanel();
        }
        HandleGamepadInput();
        UpdateVisualElements();
    }

    void HandleGamepadInput()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightStickButton.wasPressedThisFrame)
            {
                ToggleMenu();

            }

            if (isMenuOpen)
            {
                HandleGamepadNavigation();
            }
        }
    }

    void UpdateVisualElements()
    {
        if (tenueCliquee != null)
        {
            tenueCliqueeRect = tenueCliquee.GetComponent<RectTransform>();
            tenueChoisieRect = tenueChoisie.GetComponent<RectTransform>();

            if (tenueCliqueeRect != null && tenueChoisieRect != null)
            {
                // Mise à jour de la position du fond de sélection
                tenueChoisie.rectTransform.position = tenueCliqueeRect.position;
            }

            // Mise à jour du contour si on navigue avec la manette
            if (isMenuOpen && contour.gameObject.activeSelf)
            {
                contour.rectTransform.position = currentList[currentIndex].GetComponent<RectTransform>().position;
            }
        }
    }

    void HandleGamepadNavigation()
    {
        if (Time.time - lastNavigationTime < navigationCooldown)
            return;

        var gamepad = Gamepad.current;
        bool navigationChanged = false;

        // Navigation verticale (2 par 2)
        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            currentIndex = Mathf.Max(0, currentIndex - 2);
            navigationChanged = true;
        }
        else if (gamepad.dpad.down.wasPressedThisFrame)
        {
            currentIndex = Mathf.Min(currentList.Count - 1, currentIndex + 2);
            navigationChanged = true;
        }

        // Navigation horizontale (1 par 1)
        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            currentIndex = Mathf.Min(currentList.Count - 1, currentIndex + 1);
            navigationChanged = true;
        }
        else if (gamepad.dpad.left.wasPressedThisFrame)
        {
            currentIndex = Mathf.Max(0, currentIndex - 1);
            navigationChanged = true;
        }

        // Changement de catégorie avec L1/R1
        if (gamepad.rightShoulder.wasPressedThisFrame)  // R1
        {
            isNavigatingMale = false;
            currentList = tenuesListeFemme;
            currentIndex = 0;
            changeCategoriesFemme();
            navigationChanged = true;
        }
        else if (gamepad.leftShoulder.wasPressedThisFrame) 
        {
            isNavigatingMale = true;
            currentList = tenuesListeHomme;
            currentIndex = 0;
            changeCategoriesHomme();
            navigationChanged = true;
        }

        // Sélection
        if (gamepad.buttonSouth.wasPressedThisFrame && currentList.Count > 0)
        {
            SelectCurrentTenue();
        }

        if (navigationChanged)
        {
            UpdateScrollPosition();
            contour.gameObject.SetActive(true);
            lastNavigationTime = Time.time;
        }
    }

    void UpdateScrollPosition()
    {
        if (currentList.Count > 0)
        {
            float normalizedPosition = 1f - (float)currentIndex / (currentList.Count - 1);
            scrollRect.verticalNormalizedPosition = normalizedPosition;
        }
    }

    void SelectCurrentTenue()
    {
        if (currentList.Count > 0)
        {
            tenueCliquee = currentList[currentIndex];

            // Désactiver toutes les tenues
            foreach (Transform child in playerReal.transform)
            {
                if(child.gameObject.name != "Root" && child.gameObject.name != "PlayerCam")
                    child.gameObject.SetActive(false);
            }

            // Activer la tenue sélectionnée
            Transform tenueToActivate = playerReal.transform.Find(tenueCliquee.name);
            if (tenueToActivate != null)
            {
                tenueToActivate.gameObject.SetActive(true);
            }
           
            tenueChoisie.gameObject.SetActive(true);
            contour.gameObject.SetActive(false);
            UpdateVisualElements();
        }
    }

    public void clickOnTenue()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        GraphicRaycaster raycaster = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();

        if (raycaster != null)
        {
            raycaster.Raycast(pointerData, raycastResults);

            if (raycastResults.Count > 0)
            {
                tenueCliquee = raycastResults[0].gameObject;
                tenueChoisie.gameObject.SetActive(true);
                contour.gameObject.SetActive(false);

                // Désactiver toutes les tenues
                foreach (Transform child in playerReal.transform)
                {
                    child.gameObject.SetActive(false);
                }

                // Activer la tenue sélectionnée
                Transform tenueToActivate = playerReal.transform.Find(tenueCliquee.name);
                if (tenueToActivate != null)
                {
                    tenueToActivate.gameObject.SetActive(true);
                }

                UpdateVisualElements();
            }
        }
    }

    void ToggleMenu()
    {
        menuAudio.Play();
        isMenuOpen = !isMenuOpen;
        if(isMenuOpen){
            controlImage.sprite = controlVetement;
        }else{
             controlImage.sprite = controlManipuler;
        }
        menuVetement.SetActive(isMenuOpen);
        cameraPlayer.enabled = !isMenuOpen;
        cameraVetement.enabled = isMenuOpen;
        if(isMenuOpen){
            canvasVRFollower.changeCameraTransform(cameraVetement);
        }else{
            canvasVRFollower.changeCameraTransform(cameraPlayer);
        }
        flecheButton.SetActive(!isMenuOpen);
        contour.gameObject.SetActive(isMenuOpen);

        positionCurrentPlayer = playerReal.transform.position;
        rotationCurrentPlayer = playerReal.transform.rotation;
        if(isMenuOpen){GameObject.Find("Head").transform.localScale= new Vector3(1f, 1f, 1f);}
        else{
            GameObject.Find("Head").transform.localScale= new Vector3(0f, 0f, 0f);
        }
        playerReal.transform.position = isMenuOpen ? positionInitPlayer : positionCurrentPlayer;
        playerReal.transform.rotation = isMenuOpen ? rotationInitPlayer : rotationCurrentPlayer;

        vRTranslate.enabled = !isMenuOpen;
        manageMenu.enabled = !isMenuOpen;
    
        if(vRManipulateObecjts.isActiveAndEnabled){
            vRManipulateObecjts.enabled = false;
           
        }
        if(vRPlaceObjects.isActiveAndEnabled){vRPlaceObjects.enabled = false;}
        Debug.Log(manageMenu.isActiveAndEnabled);
    }

    public void changeCategoriesFemme()
    {
        tenuesFemme.SetActive(true);
        tenuesHomme.SetActive(false);
        menuHomme.SetActive(false);
        menuFemme.SetActive(true);
        scrollRect.content = tenuesFemme.GetComponent<RectTransform>();
        currentList = tenuesListeFemme;

        UpdateTenueChoisieVisibility("Female");
    }

    public void changeCategoriesHomme()
    {
        tenuesFemme.SetActive(false);
        tenuesHomme.SetActive(true);
        menuHomme.SetActive(true);
        menuFemme.SetActive(false);
        scrollRect.content = tenuesHomme.GetComponent<RectTransform>();
        currentList = tenuesListeHomme;

        UpdateTenueChoisieVisibility("Male");
    }

    private void UpdateTenueChoisieVisibility(string category)
    {
        if (tenueCliquee != null)
        {
            tenueChoisie.gameObject.SetActive(tenueCliquee.name.Contains(category));
        }
        else
        {
            tenueChoisie.gameObject.SetActive(false);
        }
    }
}