using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public static class SensorTypes
{
    public enum MaterialType
    {
        Wood,
        Glass,
        Plastic,
        Magnet,
        Aluminum,
        Steel
    }

    public enum SensorType
    {
        Capacitive,
        Inductive,
        Hall,
        Ultrasonic,
        Optical
    }
}

[AddComponentMenu("Sensors/Material Sensor")]
public class MaterialSensor : MonoBehaviour
{
    [Header("Sensor Configuration")]
    [SerializeField] private SensorTypes.SensorType sensorType = SensorTypes.SensorType.Optical;
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private LayerMask detectionLayer;
    [Tooltip("Custom local direction of the ray (e.g., forward, up, right)")]
    [SerializeField] private Vector3 rayDirection = Vector3.forward;

    [Header("LED Indicator")]
    [Tooltip("Assign a GameObject (e.g., small sphere or light) to act as LED")]
    [SerializeField] private GameObject ledIndicator;
    [SerializeField] private Color ledActiveColor = Color.green;
    [SerializeField] private Color ledInactiveColor = Color.red;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onDetectionBegin;
    [SerializeField] private UnityEvent onDetectionEnd;
    [SerializeField] private UnityEvent<SensorTypes.MaterialType> onMaterialDetected;

    private Dictionary<SensorTypes.SensorType, List<SensorTypes.MaterialType>> detectionCapabilities;
    private Dictionary<string, SensorTypes.MaterialType> tagToMaterialMap;
    private Dictionary<(SensorTypes.SensorType, SensorTypes.MaterialType), float> materialSensingRanges;

    private Ray ray;
    private RaycastHit hit;
    private bool isDetecting;
    private GameObject detectedObject;
    private SensorTypes.MaterialType? detectedMaterial;

    public bool IsDetecting => isDetecting;
    public float DetectionRange => detectionRange;
    public GameObject DetectedObject => detectedObject;
    public SensorTypes.MaterialType? DetectedMaterial => detectedMaterial;

    private void Awake()
    {
        InitializeDetectionCapabilities();
        InitializeTagMappings();
        InitializeMaterialSensingRanges();
        UpdateLED(false);
    }

    private void InitializeDetectionCapabilities()
    {
        detectionCapabilities = new Dictionary<SensorTypes.SensorType, List<SensorTypes.MaterialType>>
        {
            { SensorTypes.SensorType.Capacitive, new List<SensorTypes.MaterialType> {
                SensorTypes.MaterialType.Wood,
                SensorTypes.MaterialType.Glass,
                SensorTypes.MaterialType.Plastic,
                SensorTypes.MaterialType.Aluminum,
                SensorTypes.MaterialType.Steel,
                SensorTypes.MaterialType.Magnet
            }},
            { SensorTypes.SensorType.Inductive, new List<SensorTypes.MaterialType> {
                SensorTypes.MaterialType.Aluminum,
                SensorTypes.MaterialType.Steel
            }},
            { SensorTypes.SensorType.Hall, new List<SensorTypes.MaterialType> {
                SensorTypes.MaterialType.Magnet
            }},
            { SensorTypes.SensorType.Ultrasonic, new List<SensorTypes.MaterialType> {
                SensorTypes.MaterialType.Wood,
                SensorTypes.MaterialType.Glass,
                SensorTypes.MaterialType.Plastic,
                SensorTypes.MaterialType.Magnet,
                SensorTypes.MaterialType.Aluminum,
                SensorTypes.MaterialType.Steel
            }},
            { SensorTypes.SensorType.Optical, new List<SensorTypes.MaterialType> {
                SensorTypes.MaterialType.Wood,
                SensorTypes.MaterialType.Plastic,
                SensorTypes.MaterialType.Magnet,
                SensorTypes.MaterialType.Aluminum,
                SensorTypes.MaterialType.Steel
            }}
        };
    }

    private void InitializeTagMappings()
    {
        tagToMaterialMap = new Dictionary<string, SensorTypes.MaterialType>
        {
            { "Wood", SensorTypes.MaterialType.Wood },
            { "Glass", SensorTypes.MaterialType.Glass },
            { "Plastic", SensorTypes.MaterialType.Plastic },
            { "Magnet", SensorTypes.MaterialType.Magnet },
            { "Aluminum", SensorTypes.MaterialType.Aluminum },
            { "Steel", SensorTypes.MaterialType.Steel }
        };
    }

