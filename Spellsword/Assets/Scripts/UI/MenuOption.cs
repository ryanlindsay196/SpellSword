using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuOption : MonoBehaviour
     , IPointerClickHandler // 2
     , IDragHandler
     , IPointerEnterHandler
     , IPointerExitHandler
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    float selectedSizeMultiplier;

    bool isHovered;
    public bool IsHovered { get { return isHovered; } }

    [SerializeField]
    Vector2 selectedPositionOffset;

    Vector2 initialSizeDelta;
    Vector2 initialAnchoredPosition;

    public void SetText(string inString)
    {
        text.text = inString;
        initialSizeDelta = GetComponent<RectTransform>().sizeDelta;
        initialAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public void MoveToSelectedPosition()
    {
        GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(GetComponent<RectTransform>().sizeDelta, initialSizeDelta * selectedSizeMultiplier, 0.3f);
        GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, initialAnchoredPosition + selectedPositionOffset, 0.4f);
    }

    public void StopSelected()
    {
        GetComponent<RectTransform>().sizeDelta = initialSizeDelta;
        GetComponent<RectTransform>().anchoredPosition = initialAnchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        Debug.Log("IsHovered: " + isHovered);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        Debug.Log("IsHovered: " + isHovered);
    }
}
