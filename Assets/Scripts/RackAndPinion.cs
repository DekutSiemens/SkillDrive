using UnityEngine;

public class RackAndPinion : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed in units per second")]
    public float moveSpeed = 1f;

    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 90f;

    [Tooltip("Axis for movement (world space)")]
    public Vector3 moveAxis = Vector3.right;

    [Tooltip("Axis for rotation (local space)")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Collision Settings")]
    [Tooltip("Tags for the end stoppers")]
    public string leftStopperTag = "LeftStopper";
    public string rightStopperTag = "RightStopper";

    [Tooltip("Offset distance from stoppers")]
    public float stopOffset = 0.1f;

    private bool movingRight = true;
    private float currentMoveDistance;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Better for controlled movement
        }

        moveAxis = moveAxis.normalized;
        rotationAxis = rotationAxis.normalized;
    }

    void FixedUpdate()
    {
        // Calculate movement direction
        Vector3 movement = moveAxis * (movingRight ? 1 : -1) * moveSpeed * Time.fixedDeltaTime;

        // Apply movement and rotation
        if (rb.isKinematic)
        {
            // Precise movement for kinematic rigidbodies
            rb.MovePosition(rb.position + movement);
            rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(
                rotationSpeed * Time.fixedDeltaTime * (movingRight ? 1 : -1),
                transform.TransformDirection(rotationAxis)));
        }
        else
        {
            // Physics-based movement
            rb.linearVelocity = movement / Time.fixedDeltaTime;
            rb.angularVelocity = transform.TransformDirection(rotationAxis) *
                rotationSpeed * Mathf.Deg2Rad * (movingRight ? 1 : -1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check for stopper collisions
        if (other.CompareTag(leftStopperTag) && !movingRight)
        {
            movingRight = true;
            AdjustPositionAfterCollision(other);
        }
        else if (other.CompareTag(rightStopperTag) && movingRight)
        {
            movingRight = false;
            AdjustPositionAfterCollision(other);
        }
    }

    void AdjustPositionAfterCollision(Collider stopper)
    {
        if (rb.isKinematic)
        {
            // Calculate stopping point with offset
            Vector3 stopperPosition = stopper.ClosestPoint(transform.position);
            Vector3 stopPoint = stopperPosition - moveAxis * (movingRight ? -stopOffset : stopOffset);

            // Snap to corrected position
            rb.position = stopPoint;
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        // Draw movement axis
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveAxis * 1f);

        // Draw rotation axis
        Gizmos.color = Color.blue;
        Vector3 worldRotationAxis = transform.TransformDirection(rotationAxis);
        Gizmos.DrawLine(transform.position, transform.position + worldRotationAxis * 0.5f);
    }
}