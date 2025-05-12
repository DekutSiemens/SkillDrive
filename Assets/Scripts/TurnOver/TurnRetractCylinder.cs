using UnityEngine;

public class TurnRetractCylinder : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 0.08f;
    [SerializeField] private float moveDistance = 0.06f;
    [SerializeField] private float returnThreshold = 0.06f; // Distance threshold for cylinder return

    [Header("State")]
    [SerializeField] private bool isRunning = false;
    private bool isMoving = false;
    private bool extend;

    // Position tracking
    private Vector3 homePosition;
    private Vector3 targetPosition;

    // Edge detection
    private bool previousValue;
    private bool currentValue;
    public AudioClip clip;
    private AudioSource source;

    void Start()
    {
        homePosition = transform.position;
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check if cylinder should start retracting
        float minReturnPosition = homePosition.y + moveDistance - returnThreshold;
        extend = transform.position.y <= minReturnPosition ? isRunning : false;

        // Handle movement on positive edge trigger
        if (DetectPositiveEdge() && !isMoving && extend)
        {
            StartRetraction();
        }

        if (isMoving)
        {
            UpdatePosition();
        }
    }

    private void StartRetraction()
    {
        targetPosition = transform.position + Vector3.up * moveDistance;
        isMoving = true;
    }

    private void UpdatePosition()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    private bool DetectPositiveEdge()
    {
        currentValue = extend;
        bool trigger = !previousValue && currentValue;
        previousValue = currentValue;
        return trigger;
    }

    public void StartCylinder()
    {
        isRunning = true;
        source.Play();
    }

    public void StopCylinder()
    {
        isRunning = false;
        source.Stop();
    }
}