using UnityEngine;

public class HallSensor : MonoBehaviour
{
    [Header("Sensor State")]
    public bool magneticDetected = false;
    private GlowOnEvent glow;
    public GameObject led;

    private void Start(){


        glow = led?.GetComponent<GlowOnEvent>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MagneticField"))
        {
            magneticDetected = true;
            glow.StartGlow();
            Debug.Log("HallSensor: Magnetic field detected.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MagneticField"))
        {
            magneticDetected = false;
            glow.StopGlow();
            Debug.Log("HallSensor: Magnetic field no longer detected.");
        }
    }
}
