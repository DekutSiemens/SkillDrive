using UnityEngine;
using System.Collections;

public enum RotationAxis
{
    X,
    Y,
    Z
}

public class StepperMotor : MonoBehaviour
{
    [Header("Stepper Motor Settings")]
    public int stepsPerRevolution = 200;  // Common stepper motor: 200 steps = 360°
    public float stepInterval = 0.5f;  // Time between steps in seconds
    public bool autoStart = true;  // Start stepping automatically
    public RotationAxis rotationAxis = RotationAxis.Z;  // Axis to rotate around

    [Header("Audio Settings")]
    public AudioSource motorSoundSource;  // AudioSource for motor sound
    public AudioClip stepSound;  // Sound to play for each step
    public bool playStepSound = true;  // Enable/disable step sounds
    [Range(0.1f, 2f)] public float soundPitch = 1f;  // Pitch of the motor sound

    private float stepAngle;  // Calculated angle per step
    private bool isRotating = false;
    private bool isStepping = false;  // Prevent multiple step operations

    void Start()
    {
        // Calculate the angle per step based on steps per revolution
        stepAngle = 360f / stepsPerRevolution;

        // Setup audio source if not assigned
        if (motorSoundSource == null)
        {
            motorSoundSource = GetComponent<AudioSource>();
            if (motorSoundSource == null)
            {
                // Create AudioSource if none exists
                motorSoundSource = gameObject.AddComponent<AudioSource>();
                motorSoundSource.playOnAwake = false;
                motorSoundSource.loop = false;
            }
        }

        // Set initial pitch
        if (motorSoundSource != null)
        {
            motorSoundSource.pitch = soundPitch;
        }

        if (autoStart)
        {
            StartStepping();
        }
    }

    public void StartStepping()
    {
        if (!isRotating)
        {
            StartCoroutine(StepMotor());
        }
    }

    public void StopStepping()
    {
        isRotating = false;
        isStepping = false;  // Also stop specific stepping
    }

    // Method to step a specific number of steps (for sensor triggering)
    public void StepMotor(int numberOfSteps)
    {
        // Prevent multiple simultaneous stepping operations
        if (!isStepping)
        {
            StartCoroutine(StepSpecificSteps(numberOfSteps));
        }
    }

    private IEnumerator StepSpecificSteps(int steps)
    {
        isStepping = true;  // Mark as stepping

        for (int i = 0; i < steps; i++)
        {
            // Play step sound
            PlayStepSound();

            Vector3 rotationVector = GetStepRotationVector();
            transform.Rotate(rotationVector, Space.Self);
            yield return new WaitForSeconds(stepInterval);
        }

        isStepping = false;  // Mark as finished stepping
    }

    private IEnumerator StepMotor()
    {
        isRotating = true;
        while (isRotating)
        {
            // Play step sound
            PlayStepSound();

            // Get rotation vector based on selected axis and calculated step angle
            Vector3 rotationVector = GetStepRotationVector();
            // Rotate the stepper motor by one step around the selected local axis
            transform.Rotate(rotationVector, Space.Self);
            // Wait for the specified step interval
            yield return new WaitForSeconds(stepInterval);
        }
    }

    private void PlayStepSound()
    {
        if (playStepSound && motorSoundSource != null)
        {
            if (stepSound != null)
            {
                // Play the assigned step sound
                motorSoundSource.PlayOneShot(stepSound);
            }
            else
            {
                // Play default sound if no clip assigned
                motorSoundSource.Play();
            }
        }
    }

    private Vector3 GetStepRotationVector()
    {
        switch (rotationAxis)
        {
            case RotationAxis.X:
                return new Vector3(stepAngle, 0, 0);
            case RotationAxis.Y:
                return new Vector3(0, stepAngle, 0);
            case RotationAxis.Z:
                return new Vector3(0, 0, stepAngle);
            default:
                return new Vector3(0, 0, stepAngle);
        }
    }

    // Public methods for sound control
    public void SetMotorPitch(float pitch)
    {
        soundPitch = Mathf.Clamp(pitch, 0.1f, 2f);
        if (motorSoundSource != null)
        {
            motorSoundSource.pitch = soundPitch;
        }
    }

    public void ToggleStepSound(bool enable)
    {
        playStepSound = enable;
    }
}