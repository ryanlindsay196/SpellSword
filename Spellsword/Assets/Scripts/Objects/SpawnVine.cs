using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVine : MonoBehaviour
{
    public GameObject vinePrefab;   
    public Transform vineSpawnPos;  //transform is a child object to gameObject
    
    public GameObject plantPrefab;   
    public Transform plantSpawnPos;  //transform is a child object to gameObject       

    public Transform plantThreeSpawnPos;  //transform is a child object to gameObject 

    public GameObject ghoulPrefab;
    public Transform ghoulSpawnPos;  //transform is a child object to gameObject       
    public Transform ghoulTwoSpawnPos;  //transform is a child object to gameObject       

    bool hasEntered = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!hasEntered)
            {
                //Debug.Log(collision.gameObject.name);
                hasEntered = true;  //Cant respawn vine
                GameObject newVine = Instantiate(vinePrefab, vineSpawnPos);  //Spawn vine
                newVine.transform.localPosition = Vector3.zero;   //Reset vine coord's to zero
                                                                  //newVine.transform.localEulerAngles = new Vector3(-90, 0, -90); //faces vines forward   

                GameObject newPlant = Instantiate(plantPrefab, plantSpawnPos);  
                newPlant.transform.localPosition = Vector3.zero;   

                GameObject new3rdPlant = Instantiate(plantPrefab, plantThreeSpawnPos);
                new3rdPlant.transform.localPosition = Vector3.zero;

                GameObject newGhoul = Instantiate(ghoulPrefab, ghoulSpawnPos);
                newGhoul.transform.localPosition = Vector3.zero;   
                GameObject new2ndGhoul = Instantiate(ghoulPrefab, ghoulTwoSpawnPos);
                new2ndGhoul.transform.localPosition = Vector3.zero;
            }
        }
    }
}
