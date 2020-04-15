using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField]
    Slider healthSlider;
    [SerializeField]
    Slider manaSlider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = GameObject.FindObjectOfType<PlayerStats>().CurrentHealth;
        manaSlider.value = GameObject.FindObjectOfType<PlayerStats>().CurrentMana;
    }
}
