using UnityEngine;

public class GlowOnEvent : MonoBehaviour
{
    public Color glowColor = Color.cyan;
    public float glowIntensity = 2f;

    private Material material;
    private Color originalEmission;
    private bool isGlowing = false;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        // Clone the material so the original asset is not modified
        material = renderer.material;
        originalEmission = material.GetColor("_EmissionColor");

        // Make sure emission is enabled
        material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (isGlowing)
        {
            material.SetColor("_EmissionColor", glowColor * glowIntensity);
        }
        else
        {
            material.SetColor("_EmissionColor", originalEmission);
        }
    }

    public void StartGlow()
    {
        isGlowing = true;
    }

    public void StopGlow()
    {
        isGlowing = false;
    }
}
