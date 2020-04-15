using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMore : MonoBehaviour
{
    public GameObject MeleeMan;

    public List<GameObject> enemies = new List<GameObject>();

    private int numEnemies = 0;
    public int spawnDelay;

    private float timeSinceLastSpawn;

    public GameObject Spawner;
    void Start()
    {
        //Spawner = GameObject.FindGameObjectWithTag("Spawner");
    }

    // Update is called once per frame
    void Update()
    {
        numEnemies = enemies.Count;
        timeSinceLastSpawn += Time.deltaTime;

        if(numEnemies <= 4 && timeSinceLastSpawn >= spawnDelay)
        {
            GameObject spawnedGhoul = Instantiate(MeleeMan, Spawner.transform.position, Quaternion.identity);
            enemies.Add(spawnedGhoul);
            timeSinceLastSpawn = 0;
        }

        foreach (GameObject enemy in enemies)
        {
            if(enemy == null)
            {
                enemies.Remove(enemy);
            }
        }
    }
}
