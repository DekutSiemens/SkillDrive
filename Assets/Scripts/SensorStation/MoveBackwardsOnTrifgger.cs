using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveBackwardsOnTrigger : MonoBehaviour
{
    public float speed = 0.2f;
    private bool isTriggered = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() // Use FixedUpdate for physics-based movement
    {
        if (isTriggered)
        {
            Vector3 movement = Vector3.right * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    public void StartMoving()
    {
        isTriggered = true;
    }

    public void StopMoving()
    {
        isTriggered = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        StopMoving();
    }
}
