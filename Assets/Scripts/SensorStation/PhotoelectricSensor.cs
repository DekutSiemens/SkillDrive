using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Sensors/Photoelectric Sensor")]
public class PhotoelectricSensor : MonoBehaviour
{
    [Header("Sensor Configuration")]
    [Tooltip("Detection range in units")]
    [SerializeField] private float detectionRange = 1f;
    
    [Tooltip("Layers that the sensor will detect")]
    [SerializeField] private LayerMask detectionLayer;
    
    [Tooltip("Invert the detection direction")]
    [SerializeField] private bool invertDirection = false;

    [Header("Events")]
    [SerializeField] private UnityEvent onDetectionBegin;
    [SerializeField] private UnityEvent onDetectionEnd;

    // Cached transform direction
    private Vector3 detectionDirection;
    private Ray ray;
    private RaycastHit hit;
    private bool isDetecting;

    // Public read-only properties
    public bool IsDetecting => isDetecting;
    public float DetectionRange => detectionRange;

    private void Awake()
    {
        // Cache the ray origin
        ray = new Ray(transform.position, Vector3.forward);
    }

    private void FixedUpdate()
    {
        UpdateDetection();
    }

    private void UpdateDetection()
    {
        // Update ray parameters
        ray.origin = transform.position;
        ray.direction = invertDirection ? -transform.forward : transform.forward;

        // Check for detection
        bool wasDetecting = isDetecting;
        isDetecting = Physics.Raycast(ray, out hit, detectionRange, detectionLayer);

        // Handle detection state changes
        if (isDetecting != wasDetecting)
        {
            if (isDetecting)
            {
                onDetectionBegin?.Invoke();
            }
            else
            {
                onDetectionEnd?.Invoke();
            }
        }
    }

    // Public methods for external access
    public GameObject GetDetectedObject()
    {
        return isDetecting ? hit.collider.gameObject : null;
    }

    public float GetDetectedDistance()
    {
        return isDetecting ? hit.distance : -1f;
    }

#if UNITY_EDITOR
    // Editor-only visualization
    private void OnDrawGizmos()
    {
        Vector3 direction = invertDirection ? -transform.forward : transform.forward;
        
        // Detection range indicator
        Gizmos.color = Color.yellow * 0.5f;
        //Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Detection ray
        Gizmos.color = isDetecting ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, direction * detectionRange);
    }
#endif
}