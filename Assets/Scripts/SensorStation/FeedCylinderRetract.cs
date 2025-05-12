using UnityEngine;

public class FeedCylinderRetract : MonoBehaviour
{
    public enum RetractionDirection
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Backward
    }

    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 0.08f;
    [SerializeField] private float moveDistance = 0.06f;
    [SerializeField] private float returnThreshold = 0.02f;
    [SerializeField] private RetractionDirection retractionDirection = RetractionDirection.Right;

    [Header("State")]
    [SerializeField] private bool isRunning = false;
    private bool isMoving = false;
    private bool retract;

    private Vector3 homePosition;
    private Vector3 targetPosition;

    private bool previousValue;
    private bool currentValue;
    public AudioClip clip;
    private AudioSource source;

    private Rigidbody rb;

    void Start()
    {
        homePosition = transform.position;
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Ensure this is set in inspector or here
    }

    void Update()
    {
        float minRetractPosition = homePosition.y + moveDistance - returnThreshold;
        retract = transform.position.y <= minRetractPosition ? isRunning : false;

        if (DetectPositiveEdge() && !isMoving && retract)
        {
            StartRetraction();
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            UpdatePosition();
        }
    }

    private void StartRetraction()
    {
        Vector3 direction = GetDirectionVector(retractionDirection);
        targetPosition = transform.position + direction * moveDistance;
        isMoving = true;
    }

    private void UpdatePosition()
    {
        rb.MovePosition(Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        ));

        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    private bool DetectPositiveEdge()
    {
        currentValue = retract;
        bool trigger = !previousValue && currentValue;
        previousValue = currentValue;
        return trigger;
    }

    private Vector3 GetDirectionVector(RetractionDirection dir)
    {
        return dir switch
        {
            RetractionDirection.Left => Vector3.left,
            RetractionDirection.Right => Vector3.right,
            RetractionDirection.Up => Vector3.up,
            RetractionDirection.Down => Vector3.down,
            RetractionDirection.Forward => Vector3.forward,
            RetractionDirection.Backward => Vector3.back,
            _ => Vector3.right,
        };
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
