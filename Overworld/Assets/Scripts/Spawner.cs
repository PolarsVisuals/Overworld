using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public float spawnTime = 2f;

    public GameObject lightening;
    public GameObject beam;
    public GameObject skelly;

    public List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }

    void Spawn()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        Instantiate(lightening, spawnPoints[spawnPointIndex]);
        Instantiate(beam, spawnPoints[spawnPointIndex]);

        GameObject enemy = Instantiate(skelly, spawnPoints[spawnPointIndex]);

        enemies.Add(skelly);
    }

    private void Update()
    {
        foreach(GameObject skelly in enemies)
        {
            if(skelly == null)
            {
                enemies.Remove(skelly);
            }
        }
    }
}
