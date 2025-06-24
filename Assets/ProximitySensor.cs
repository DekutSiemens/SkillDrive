using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum SensorType
{
    Capacitive,
    Inductive,
    Optical,
    Ultrasonic,
    HallEffect
}

public enum DetectionAxis
{
    PositiveX,
    NegativeX,
    PositiveY,
    NegativeY,
    PositiveZ,
    NegativeZ
}

public class ProximitySensor : MonoBehaviour
{
    [Header("Sensor Configuration")]
    public SensorType sensorType = SensorType.Inductive;
    public DetectionAxis detectionAxis = DetectionAxis.PositiveZ;
    public float detectionRange = 0.5f; // Default initial value
    public LayerMask detectionLayer = -1;

    [Header("Sensor Specific Settings")]
    [Range(0f, 1f)] public float sensitivity = 0.5f;
    public bool debugMode = true;

    [Header("Positioning Control")]
    public bool enableDetection = false;

    [Header("Actuator Connection")]
    public StepperMotor connectedActuator;
    public int stepsOnDetection = 10;

    [Header("UI Controls")]
    public Slider rangeSlider;             // Assign this in the Inspector
    public TMP_Text rangeLabel;
    

    // Detection state
    private bool isDetecting = false;
    private bool previousDetectionState = false;
    private MaterialProperties currentMaterial;

    // Visual feedback
    private Renderer sensorRenderer;
    private Color originalColor;
    private Color detectionColor = Color.green;

    void Start()
    {
        sensorRenderer = GetComponent<Renderer>();
        if (sensorRenderer != null)
        {
            originalColor = sensorRenderer.material.color;
        }
    }

    void Update()
    {
        // Update detection range from UI slider (0 to 1)
        if (rangeSlider != null)
        {
            detectionRange = rangeSlider.value;
        }

        if (rangeLabel != null)
        {
            rangeLabel.text = $"Range: {detectionRange:F2} m";
        }


        if (enableDetection)
        {
            PerformDetection();
            HandleDetectionChange();
        }
        else
        {
            isDetecting = false;
            currentMaterial = null;
        }

        UpdateVisualFeedback();
    }

    void PerformDetection()
    {
        Vector3 detectionDirection = GetDetectionDirection();
        RaycastHit[] hits = Physics.RaycastAll(transform.position, detectionDirection, detectionRange, detectionLayer);

        bool materialDetected = false;
        MaterialProperties closestMaterial = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform) continue;

            MaterialProperties material = hit.collider.GetComponent<MaterialProperties>();
            if (material != null)
            {
                float distance = hit.distance;

                if (CanDetectMaterial(material, distance))
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMaterial = material;
                        materialDetected = true;
                    }
                }
            }
        }

        isDetecting = materialDetected;
        currentMaterial = closestMaterial;

        if (debugMode && isDetecting && currentMaterial != null)
        {
            Debug.Log($"{sensorType} Sensor detected: {currentMaterial.materialType} at distance {closestDistance:F2}");
        }
    }

    Vector3 GetDetectionDirection()
    {
        switch (detectionAxis)
        {
            case DetectionAxis.PositiveX: return Vector3.right;
            case DetectionAxis.NegativeX: return Vector3.left;
            case DetectionAxis.PositiveY: return Vector3.up;
            case DetectionAxis.NegativeY: return Vector3.down;
            case DetectionAxis.PositiveZ: return Vector3.forward;
            case DetectionAxis.NegativeZ: return Vector3.back;
            default: return Vector3.forward;
        }
    }

    bool CanDetectMaterial(MaterialProperties material, float distance)
    {
        if (distance > detectionRange) return false;

        float distanceFactor = 1f - (distance / detectionRange);
        float detectionThreshold = sensitivity;

        switch (sensorType)
        {
            case SensorType.Inductive:
                if (material.materialType == MaterialType.Steel)
                    return (material.conductivity * distanceFactor) > detectionThreshold;
                else if (material.materialType == MaterialType.Aluminium)
                    return (material.conductivity * distanceFactor * 0.7f) > detectionThreshold;
                else if (material.materialType == MaterialType.Magnet)
                    return (material.conductivity * distanceFactor) > detectionThreshold && material.conductivity > 0.3f;
                return false;

            case SensorType.Capacitive:
                float capacitiveStrength = (material.dielectricConstant + 0.3f) * distanceFactor;
                return capacitiveStrength > detectionThreshold;

            case SensorType.Optical:
                float opticalStrength = material.reflectivity * distanceFactor;
                return opticalStrength > detectionThreshold && material.reflectivity > 0.15f && HasLineOfSight(material.transform);

            case SensorType.Ultrasonic:
                float ultrasonicStrength = (material.reflectivity + 0.4f) * distanceFactor;
                return ultrasonicStrength > detectionThreshold;

            case SensorType.HallEffect:
                if (material.materialType == MaterialType.Steel)
                    return material.isMagnetic && distanceFactor > detectionThreshold;
                else if (material.materialType == MaterialType.Magnet)
                    return distanceFactor > detectionThreshold;
                return false;

            default:
                return false;
        }
    }

    bool HasLineOfSight(Transform target)
    {
        Vector3 detectionDirection = GetDetectionDirection();
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(detectionDirection, directionToTarget);
        if (dotProduct < 0.7f) return false;

        float distance = Vector3.Distance(transform.position, target.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, detectionDirection, out hit, distance))
        {
            return hit.transform == target;
        }
        return false;
    }

    void HandleDetectionChange()
    {
        if (isDetecting && !previousDetectionState)
        {
            TriggerActuator();
        }

        previousDetectionState = isDetecting;
    }

    void TriggerActuator()
    {
        if (connectedActuator != null)
        {
            connectedActuator.StepMotor(stepsOnDetection);
            if (debugMode)
            {
                Debug.Log($"Actuator triggered: {stepsOnDetection} steps");
            }
        }
    }

    void UpdateVisualFeedback()
    {
        if (sensorRenderer != null)
        {
            sensorRenderer.material.color = isDetecting ? detectionColor : originalColor;
        }
    }

    void OnDrawGizmos()
    {
        if (!enableDetection) return;

        Vector3 detectionDirection = GetDetectionDirection();
        Vector3 endPoint = transform.position + (detectionDirection * detectionRange);

        Gizmos.color = isDetecting ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, endPoint);
        Gizmos.DrawWireSphere(endPoint, 0.2f);

        if (isDetecting && currentMaterial != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentMaterial.transform.position);
        }
    }

    // Public methods
    public void EnableDetection()
    {
        enableDetection = true;
        if (debugMode) Debug.Log($"{sensorType} sensor detection enabled");
    }

    public void DisableDetection()
    {
        enableDetection = false;
        if (debugMode) Debug.Log($"{sensorType} sensor detection disabled");
    }

    public bool IsDetecting() => isDetecting;
    public MaterialType GetDetectedMaterial() => currentMaterial?.materialType ?? MaterialType.Air;
}
