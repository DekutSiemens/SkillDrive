using UnityEngine;

public class Range : MonoBehaviour
{
    public float ray_length  = 0.4f;
    public  Vector3 ray_dir = Vector3.up;
    public float distance;
    
    // Update is called once per frame
    void Update()
    {
        Ray ray = new(transform.position, transform.TransformDirection(ray_dir));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, ray_length)){
            if(hit.collider.name == "carrie"){
                Debug.Log(hit.collider.name);
                Debug.Log(hit.distance);
                distance = hit.distance;
            }
            
        }

        Debug.DrawRay(transform.position, transform.transform.TransformDirection(ray_dir) * ray_length, Color.red);
    }
}
