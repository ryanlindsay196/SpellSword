using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public Canvas deathCanvas;
    private float pHP;
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }

    public void Respawn()
    {
        playerStats.Respawn();
    }

    // Update is called once per frame
    void Update()
    {
        pHP = playerStats.CurrentHealth;

        if (pHP <= 0)
        { if (!deathCanvas.gameObject.activeInHierarchy) { deathCanvas.gameObject.SetActive(true); } }
        else if(Input.GetButton("UseSpell") || Input.GetButton("Jump"))
        { if (deathCanvas.gameObject.activeInHierarchy) { deathCanvas.gameObject.SetActive(false); } }

    }
}
