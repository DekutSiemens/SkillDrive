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

    private float stepAngle;  // Calculated angle per step

    private bool isRotating = false;

    void Start()
    {
        // Calculate the angle per step based on steps per revolution
        stepAngle = 360f / stepsPerRevolution;

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
    }

    // Method to step a specific number of steps (for sensor triggering)
    public void StepMotor(int numberOfSteps)
    {
        StartCoroutine(StepSpecificSteps(numberOfSteps));
    }

    private IEnumerator StepSpecificSteps(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            Vector3 rotationVector = GetStepRotationVector();
            transform.Rotate(rotationVector, Space.Self);
            yield return new WaitForSeconds(stepInterval);
        }
    }

    private IEnumerator StepMotor()
    {
        isRotating = true;

        while (isRotating)
        {
            // Get rotation vector based on selected axis and calculated step angle
            Vector3 rotationVector = GetStepRotationVector();

            // Rotate the stepper motor by one step around the selected local axis
            transform.Rotate(rotationVector, Space.Self);

            // Wait for the specified step interval
            yield return new WaitForSeconds(stepInterval);
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
}