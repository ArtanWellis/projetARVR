using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class QRCodeDisplay : MonoBehaviour
{
    public GameObject qrPrefab;
    public ARTrackedImageManager arManager;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    private void OnEnable() => arManager.trackedImagesChanged += OnTrackedImagesChanged;
    private void OnDisable() => arManager.trackedImagesChanged -= OnTrackedImagesChanged;


    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added)
        {
            if (spawnedObjects.ContainsKey(img.referenceImage.guid.ToString())) continue;

            GameObject cube = Instantiate(qrPrefab, img.transform.position, img.transform.rotation);
            cube.transform.localScale = new Vector3(img.size.x, 0.001f, img.size.y);
            cube.name = img.referenceImage.name;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(cube.transform);
            textObj.transform.localPosition = new Vector3(0, 5f, 0);
            textObj.transform.localRotation = Quaternion.Euler(90, 0, 0); 

            TextMesh text = textObj.AddComponent<TextMesh>();
            text.text = "Salle " + img.referenceImage.name.Split("room")[1].Trim();
            Debug.Log($" name : {text.text}");
            text.fontSize = 70;
            text.alignment = TextAlignment.Center;
            text.anchor = TextAnchor.MiddleCenter;
            text.color = Color.white;
            textObj.transform.localScale = Vector3.one * Mathf.Min(img.size.x, img.size.y) * 0.5f;

            cube.AddComponent<BoxCollider>().size = Vector3.one;

            spawnedObjects[img.referenceImage.guid.ToString()] = cube;
        }

        foreach (var img in args.updated)
        {
            if (spawnedObjects.TryGetValue(img.referenceImage.guid.ToString(), out GameObject cube))
            {
                cube.SetActive(img.trackingState == TrackingState.Tracking);
                if (cube.activeSelf)
                {
                    cube.transform.position = img.transform.position;
                    cube.transform.rotation = img.transform.rotation;
                }
            }
        }

        foreach (var img in args.removed)
        {
            if (spawnedObjects.TryGetValue(img.referenceImage.guid.ToString(), out GameObject cube))
            {
                Destroy(cube);
                spawnedObjects.Remove(img.referenceImage.guid.ToString());
            }
        }
    }
}
