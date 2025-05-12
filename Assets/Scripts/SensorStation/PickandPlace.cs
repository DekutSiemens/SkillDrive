using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles object pick and place operations with sensor-based detection
/// </summary>
public class PickandPlace
{
    public RunPickandPlace Process { get; private set; }

    /// <summary>
    /// Initializes a new instance of the PickandPlace class
    /// </summary>
    /// <param name="parent">Parent GameObject that contains or references the sensors</param>
    /// <exception cref="ArgumentNullException">Thrown when parent is null</exception>
    public PickandPlace(GameObject parent)
    {
        if (parent == null)
            throw new ArgumentNullException(nameof(parent), "Parent GameObject cannot be null");
            
        Process = new RunPickandPlace(parent);
    }

    /// <summary>
    /// Manages the run-time processing of pick and place operations
    /// </summary>
    public class RunPickandPlace
    {
        // Sensors
        private readonly PhotoelectricSensor _entrySensor;
        private readonly PhotoelectricSensor _atPlaceSensor;
        private readonly PhotoelectricSensor _exitSensor;
        private FeedCylinderExtend feedcylinder_extend;
        private FeedCylinderRetract feedcylinder_retract;
        private TurnExtendCylinder pickupcylinder_extend;
        private TurnRetractCylinder pickupcylinder_retract;

        private FeedCylinderExtend tranversecylinder_extend;
        private FeedCylinderRetract tranversecylinder_retract;

        private PrefabSpawner spawner;


        
        // Parent reference for context
        private readonly GameObject _parent;
        
        // Public properties for sensor states
        public bool EntryPos { get; private set; }
        public bool AtPlacePos { get; private set; }
        public bool ExitPos { get; private set; }
        
        // Processing state
        private bool _isProcessing;
        
        // Event to notify when processing completes
        public event Action ProcessingCompleted;

        /// <summary>
        /// Initializes a new RunPickandPlace instance
        /// </summary>
        /// <param name="parent">Parent GameObject containing sensor references</param>
        /// <exception cref="InvalidOperationException">Thrown when sensors cannot be found</exception>
        public RunPickandPlace(GameObject parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            
            // Initialize sensors directly in the constructor
            //_entrySensor = GameObject.Find("EntryPosition")?.GetComponent<PhotoelectricSensor>();
            //_atPlaceSensor = GameObject.Find("AtPlacePosition")?.GetComponent<PhotoelectricSensor>();
            //_exitSensor = GameObject.Find("ExitPosition")?.GetComponent<PhotoelectricSensor>();

            feedcylinder_extend = GameObject.Find("feed_piston")?.GetComponent<FeedCylinderExtend>();
            feedcylinder_retract = GameObject.Find("feed_piston")?.GetComponent<FeedCylinderRetract>();

            pickupcylinder_extend= GameObject.Find("pickupCylinder")?.GetComponent<TurnExtendCylinder>();
            pickupcylinder_retract = GameObject.Find("pickupCylinder")?.GetComponent<TurnRetractCylinder>();

            tranversecylinder_extend = GameObject.Find("transverseCylinder")?.GetComponent<FeedCylinderExtend>();
            tranversecylinder_retract = GameObject.Find("transverseCylinder")?.GetComponent<FeedCylinderRetract>();
            
            spawner = GameObject.Find("blackhole")?.GetComponent<PrefabSpawner>();
            // Validate that all sensors are properly found
            /*
            if (_entrySensor == null)
                throw new InvalidOperationException("EntryPosition sensor not found");
                
            if (_atPlaceSensor == null)
                throw new InvalidOperationException("AtPlacePosition sensor not found");
                
            if (_exitSensor == null)
                throw new InvalidOperationException("ExitPosition sensor not found");
            
            // Initial sensor state update
            UpdateSensorStates();
            */
        }
        
