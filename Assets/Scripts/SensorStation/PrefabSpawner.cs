using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float scaleMultiplier = 0.001f; // Single value to control overall scale

    public void Spawn()
    {
        if(prefab != null)
        {
            // Instantiate the prefab at the transform's position and rotation
            GameObject spawnedObject = Instantiate(prefab, transform.position, transform.rotation);
            
            // Get the original scale and multiply it by our scale multiplier
            Vector3 originalScale = spawnedObject.transform.localScale;
            Vector3 newScale = originalScale * scaleMultiplier;
            
            // Apply the new scale
            spawnedObject.transform.localScale = newScale;
        }
    }
}