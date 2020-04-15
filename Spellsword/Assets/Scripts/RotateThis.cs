using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateThis : MonoBehaviour
{

    public float RotationSpeedX;
    public float RotationSpeedY;
    public float RotationSpeedZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * (RotationSpeedZ * Time.deltaTime));
        transform.Rotate(Vector3.up * RotationSpeedY * Time.deltaTime);
        transform.Rotate(Vector3.left * RotationSpeedX * Time.deltaTime);
    }
}