        /// <summary>
        /// Updates internal state based on current sensor readings
        /// </summary>
        public void UpdateSensorStates()
        {
            // Check if objects are detected by evaluating if GetDetectedObject returns a non-null value
            EntryPos = _entrySensor.GetDetectedObject() != null;
            AtPlacePos = _atPlaceSensor.GetDetectedObject() != null;
            ExitPos = _exitSensor.GetDetectedObject() != null;
        }

        /// <summary>
        /// Gets the object detected at the entry position, if any
        /// </summary>
        /// <returns>Detected GameObject or null if none</returns>
        public GameObject GetEntryObject() => _entrySensor.GetDetectedObject();

        /// <summary>
        /// Gets the object at the placement position, if any
        /// </summary>
        /// <returns>Detected GameObject or null if none</returns>
        public GameObject GetPlaceObject() => _atPlaceSensor.GetDetectedObject();

        /// <summary>
        /// Gets the object at the exit position, if any
        /// </summary>
        /// <returns>Detected GameObject or null if none</returns>
        public GameObject GetExitObject() => _exitSensor.GetDetectedObject();

        /// <summary>
        /// Processes a workpiece through the pick and place sequence
        /// </summary>
        /// <returns>Coroutine IEnumerator</returns>
        public IEnumerator ProcessWorkpiece()
        {
            // Check if already processing
            if (_isProcessing)
            {
                Debug.Log("Already processing a workpiece. Skipping.");
                yield break;
            }
            
            _isProcessing = true;
            feedcylinder_extend.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            feedcylinder_extend.StopCylinder();
            
            /*********** extend pickup cylinder *******************************/
            yield return new WaitForSeconds(1.0f);
            
            feedcylinder_retract.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            feedcylinder_retract.StopCylinder();

            yield return new WaitForSeconds(1.0f);
            pickupcylinder_extend.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            pickupcylinder_extend.StopCylinder();

            ToggleMeshRenderer("workpiece", false);
            ToggleMeshRenderer("workpiece (1)", true);
            yield return new WaitForSeconds(2.0f);
            pickupcylinder_retract.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            pickupcylinder_retract.StopCylinder();
            ToggleRididbody("pickupCylinder", true);

            yield return new WaitForSeconds(2.0f);
            tranversecylinder_extend.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            tranversecylinder_extend.StopCylinder();

            yield return new WaitForSeconds(5.0f);
            pickupcylinder_extend.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            pickupcylinder_extend.StopCylinder();

            ToggleMeshRenderer("workpiece", true);
            ToggleMeshRenderer("workpiece (1)", false);
            //ToggleMeshRenderer("workpiece_s", true);
            //ToggleMeshGravity("workpiece_s");
            ToggleMeshRenderer("workpiece (2)", true);
            //spawner.Spawn();

            yield return new WaitForSeconds(2.0f);
            pickupcylinder_retract.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            pickupcylinder_retract.StopCylinder();

            yield return new WaitForSeconds(2.0f);
            tranversecylinder_retract.StartCylinder();
            yield return new WaitForSeconds(2.0f);
            tranversecylinder_retract.StopCylinder();
            
            _isProcessing = false;

        }
        public void ToggleRididbody(string traget_name, bool isEnabled){

            GameObject traget = GameObject.Find(traget_name);

            Rigidbody rb = traget.GetComponent<Rigidbody>();
            if( traget != null){
                if(rb != null){
                    rb.isKinematic = !isEnabled;

            }
            }
        }

        public void ToggleMeshRenderer(string name, bool enable){
            GameObject  obj = GameObject.Find(name);
            if(obj != null){
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if(renderer != null){
                    renderer.enabled = enable;

                }
            }
        }

        public void ToggleMeshGravity(string name){
            GameObject  obj = GameObject.Find(name);
            if(obj != null){
                obj.AddComponent<Rigidbody>();
            }
        }
        
        /// <summary>
        /// Checks if the system is currently processing a workpiece
        /// </summary>
        /// <returns>True if processing, false otherwise</returns>
        public bool IsProcessing() => _isProcessing;
    }
}