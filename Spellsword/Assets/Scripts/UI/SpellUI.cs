using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellUI : MonoBehaviour
{
    Vector2 initialPosition;
    [SerializeField]
    Vector2 positionOffsetWhenActive;

    [SerializeField]
    Image[] spellUIImage;

    bool MenuActive
    {
        get { return GameObject.FindObjectOfType<EquipmentManager>().GetCurrentEquipment.GetComponent<BookBehavior>() != null; }
    }

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    private void FixedUpdate()
    {
        //Vector2 spellUIrect = GetComponent<RectTransform>().anchoredPosition;
        if(MenuActive)
        {
            //Debug.Log("SpellUI::FixedUpdate()::Menu active");
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, initialPosition, 0.2f);//new Rect(GetComponent<RectTransform>().rect.position, GetComponent<RectTransform>().rect.size);
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, initialPosition + positionOffsetWhenActive, 0.2f);//new Rect(GetComponent<RectTransform>().rect.position, GetComponent<RectTransform>().rect.size);
        }

        if (MenuActive)
        {
            for (int i = 0; i < spellUIImage.Length; i++)
            {
                if (i == GameObject.FindObjectOfType<BookBehavior>().CurrentPageIndex)
                {
                    spellUIImage[i].GetComponent<RectTransform>().localScale = Vector3.Lerp(spellUIImage[i].GetComponent<RectTransform>().localScale, new Vector3(1.3f, 1.3f, 1.3f), 0.2f);
                    spellUIImage[i].GetComponent<Image>().color = Vector4.Lerp(spellUIImage[i].GetComponent<Image>().color, new Color(1, 1, 1, 1), 0.2f);
                }
                else
                {
                    spellUIImage[i].GetComponent<Image>().color = Vector4.Lerp(spellUIImage[i].GetComponent<Image>().color, new Color(0.4f, 0.4f, 0.4f, 1), 0.5f);
                    spellUIImage[i].GetComponent<RectTransform>().localScale = Vector3.Lerp(spellUIImage[i].GetComponent<RectTransform>().localScale, new Vector3(1f, 1f, 1f), 0.2f);
                }
            }
        }
    }
}
