using UnityEngine;

public class FeedCylinderExtend : MonoBehaviour
{
    public enum ExtensionDirection
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
    [SerializeField] private ExtensionDirection extensionDirection = ExtensionDirection.Left;

    [Header("State")]
    [SerializeField] private bool isRunning = false;
    private bool isMoving = false;
    private bool extend;

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
        float maxReturnPosition = homePosition.y - moveDistance + returnThreshold;
        extend = transform.position.y >= maxReturnPosition ? isRunning : false;

        if (DetectPositiveEdge() && !isMoving && extend)
        {
            StartExtension();
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            UpdatePosition();
        }
    }

    private void StartExtension()
    {
        Vector3 direction = GetDirectionVector(extensionDirection);
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

    private Vector3 GetDirectionVector(ExtensionDirection dir)
    {
        return dir switch
        {
            ExtensionDirection.Left => Vector3.left,
            ExtensionDirection.Right => Vector3.right,
            ExtensionDirection.Up => Vector3.up,
            ExtensionDirection.Down => Vector3.down,
            ExtensionDirection.Forward => Vector3.forward,
            ExtensionDirection.Backward => Vector3.back,
            _ => Vector3.left,

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
