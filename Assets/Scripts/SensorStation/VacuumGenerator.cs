using UnityEngine;

public class VacuumGenerator : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    [Header("Manual Control")]
    public bool vacuumState = false; // Toggle manually from Inspector

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning("VacuumGenerator: MeshRenderer not found on this GameObject.");
        }
    }

    void Update()
    {
        // Automatically sync MeshRenderer visibility with vacuumState toggle
        if (meshRenderer != null && meshRenderer.enabled != vacuumState)
        {
            meshRenderer.enabled = vacuumState;
        }
    }

    public void VacuumOn()
    {
        vacuumState = true;
        if (meshRenderer != null)
            meshRenderer.enabled = true;
    }

    public void VacuumOff()
    {
        vacuumState = false;
        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }
}
