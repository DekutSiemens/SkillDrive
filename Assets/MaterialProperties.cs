using UnityEngine;

// Material types for sensor detection
public enum MaterialType
{
    Steel,
    Aluminium,
    Magnet,
    Plastic,
    Glass,
    Wood,
    Air // No material present
}

// Material properties component - attach to objects that can be detected
[System.Serializable]
public class MaterialProperties : MonoBehaviour
{
    [Header("Material Settings")]
    public MaterialType materialType = MaterialType.Steel;

    [Header("Physical Properties")]
    [Range(0f, 1f)] public float reflectivity = 0.8f; // For optical/ultrasonic
    [Range(0f, 1f)] public float conductivity = 0.9f; // For capacitive/inductive
    [Range(0f, 1f)] public float magneticPermeability = 0.1f; // For inductive/hall
    [Range(0f, 1f)] public float dielectricConstant = 0.1f; // For capacitive
    public bool isMagnetic = false; // For hall effect

    void Start()
    {
        SetMaterialProperties();
    }

    void SetMaterialProperties()
    {
        switch (materialType)
        {
            case MaterialType.Steel:
                conductivity = 0.85f;        // High conductivity (ferrous metal)
                magneticPermeability = 0.9f; // High magnetic response
                reflectivity = 0.6f;         // Moderate reflection (matte finish)
                dielectricConstant = 0.1f;   // Low dielectric (conductor)
                isMagnetic = true;           // Ferromagnetic material
                break;

            case MaterialType.Aluminium:
                conductivity = 0.95f;        // Very high conductivity (non-ferrous)
                magneticPermeability = 0.0f; // Non-magnetic
                reflectivity = 0.9f;         // High reflection (shiny surface)
                dielectricConstant = 0.05f;  // Very low dielectric (good conductor)
                isMagnetic = false;          // Non-magnetic
                break;

            case MaterialType.Magnet:
                conductivity = 0.4f;         // Lower conductivity (ceramic/ferrite magnets)
                magneticPermeability = 1.0f; // Maximum magnetic response
                reflectivity = 0.3f;         // Low reflection (dark surface)
                dielectricConstant = 0.3f;   // Moderate dielectric
                isMagnetic = true;           // Strong magnetic field
                break;

            case MaterialType.Plastic:
                conductivity = 0.0f;         // Insulator (no conductivity)
                magneticPermeability = 0.0f; // Non-magnetic
                reflectivity = 0.5f;         // Moderate reflection
                dielectricConstant = 0.8f;   // High dielectric constant
                isMagnetic = false;          // Non-magnetic
                break;

            case MaterialType.Glass:
                conductivity = 0.0f;         // Insulator
                magneticPermeability = 0.0f; // Non-magnetic
                reflectivity = 0.8f;         // High reflection (smooth surface)
                dielectricConstant = 0.6f;   // Moderate-high dielectric
                isMagnetic = false;          // Non-magnetic
                break;

            case MaterialType.Wood:
                conductivity = 0.05f;        // Very low (slightly conductive when wet)
                magneticPermeability = 0.0f; // Non-magnetic
                reflectivity = 0.2f;         // Low reflection (rough, absorptive)
                dielectricConstant = 0.4f;   // Moderate dielectric
                isMagnetic = false;          // Non-magnetic
                break;
        }
    }
}