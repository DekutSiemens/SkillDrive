using UnityEngine;

public class TurnExtendCylinder : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 0.08f;
    [SerializeField] private float moveDistance = 0.1f;
    [SerializeField] private float returnThreshold = 0.02f; // Distance threshold for cylinder return

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
        // Check if cylinder should start extending
        float maxReturnPosition = homePosition.y - moveDistance + returnThreshold;
        extend = transform.position.y >= maxReturnPosition ? isRunning : false;

        // Handle movement on positive edge trigger
        if (DetectPositiveEdge() && !isMoving && extend)
        {
            StartExtension();
        }

        if (isMoving)
        {
            UpdatePosition();
        }
    }

    private void StartExtension()
    {
        targetPosition = transform.position + Vector3.down * moveDistance;
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