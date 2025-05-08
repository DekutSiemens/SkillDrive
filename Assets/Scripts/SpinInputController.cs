using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpinInputController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField1; // First input field for spin speed
    public TMP_InputField inputField2; // Second input field for spin speed
    public Button startButton;        // Button to start spinning
    public Button stopButton;         // Button to stop spinning
    public Button resetButton;        // Button to reset rotations

    [Header("Spin Controller Reference")]
    public SpinController spinController; // Reference to your existing SpinController

    private Quaternion[] initialRotations; // To store initial rotations for reset

    void Start()
    {
        // Store initial rotations
        initialRotations = new Quaternion[spinController.objectsToSpin.Length];
        for (int i = 0; i < spinController.objectsToSpin.Length; i++)
        {
            initialRotations[i] = spinController.objectsToSpin[i].objectToSpin.rotation;
        }

        // Set up button click listeners
        startButton.onClick.AddListener(StartSpinning);
        stopButton.onClick.AddListener(StopSpinning);
        resetButton.onClick.AddListener(ResetRotations);

        // Set up input field validation
        inputField1.contentType = TMP_InputField.ContentType.DecimalNumber;
        inputField2.contentType = TMP_InputField.ContentType.DecimalNumber;

        // Optional: Add listeners to update speeds when input changes
        inputField1.onEndEdit.AddListener((value) => UpdateSpinSpeeds());
        inputField2.onEndEdit.AddListener((value) => UpdateSpinSpeeds());
    }

    public void UpdateSpinSpeeds()
    {
        // Parse input values and update spin speeds
        if (float.TryParse(inputField1.text, out float speed1))
        {
            spinController.SetObjectSpinSpeed(0, speed1);
        }

        if (float.TryParse(inputField2.text, out float speed2))
        {
            spinController.SetObjectSpinSpeed(1, speed2);
        }
    }

    public void StartSpinning()
    {
        // Update speeds from input fields before starting
        UpdateSpinSpeeds();
        spinController.StartSpinning();
    }

    public void StopSpinning()
    {
        spinController.StopSpinning();
    }

    public void ResetRotations()
    {
        spinController.StopSpinning();

        // Reset each object to its initial rotation
        for (int i = 0; i < spinController.objectsToSpin.Length; i++)
        {
            if (i < initialRotations.Length)
            {
                spinController.objectsToSpin[i].objectToSpin.rotation = initialRotations[i];
            }
        }
    }
}