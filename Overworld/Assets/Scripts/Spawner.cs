using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public float spawnTime = 2f;
    public GameObject skelly;

    public List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating("Spawn", 1, spawnTime);
    }

    void Spawn()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject enemy = Instantiate(skelly, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

        enemies.Add(enemy);
    }

    private void Update()
    {
        foreach(GameObject enemy in enemies)
        {
            if(enemy == null)
            {
                enemies.Remove(enemy);
            }
        }
    }
}
