using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableObject : MonoBehaviour
{
    //[SerializeField]
    //string objectName;
    int objectState;

    SaveFileLoader saveFileLoader;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LateStart", 0.01f);
    }
    void LateStart()
    {
        if(saveFileLoader == null)
            saveFileLoader = GameObject.FindObjectOfType<SaveFileLoader>();

        SaveFileLoader.GetObjectState(gameObject.name);
    }

    public void UpdateObjectState(int newState)
    {
        if (saveFileLoader == null)
            saveFileLoader = GameObject.FindObjectOfType<SaveFileLoader>();

        SaveFileLoader.SetObjectState(gameObject.name, newState);//Update save file's state based on object name
        objectState = newState;//Update internal object state
    }

    public int GetState()
    {
        return objectState;
    }

    // Update is called once per frame
    void Update()
    {

    }
}