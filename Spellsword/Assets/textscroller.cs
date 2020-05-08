using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class textscroller : MonoBehaviour
{
    public Transform uiTransform;
    float change;
    // Update is called once per frame
    void Update()
    {
        change += (Time.deltaTime * 25);
        uiTransform.localPosition = new Vector3(0, change, 0);
        if(uiTransform.localPosition.y >= 2395.075)
        { SceneManager.LoadScene(0); }
    }
}
