using UnityEngine;
public class SpinController : MonoBehaviour
{
    [System.Serializable]
    public class SpinObject
    {
        public Transform objectToSpin; // The object to spin
        public Vector3 rotationAxis = Vector3.up; // Axis of rotation for this object
        public float spinSpeed = 100f; // Individual spin speed for this object
    }

    [Header("Spin Settings")]
    public SpinObject[] objectsToSpin; // Array of objects and their rotation axes
    public float globalSpinSpeed = 100f; // Global speed multiplier
    public bool isSpinning = false; // Whether objects are currently spinning

    void Update()
    {
        if (isSpinning)
        {
            // Rotate each object around its specified axis at its individual speed
            foreach (SpinObject spinObject in objectsToSpin)
            {
                spinObject.objectToSpin.Rotate(spinObject.rotationAxis,
                    spinObject.spinSpeed * globalSpinSpeed * Time.deltaTime * 0.01f);
            }
        }
    }

    public void StartSpinning()
    {
        isSpinning = true;
        Debug.Log("Spinning Started!");
    }

    public void StopSpinning()
    {
        isSpinning = false;
        Debug.Log("Spinning Stopped!");
    }

    public void SetGlobalSpinSpeed(float speed)
    {
        globalSpinSpeed = speed;
        Debug.Log("Global Spin Speed Set to: " + speed);
    }

    public void SetObjectSpinSpeed(int objectIndex, float speed)
    {
        if (objectIndex >= 0 && objectIndex < objectsToSpin.Length)
        {
            objectsToSpin[objectIndex].spinSpeed = speed;
            Debug.Log("Object " + objectIndex + " Spin Speed Set to: " + speed);
        }
        else
        {
            Debug.LogError("Invalid object index: " + objectIndex);
        }
    }
}