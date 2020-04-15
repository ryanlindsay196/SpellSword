using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialSelectorAspectRatioManager : MonoBehaviour
{
    [SerializeField]
    RectTransform runeSelection;

    Vector2 runeSelectionInitialPosition;
    Vector2 initialScreenSize;
    // Start is called before the first frame update
    void Start()
    {
        initialScreenSize = new Vector2(Screen.width, Screen.height);
        runeSelectionInitialPosition = runeSelection.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(runeSelection.localScale + "::::" + (float)((float)Screen.width / (float)Screen.height));
        float poop = (float)((float)Screen.width / (float)Screen.height);
        switch ((float)((float)Screen.width / (float)Screen.height))
        {
            case 5f / 4f:
                runeSelection.localScale = new Vector2(1.25f, 1.305f);
                break;
            case 16f / 9f:
            case 1.779412f:
                runeSelection.localScale = new Vector2(1f, 1f);
                break;
            case 16f / 10f:
                runeSelection.localScale = new Vector2(1.07f, 1.08f);
                break;
            case 4f / 3f:
                runeSelection.localScale = new Vector2(1.202f, 1.24f);
                break;
            case 3f / 2f:
                runeSelection.localScale = new Vector2(1.1f, 1.14f);
                break;
            default:
                runeSelection.localScale = new Vector2(1f, 1f);
                break;
        }
        //runeSelection.sizeDelta = runeSelectionInitialPosition * (new Vector2(Screen.width, Screen.height) / initialScreenSize);
        //runeSelection.sizeDelta
        //Debug.Log("Rune selection position: " + runeSelection.anchoredPosition);
   }
}
