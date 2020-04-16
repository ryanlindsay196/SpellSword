
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_RadialMenu : MonoBehaviour
{
    [SerializeField]
    RadialMenu radialMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().anchoredPosition = radialMenu.SelectionPosition;
    }
}
