using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ObjectSelector : MonoBehaviour
{
    private Camera mainCamera;
 
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI objectNameText;
    [SerializeField] private TextMeshProUGUI objectDescriptionText;
    private bool isUIVisible = false;
    private void Start()
    {
        mainCamera = Camera.main;

        
    }

    private void Update()
    {
        HandleHover();
    
    }

    private void HandleHover()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && Gamepad.current.rightShoulder.isPressed)
        {
             string objectTag = hit.collider.gameObject.tag;
             string objectPos = "Position : " + hit.collider.gameObject.transform.position.ToString() + "\nRotation : " 
             + hit.collider.gameObject.transform.rotation.ToString() + "\nScale: " 
             +hit.collider.gameObject.transform.localScale.ToString() ;
             UpdateInfoPanel(objectTag, objectPos);
      
        
            ShowInfoPanel();

        }
       
    }
      private void UpdateInfoPanel(string name, string description)
    {

            objectNameText.text = "Name : "+ name;
            objectDescriptionText.text = "Description : " + description;
        
    }
     public void ShowInfoPanel()
    {
        if (!isUIVisible)
        {
            infoPanel.SetActive(true);
            isUIVisible = true;
        }
    }

    public void HideInfoPanel()
    {
      
            infoPanel.SetActive(false);
            isUIVisible = false;
        
    }



}

public class SelectableObject : MonoBehaviour
{
    public string objectName = "Default Object";
    [TextArea(3, 10)]
    public string description = "This is a selectable object in VR space.";
}

