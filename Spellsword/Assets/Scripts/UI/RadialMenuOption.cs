using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuOption : MonoBehaviour
{
    bool isSelected;
    public bool IsSelected
    {
        set { isSelected = value; }
    }

    float sizeMultiplier;
    public float SizeMultiplier
    {
        set { sizeMultiplier = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isSelected)
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(GetComponent<RectTransform>().sizeDelta, new Vector2(150f, 150f), 0.5f) * sizeMultiplier;
        }
        else
        {

            GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100) * sizeMultiplier;
        }
    }
}
