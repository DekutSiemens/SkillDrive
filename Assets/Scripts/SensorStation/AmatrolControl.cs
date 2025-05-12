using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the Amatrol station automation including pick and place operations
/// </summary>
public class AmatrolControl : MonoBehaviour
{
    [SerializeField, Tooltip("Enable to start the pick and place process")]
    private bool startPickAndPlace;

    private PickandPlace _pickAndPlace;
    private bool _isProcessRunning;
    private Coroutine _activeProcess;

    // Public events
    public event Action ProcessStarted;
    public event Action ProcessCompleted;
    public event Action ProcessFailed;

    /// <summary>
    /// Initializes the Amatrol control system
    /// </summary>
    private void Start()
    {
        try
        {
            // Initialize the pick and place system
            _pickAndPlace = new PickandPlace(this.gameObject);
            
            // Subscribe to completion event
            _pickAndPlace.Process.ProcessingCompleted += OnPickAndPlaceCompleted;
            
            Debug.Log("AmatrolControl initialized successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize AmatrolControl: {ex.Message}");
        }
    }

    public void Start_p(){
        startPickAndPlace = true;
    }

    public void Stop_p(){
        startPickAndPlace = false;
    }

    /// <summary>
    /// Main update loop to check for process start/stop conditions
    /// </summary>
    private void Update()
    {
        // Check for changes in the start flag
        if (startPickAndPlace && !_isProcessRunning)
        {
            StartPickAndPlaceProcess();
        }
        else if (!startPickAndPlace && _isProcessRunning)
        {
            StopPickAndPlaceProcess();
        }
    }

    /// <summary>
    /// Starts the pick and place process 
    /// </summary>
    public void StartPickAndPlaceProcess()
    {
        if (_isProcessRunning)
        {
            Debug.Log("Pick and place process already running");
            return;
        }

        _activeProcess = StartCoroutine(PickAndPlaceProcess());
    }

    /// <summary>
    /// Stops the active pick and place process if one is running
    /// </summary>
    public void StopPickAndPlaceProcess()
    {
        if (!_isProcessRunning || _activeProcess == null)
            return;
            
        StopCoroutine(_activeProcess);
        _activeProcess = null;
        _isProcessRunning = false;
        
        Debug.Log("Pick and place process stopped");
    }

    /// <summary>
    /// Handler for pick and place completion event
    /// </summary>
    private void OnPickAndPlaceCompleted()
    {
        Debug.Log("Pick and place process completed");
        _isProcessRunning = false;
        _activeProcess = null;
        
        // Auto-reset the flag to allow for easy restart
        startPickAndPlace = false;
        
        // Notify listeners of completion
        ProcessCompleted?.Invoke();
    }

    /// <summary>
    /// Executes the pick and place process workflow
    /// </summary>
    private IEnumerator PickAndPlaceProcess()
    {
        _isProcessRunning = true;
        
        Debug.Log("Starting pick and place process");
        ProcessStarted?.Invoke();
        
        // Execute the pick and place sequence
        yield return StartCoroutine(SafeCoroutine(_pickAndPlace.Process.ProcessWorkpiece()));
        
        // Safety check to ensure state is reset if the completion event wasn't triggered
        if (_isProcessRunning)
        {
            _isProcessRunning = false;
            startPickAndPlace = false;
        }
    }
    
    /// <summary>
    /// A wrapper to safely execute coroutines with error handling
    /// </summary>
    /// <param name="routine">The coroutine to execute safely</param>
    private IEnumerator SafeCoroutine(IEnumerator routine)
    {
        while (true)
        {
            object current;
            
            // Move to the next step and capture any exceptions
            try
            {
                if (!routine.MoveNext())
                {
                    yield break;
                }
                
                current = routine.Current;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in pick and place process: {ex.Message}");
                ProcessFailed?.Invoke();
                yield break;
            }
            
            // Yield the current value
            yield return current;
        }
    }

    /// <summary>
    /// Cleanup when this component is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (_pickAndPlace != null)
        {
            _pickAndPlace.Process.ProcessingCompleted -= OnPickAndPlaceCompleted;
        }
    }
}