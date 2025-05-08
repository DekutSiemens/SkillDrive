using UnityEngine;

public class GearController : MonoBehaviour
{
    public float rotationSpeed = 30f; // Degrees per second
    private float currentRotation = 0f;
    private bool isRotating = false;

    void Update()
    {
        if (isRotating)
        {
            currentRotation += rotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }

    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }

    public void ResetRotation()
    {
        currentRotation = 0f;
        transform.localRotation = Quaternion.identity;
    }
}