    private void InitializeMaterialSensingRanges()
    {
        materialSensingRanges = new Dictionary<(SensorTypes.SensorType, SensorTypes.MaterialType), float>
        {
            // Capacitive
            { (SensorTypes.SensorType.Capacitive, SensorTypes.MaterialType.Plastic), 0.6f },
            { (SensorTypes.SensorType.Capacitive, SensorTypes.MaterialType.Glass), 0.4f },
            { (SensorTypes.SensorType.Capacitive, SensorTypes.MaterialType.Wood), 0.3f },
            { (SensorTypes.SensorType.Capacitive, SensorTypes.MaterialType.Magnet), 0.3f },
            { (SensorTypes.SensorType.Capacitive, SensorTypes.MaterialType.Aluminum), 0.4f },
            { (SensorTypes.SensorType.Capacitive, SensorTypes.MaterialType.Steel), 0.5f },


            // Inductive
            { (SensorTypes.SensorType.Inductive, SensorTypes.MaterialType.Steel), 1.0f },
            { (SensorTypes.SensorType.Inductive, SensorTypes.MaterialType.Aluminum), 0.8f },

            // Hall
            { (SensorTypes.SensorType.Hall, SensorTypes.MaterialType.Magnet), 0.9f },

            // Optical
            { (SensorTypes.SensorType.Optical, SensorTypes.MaterialType.Plastic), 0.9f },
            { (SensorTypes.SensorType.Optical, SensorTypes.MaterialType.Steel), 1.0f },
            { (SensorTypes.SensorType.Optical, SensorTypes.MaterialType.Wood), 0.7f },

            // Ultrasonic
            { (SensorTypes.SensorType.Ultrasonic, SensorTypes.MaterialType.Wood), 1.2f },
            { (SensorTypes.SensorType.Ultrasonic, SensorTypes.MaterialType.Plastic), 1.0f },
            { (SensorTypes.SensorType.Ultrasonic, SensorTypes.MaterialType.Glass), 1.0f },
            { (SensorTypes.SensorType.Ultrasonic, SensorTypes.MaterialType.Magnet), 1.0f },
            { (SensorTypes.SensorType.Ultrasonic, SensorTypes.MaterialType.Steel), 1.0f },
            { (SensorTypes.SensorType.Ultrasonic, SensorTypes.MaterialType.Aluminum), 1.0f }
        };
    }

    private void FixedUpdate()
    {
        UpdateDetection();
    }

    private void UpdateDetection()
    {
        bool wasDetecting = isDetecting;
        isDetecting = false;
        detectedObject = null;
        detectedMaterial = null;

        Vector3 worldRayDirection = transform.TransformDirection(rayDirection.normalized);
        ray = new Ray(transform.position, worldRayDirection);

        if (Physics.Raycast(ray, out hit, detectionRange, detectionLayer))
        {
            string tag = hit.collider.tag;

            if (tagToMaterialMap.TryGetValue(tag, out SensorTypes.MaterialType material))
            {
                if (detectionCapabilities[sensorType].Contains(material))
                {
                    float maxRange = materialSensingRanges.TryGetValue((sensorType, material), out float r)
                        ? r
                        : detectionRange;

                    if (hit.distance <= maxRange)
                    {
                        isDetecting = true;
                        detectedObject = hit.collider.gameObject;
                        detectedMaterial = material;

                        if (showDebugInfo)
                        {
                            Debug.Log($"{sensorType} sensor detected {material} (tag: {tag}) at {hit.distance:F2}m [limit: {maxRange}m]");
                        }

                        onMaterialDetected?.Invoke(material);
                    }
                }
            }
        }

        if (isDetecting != wasDetecting)
        {
            if (isDetecting) onDetectionBegin?.Invoke();
            else onDetectionEnd?.Invoke();

            UpdateLED(isDetecting);
        }
    }

    private void UpdateLED(bool active)
    {
        if (ledIndicator != null)
        {
            Renderer renderer = ledIndicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = active ? ledActiveColor : ledInactiveColor;
            }
        }
    }

    public bool CanDetectMaterial(SensorTypes.MaterialType material)
    {
        return detectionCapabilities[sensorType].Contains(material);
    }

    public float GetDetectedDistance()
    {
        return isDetecting ? hit.distance : -1f;
    }

    public List<SensorTypes.MaterialType> GetDetectableMaterials()
    {
        return new List<SensorTypes.MaterialType>(detectionCapabilities[sensorType]);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 direction = transform.TransformDirection(rayDirection.normalized);
        Gizmos.color = isDetecting ? ledActiveColor : ledInactiveColor;
        Gizmos.DrawRay(transform.position, direction * detectionRange);
        Gizmos.DrawWireSphere(transform.position + (direction * detectionRange), 0.05f);

        if (showDebugInfo)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.1f, sensorType.ToString());
        }
    }
#endif
}
