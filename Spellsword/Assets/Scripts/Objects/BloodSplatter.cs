using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDelete", 0.5f);
    }

    void SelfDelete()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
