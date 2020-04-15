using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    [SerializeField]
    GameObject pageFront, pageBack;

    public GameObject PageFront
    {
        get { return pageFront; }
        set { pageFront = value; }
    }
    public GameObject PageBack
    {
        get { return pageBack; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
