using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    [Header("Sensor Configuration")]
    public SensorType sensorType = SensorType.Inductive;
    public float detectionRange = 5f;
    public LayerMask detectionLayer = -1;

    [Header("Sensor Specific Settings")]
    [Range(0f, 1f)] public float sensitivity = 0.5f;
    public bool debugMode = true;

    [Header("Actuator Connection")]
    public StepperMotor connectedActuator;
    public int stepsOnDetection = 10;

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
        PerformDetection();
        HandleDetectionChange();
        UpdateVisualFeedback();
    }

    void PerformDetection()
    {
        // Cast a sphere to detect objects in range
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRange, detectionLayer);

        bool materialDetected = false;
        MaterialProperties closestMaterial = null;
        float closestDistance = float.MaxValue;

        foreach (Collider obj in objectsInRange)
        {
            MaterialProperties material = obj.GetComponent<MaterialProperties>();
            if (material != null)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);

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

    bool CanDetectMaterial(MaterialProperties material, float distance)
    {
        // Distance affects detection strength
        float distanceFactor = 1f - (distance / detectionRange);

        switch (sensorType)
        {
            case SensorType.Inductive:
                // Detects conductive materials (metals)
                return (material.conductivity * distanceFactor) > sensitivity;

            case SensorType.Capacitive:
                // Detects materials with different dielectric properties
                return (material.dielectricConstant * distanceFactor) > sensitivity;

            case SensorType.Optical:
                // Detects based on reflectivity and line of sight
                return (material.reflectivity * distanceFactor) > sensitivity && HasLineOfSight(material.transform);

            case SensorType.Ultrasonic:
                // Detects solid objects based on sound reflection
                return (material.reflectivity * distanceFactor) > sensitivity;

            case SensorType.HallEffect:
                // Only detects magnetic materials
                return material.isMagnetic && distanceFactor > sensitivity;

            default:
                return false;
        }
    }

    bool HasLineOfSight(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            return hit.transform == target;
        }
        return false;
    }

    void HandleDetectionChange()
    {
        // Trigger actuator when detection state changes from false to true
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

    // Gizmos for visualization in scene view
    void OnDrawGizmos()
    {
        Gizmos.color = isDetecting ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (sensorType == SensorType.Optical && isDetecting && currentMaterial != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentMaterial.transform.position);
        }
    }

    // Public methods for testing
    public bool IsDetecting() => isDetecting;
    public MaterialType GetDetectedMaterial() => currentMaterial?.materialType ?? MaterialType.Air;
}