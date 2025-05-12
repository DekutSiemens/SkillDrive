using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TwoPointLine : MonoBehaviour
{
    public Transform pointA;
    public Transform pointC;
    public Transform pointB;
    private LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        line.positionCount = 3;
        line.SetPosition(0, pointA.position);
        line.SetPosition(1, pointB.position);
        line.SetPosition(2, pointC.position);
    }
}
