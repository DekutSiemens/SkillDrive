using UnityEngine;

public class RotateWithSelectableAxis : MonoBehaviour
{
    // Enum for rotation modes
    public enum RotationMode
    {
        UsePredefinedAxis,
        UseAxisFromObject
    }

    // Public variables to configure rotation behavior in the inspector
    public RotationMode rotationMode = RotationMode.UsePredefinedAxis;

    [Header("Predefined Axis (Only used if 'UsePredefinedAxis' is selected)")]
    public Vector3 predefinedAxis = Vector3.up;

    [Header("Axis From Object (Only used if 'UseAxisFromObject' is selected)")]
    public GameObject axisObject;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;

    // Private flag to control if the object is rotating
    private bool isRotating = false;

    // Audio-related variables
    public AudioClip rotationClip;  // Single sound for both start and stop
    private AudioSource audioSource; // AudioSource to control playback

    void Start()
    {
        // Initialize the AudioSource component on the GameObject this script is attached to
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // If the object is not rotating, skip the update
        if (!isRotating) return;

        // Get the rotation axis based on the selected mode
        Vector3 axis = GetRotationAxis();
        // Rotate the object around the selected axis at the specified speed
        transform.Rotate(axis, rotationSpeed * Time.deltaTime, Space.World);
    }

    // Method to determine the axis of rotation
    Vector3 GetRotationAxis()
    {
        // If using the axis from the object, return its forward direction
        if (rotationMode == RotationMode.UseAxisFromObject && axisObject != null)
        {
            return axisObject.transform.forward.normalized;
        }

        // Otherwise, return the predefined axis
        return predefinedAxis.normalized;
    }

    // Method to start rotating the object and play the sound
    public void StartRotating()
    {
        isRotating = true;

        // Play the sound if it's set and the AudioSource exists (but do not loop)
        if (rotationClip != null && audioSource != null)
        {
            audioSource.clip = rotationClip; // Set the sound clip
            audioSource.loop = true;         // Loop the audio while rotating
            audioSource.Play();              // Play the sound
        }
    }

    // Method to stop rotating the object and stop the sound
    public void StopRotating()
    {
        isRotating = false;

        // Stop the sound if it's set and the AudioSource exists
        if (rotationClip != null && audioSource != null)
        {
            audioSource.loop = false;        // Stop looping
            audioSource.Stop();              // Stop the sound
        }
    }
}
