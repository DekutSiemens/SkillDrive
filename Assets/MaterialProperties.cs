using UnityEngine;
using System.Collections.Generic;

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

// Sensor types
public enum SensorType
{
    Capacitive,
    Inductive,
    Optical,
    Ultrasonic,
    HallEffect
}

// Material properties component
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
                conductivity = 0.9f;
                magneticPermeability = 0.8f;
                reflectivity = 0.7f;
                dielectricConstant = 0.1f;
                isMagnetic = true;
                break;

            case MaterialType.Aluminium:
                conductivity = 0.95f;
                magneticPermeability = 0.1f;
                reflectivity = 0.9f;
                dielectricConstant = 0.1f;
                isMagnetic = false;
                break;

            case MaterialType.Magnet:
                conductivity = 0.3f;
                magneticPermeability = 1.0f;
                reflectivity = 0.5f;
                dielectricConstant = 0.2f;
                isMagnetic = true;
                break;

            case MaterialType.Plastic:
                conductivity = 0.0f;
                magneticPermeability = 0.0f;
                reflectivity = 0.6f;
                dielectricConstant = 0.8f;
                isMagnetic = false;
                break;

            case MaterialType.Glass:
                conductivity = 0.0f;
                magneticPermeability = 0.0f;
                reflectivity = 0.4f;
                dielectricConstant = 0.6f;
                isMagnetic = false;
                break;

            case MaterialType.Wood:
                conductivity = 0.1f;
                magneticPermeability = 0.0f;
                reflectivity = 0.3f;
                dielectricConstant = 0.5f;
                isMagnetic = false;
                break;
        }
    }
}