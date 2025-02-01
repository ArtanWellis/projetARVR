using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VRManageScripts : MonoBehaviour
{
    public GameObject targetObject; // The GameObject with the components
    public MonoBehaviour component1; // First component
    public MonoBehaviour component3; // Third component
    public DropdownController componentDropDown; 
    public ObjectSelector objectSelector; 
    public GameObject placeObjectPanel;
    public GameObject menuPanel;
    public Image controlImage;
    public Sprite controlManipuler;
    public Sprite controlPlacer;
    private Button[] buttons; // Buttons from the top menu
    private int currentButtonIndex = 0; // Tracks the currently selected button
    private Color originalColor;
    public Color hoverColor = Color.gray;
    private bool tenue = false;

    public AudioSource menuAudio;
    void Start()
    {
        // Get all buttons inside the menuPanel
        buttons = GameObject.Find("Fleche").GetComponentsInChildren<Button>();

        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("Buttons not found in the menu panel!");
            return;
        }

        Debug.Log("Buttons count: " + buttons.Length);

        // Add listeners to buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; 
            buttons[i].onClick.AddListener(() => ToggleComponent(index + 1));

            EventTrigger trigger = buttons[i].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => OnPointerEnter(buttons[index]));
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => OnPointerExit(buttons[index]));
            trigger.triggers.Add(entryExit);
        }

        // Activate the first component and simulate hover on the first button
        currentButtonIndex = 0;
        ToggleComponent(1);
        OnPointerEnter(buttons[currentButtonIndex]);
        AnimateButton(0);
    }

  void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return; // Exit if no gamepad is connected
   
        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            menuAudio.Play();
            controlImage.sprite = controlPlacer;
            NavigateToNextButton(1);
        }
        else if (gamepad.dpad.left.wasPressedThisFrame)
        {
            menuAudio.Play();
            controlImage.sprite = controlManipuler;
            NavigateToNextButton(-1);
        }


    }
    private void NavigateToNextButton(int direction)
    {
        // Reset the current button's hover effect
        OnPointerExit(buttons[currentButtonIndex]);

        // Update the current button index
        currentButtonIndex += direction;
        if (currentButtonIndex < 0)
        {
            currentButtonIndex = 0;
        }
        else if (currentButtonIndex >= buttons.Length)
        {
            currentButtonIndex = 1;
        }

        // Apply hover effect to the new button and activate it
        OnPointerEnter(buttons[currentButtonIndex]);
        ToggleComponent(currentButtonIndex + 1);
    }

    // Function to toggle components
    public void ToggleComponent(int componentIndex)
    {
       
        if (placeObjectPanel == null) return;

        component1.enabled = false;
        component3.enabled = false;
        componentDropDown.enabled = false;
        objectSelector.HideInfoPanel();
        objectSelector.enabled = false;
        
      
        switch (componentIndex)
        {
            case 1:
                component1.enabled = true;
                component3.GetComponent<VRPlaceObjects>().ResetHover();
                objectSelector.enabled = true;
                 placeObjectPanel.SetActive(false);
                 objectSelector.ShowInfoPanel();
                break;
            case 2:
                component3.enabled = true;
                  component1.GetComponent<VRManipulateObecjts>().ResetHover();
                placeObjectPanel.SetActive(true);
                componentDropDown.enabled = true;
                break;
            default :
            component3.GetComponent<VRPlaceObjects>().ResetHover();
              component1.GetComponent<VRManipulateObecjts>().ResetHover();
               placeObjectPanel.SetActive(false);
                break; 
        }
        Debug.Log("component 1 : "+ component1.isActiveAndEnabled);
        Debug.Log("component 3 : "+ component3.isActiveAndEnabled);
     //   if(componentIndex>=0)
            AnimateButton(componentIndex - 1);
    }

    private void OnPointerEnter(Button button)
    {
        var backgroundImage = button.GetComponent<Image>();
        originalColor = backgroundImage.color;
        backgroundImage.color = hoverColor;
    }

  
    private void OnPointerExit(Button button)
    {
        var backgroundImage = button.GetComponent<Image>();
        backgroundImage.color = originalColor;
    }

    private void AnimateButton(int buttonIndex)
    {
        Debug.Log("Animate button " + buttonIndex);
        Debug.Log(buttons[buttonIndex].ToString());
        for (int i = 0; i < buttons.Length; i++)
        {
            // Reset all button backgrounds to default (white in this example)
            var backgroundImage = buttons[i].GetComponent<Image>();
            backgroundImage.color = Color.white;
        }

        // Change the clicked button background to green
        var clickedButtonBackground = buttons[buttonIndex].GetComponent<Image>();
        clickedButtonBackground.color = Color.green;
    }
}

  