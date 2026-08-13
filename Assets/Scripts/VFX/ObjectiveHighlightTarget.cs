using UnityEngine;

public class ObjectiveHighlightTarget : MonoBehaviour
{
    [Header("Objective")]
    [SerializeField] private string objectiveID;

    [Header("Renderers")]
    [SerializeField] private Renderer[] targetRenderers;

    private Material[][] originalMaterials;

    public string ObjectiveID => objectiveID;

    private void Awake()
    {
        if (targetRenderers == null ||
            targetRenderers.Length == 0)
        {
            targetRenderers =
                GetComponentsInChildren<Renderer>();
        }

        CacheOriginalMaterials();
    }

    private void Start()
    {
        RegisterWithManager();
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            RegisterWithManager();
        }
    }

    private void OnDisable()
    {
        if (ObjectiveHighlightManager.Instance != null)
        {
            ObjectiveHighlightManager.Instance
                .UnregisterTarget(this);
        }
    }

    private void CacheOriginalMaterials()
    {
        if (targetRenderers == null)
            return;

        originalMaterials =
            new Material[targetRenderers.Length][];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] == null)
                continue;

            originalMaterials[i] =
                targetRenderers[i].sharedMaterials;
        }
    }

    private void RegisterWithManager()
    {
        if (ObjectiveHighlightManager.Instance == null)
            return;

        ObjectiveHighlightManager.Instance
            .RegisterTarget(this);
    }

    public void CompleteObjective()
    {
        if (ObjectiveHighlightManager.Instance == null)
            return;

        ObjectiveHighlightManager.Instance
            .CompleteObjective(objectiveID);
    }

    public void ApplyHighlight(Material highlightMaterial)
    {
        if (highlightMaterial == null ||
            targetRenderers == null)
        {
            return;
        }

        foreach (Renderer renderer in targetRenderers)
        {
            if (renderer == null)
                continue;

            Material[] materials =
                renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = highlightMaterial;
            }

            renderer.sharedMaterials = materials;
        }
    }

    public void RestoreOriginalMaterial()
    {
        if (targetRenderers == null ||
            originalMaterials == null)
        {
            return;
        }

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] == null ||
                originalMaterials[i] == null)
            {
                continue;
            }

            targetRenderers[i].sharedMaterials =
                originalMaterials[i];
        }
    }
}