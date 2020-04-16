using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBlast : MonoBehaviour
{
    public GameObject fireballImpact;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SkullBlast>().fireballImpact.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
