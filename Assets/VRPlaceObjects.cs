using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class VRPlaceObjects : MonoBehaviour
{
    private Camera mainCamera;
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    public Material hoverMaterial;
    private Renderer targetRenderer;

    public string targetTag = "Floor";
    public TMP_Dropdown prefabDropdown;
    private List<GameObject> availablePrefabs = new List<GameObject>();
    private Dictionary<string, Material[]> prefabOriginalMaterials = new Dictionary<string, Material[]>(); // Nouveau dictionnaire pour stocker les matériaux originaux

    void Start()
    {
        mainCamera = Camera.main;

        // Création du matériau de survol si non assigné
        if (hoverMaterial == null)
        {
            hoverMaterial = new Material(Shader.Find("Standard"));
            hoverMaterial.color = Color.white;
        }

        LoadAllPrefabs();
        PopulateDropdown();
        StorePrefabMaterials();
    }

    void Update()
    {
        HandleCameraRaycast();
        HandlePlacement();
    }
     private void StorePrefabMaterials()
    {
        foreach (GameObject prefab in availablePrefabs)
        {
            // Récupère tous les Renderers du prefab
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            List<Material> materials = new List<Material>();

            foreach (Renderer renderer in renderers)
            {
                // Stocke tous les matériaux de chaque renderer
                materials.AddRange(renderer.sharedMaterials);
            }

            // Stocke les matériaux dans le dictionnaire
            prefabOriginalMaterials[prefab.name] = materials.ToArray();
        }
        
    }

    private void HandleCameraRaycast()
    {
        // Utilise la position et la direction de la caméra pour le raycast
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && IsFloorTile(hit.collider.gameObject))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer != null)
            {
                if (renderer != targetRenderer)
                {
                    ResetPreviousObject();

                    targetRenderer = renderer;

                    // Sauvegarde le matériau original si pas encore sauvegardé
                    if (!originalMaterials.ContainsKey(renderer))
                    {
                        originalMaterials[renderer] = renderer.material;
                    }

                    // Applique le matériau de survol
                    renderer.material = hoverMaterial;
                }
            }
        }
        else
        {
            ResetPreviousObject();
        }
    }

    private void HandlePlacement()
    {
        if (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame)
        {
            if (targetRenderer != null)
            {
                PlacePrefabOnTile(targetRenderer.gameObject);
            }
        }
    }
        private void ApplyOriginalMaterials(GameObject instantiatedObject, string prefabName)
    {
        if (prefabOriginalMaterials.ContainsKey(prefabName))
        {
            Renderer[] renderers = instantiatedObject.GetComponentsInChildren<Renderer>(true);
            Material[] originalMats = prefabOriginalMaterials[prefabName];
            
            int materialIndex = 0;
            foreach (Renderer renderer in renderers)
            {
                Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (materialIndex < originalMats.Length)
                    {
                        newMaterials[i] = new Material(originalMats[materialIndex]);
                        materialIndex++;
                    }
                }
                renderer.materials = newMaterials;
            }
        }
    }

    private void ResetPreviousObject()
    {
        if (targetRenderer != null)
        {
            // Vérifie si un matériau original a été sauvegardé
            if (originalMaterials.ContainsKey(targetRenderer))
            {
                targetRenderer.material = originalMaterials[targetRenderer];
            }

            // Supprime le renderer du dictionnaire
            originalMaterials.Remove(targetRenderer);

            targetRenderer = null;
        }
    }

     private void PlacePrefabOnTile(GameObject targetObject)
    {
        if (prefabDropdown != null && prefabDropdown.value < availablePrefabs.Count)
        {
            GameObject selectedPrefab = availablePrefabs[prefabDropdown.value];
            if (selectedPrefab != null)
            {
                // Calcule le centre du tile
                Vector3 tileCenter = targetObject.GetComponent<Renderer>().bounds.center;

                // Instancie le prefab
                GameObject instantiatedObject = Instantiate(selectedPrefab, tileCenter, Quaternion.identity);

                instantiatedObject.tag = "NewObject";
                // Applique les matériaux originaux
                ApplyOriginalMaterials(instantiatedObject, selectedPrefab.name);

                Debug.Log("Objet placé au centre : " + tileCenter);
            }
        }
        else
        {
            Debug.LogWarning("Aucun prefab sélectionné ou valeur du dropdown hors limites.");
        }
    }
    private void LoadAllPrefabs()
    {
    string[] folders = { "Props" };
    availablePrefabs.Clear();
        
        foreach (string folder in folders)
        {
            Debug.Log(Resources.LoadAll<Object>(folder));
             string fullPath = System.IO.Path.Combine(Application.dataPath, "Resources", folder);
                Debug.Log($"Recherche de prefabs dans : {fullPath}");
            GameObject[] prefabsInFolder = Resources.LoadAll<GameObject>(folder);
            availablePrefabs.AddRange(prefabsInFolder);
        }
    }

   private string CleanPrefabName(string originalName)
    {
        // Enlève "SM" du nom
        string cleanName = originalName.Replace("SM", "");
        
        // Remplace les underscores par des espaces
        cleanName = cleanName.Replace('_', ' ');
        
        // Enlève les espaces multiples qui pourraient résulter des remplacements
        while (cleanName.Contains("  "))
        {
            cleanName = cleanName.Replace("  ", " ");
        }
        
        // Nettoie les espaces au début et à la fin
        cleanName = cleanName.Trim();
        
        return cleanName;
    }

    private void PopulateDropdown()
    {
        if (prefabDropdown != null)
        {
            prefabDropdown.ClearOptions();
            List<string> options = new List<string>();

            foreach (GameObject prefab in availablePrefabs)
            {
                // Applique le nettoyage du nom
                string cleanName = CleanPrefabName(prefab.name);
                options.Add(cleanName);
            }

            prefabDropdown.AddOptions(options);
        }
        else
        {
            Debug.LogWarning("Le dropdown des prefabs n'est pas assigné dans l'inspecteur.");
        }
    }
    private bool IsFloorTile(GameObject gameObject)
    {
        return gameObject.CompareTag(targetTag);
    }
       public void ResetHover()
    {
        if (targetRenderer != null)
        {
            // Restaure le matériau d'origine si disponible
            if (originalMaterials.ContainsKey(targetRenderer))
            {
                targetRenderer.material = originalMaterials[targetRenderer];
                originalMaterials.Remove(targetRenderer);
            }
            targetRenderer = null;
        }
    }
}
