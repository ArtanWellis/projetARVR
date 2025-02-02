using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSelector : MonoBehaviour
{
    private Camera mainCamera;
    

    public GameObject currentInfoPanel;
    public TextMeshProUGUI objectNameText;
    public TextMeshProUGUI objectDescriptionText;
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
            ShowInfoPanel(hit.collider.gameObject, objectTag, objectPos);

        }
     
    }

    private void ShowInfoPanel(GameObject target, string name, string description)
    {
       
        objectNameText.text = "Name: " + name;
        objectDescriptionText.text = "Description: " + description;

        Vector3 panelPosition = target.transform.position + target.transform.up * 0.5f + target.transform.right * 0.5f;
        currentInfoPanel.transform.position = panelPosition;
        currentInfoPanel.transform.LookAt(mainCamera.transform);
        currentInfoPanel.transform.Rotate(0, 180, 0);

        if (!isUIVisible)
        {
            currentInfoPanel.SetActive(true);
            isUIVisible = true;
        }
    }

    public void HideInfoPanel()
    {
        if (currentInfoPanel != null)
        {
            currentInfoPanel.SetActive(false);
            isUIVisible = false;
        }
    }
    
     public void ShowInfoPanel()
    {
        if (!isUIVisible)
        {
            currentInfoPanel.SetActive(true);
            isUIVisible = true;
        }
    }

}

public class SelectableObject : MonoBehaviour
{
    public string objectName = "Default Object";
    [TextArea(3, 10)]
    public string description = "This is a selectable object in VR space.";
}


