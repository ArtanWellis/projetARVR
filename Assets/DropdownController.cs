using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class DropdownController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private EventSystem eventSystem;
    private int currentIndex = 0;
    private bool inputDelay = false;
    private float inputDelayTime = 0.2f;
    private float lastInputTime = 0f;
    private StandaloneInputModule inputModule;
    
    void Start()
    {
        eventSystem = EventSystem.current;
        // Récupère le module d'input
        inputModule = eventSystem.GetComponent<StandaloneInputModule>();
        // Désactive la navigation par stick
        DisableStickNavigation();
    }

    private void DisableStickNavigation()
    {
        if (inputModule != null)
        {
            // Définit des axes vides pour le stick
            inputModule.horizontalAxis = "DISABLED_HORIZONTAL";
            inputModule.verticalAxis = "DISABLED_VERTICAL";
        }
    }

    void Update()
    {
        if (Gamepad.current.buttonSouth.wasPressedThisFrame && dropdown.IsActive())
        {
            if (!dropdown.IsExpanded)
            {
                dropdown.Show();
                currentIndex = dropdown.value;
                UpdateVisualSelection();
            }
            else
            {
                dropdown.Hide();
            }
        }

        if (dropdown.IsExpanded && Time.time > lastInputTime + inputDelayTime)
        {
            bool selectionChanged = false;
            
            if (Gamepad.current != null && Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                currentIndex--;
                if (currentIndex < 0) 
                    currentIndex = dropdown.options.Count - 1;
                selectionChanged = true;
            }
            else if (Gamepad.current != null && Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                currentIndex++;
                if (currentIndex >= dropdown.options.Count) 
                    currentIndex = 0;
                selectionChanged = true;
            }

            if (selectionChanged)
            {
                dropdown.value = currentIndex;
                lastInputTime = Time.time;
                UpdateVisualSelection();
            }
        }

        // Bloque explicitement les inputs du stick gauche quand le dropdown est ouvert
        if (dropdown.IsExpanded && Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (stick.magnitude > 0)
            {
                // Empêche la propagation de l'input du stick
                eventSystem.SetSelectedGameObject(eventSystem.currentSelectedGameObject);
            }
        }
    }

    private void UpdateVisualSelection()
    {
        Transform dropdownList = dropdown.transform.Find("Dropdown List");
        if (dropdownList != null)
        {
            Transform viewport = dropdownList.Find("Viewport");
            if (viewport != null)
            {
                Transform content = viewport.Find("Content");
                if (content != null && content.childCount > currentIndex)
                {
                    GameObject selectedItem = content.GetChild(currentIndex).gameObject;
                    eventSystem.SetSelectedGameObject(selectedItem);

                    var pointer = new PointerEventData(eventSystem);
                    pointer.position = RectTransformUtility.WorldToScreenPoint(Camera.main, selectedItem.transform.position);
                    ExecuteEvents.Execute(selectedItem, pointer, ExecuteEvents.pointerEnterHandler);
                }
            }
        }
    }
}