using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int[] enemyCountPerRound;
    public int multiplier;

    public int enemyCount;
    public int currentRound;

    public bool roundOver;
    public bool maximumSpawned;
    public bool canSpawn;

    public Transform[] spawnPoints;
    public float spawnTime = 2f;
    public GameObject skelly;

    public List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        currentRound = 1;
        enemyCount = 0;

        roundOver = true;
        maximumSpawned = false;
        canSpawn = true;
    }

    private void Update()
    {
        ClearEnemies();

        if(roundOver)
        {
            if(currentRound == 1 && !maximumSpawned && canSpawn)
            {
                Spawn();
            }
            if (currentRound == 2 && !maximumSpawned && canSpawn)
            {
                Spawn();
            }
            if (currentRound == 3 && !maximumSpawned && canSpawn)
            {
                Spawn();
            }
            if (currentRound == 4 && !maximumSpawned && canSpawn)
            {
                Spawn();
            }
            if (currentRound == 5 && !maximumSpawned && canSpawn)
            {
                Spawn();
            }
            if (currentRound > 5 && !maximumSpawned && canSpawn)
            {
                enemyCountPerRound[currentRound - 1] = enemyCountPerRound[currentRound - 1] + multiplier;
                multiplier++;
                Spawn();
            }

            if (maximumSpawned)
            {
                enemyCount = enemies.Count;
                if (enemyCount == 0)
                {
                    currentRound++;
                    maximumSpawned = false;
                }
            }
        }
    }

    void Spawn()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject enemy = Instantiate(skelly, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

        enemies.Add(enemy);

        enemyCount++;

        canSpawn = false;

        StartCoroutine(Cooldown());

        if (enemyCount == enemyCountPerRound[currentRound - 1])
        {
            maximumSpawned = true;
        }
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(spawnTime);
        canSpawn = true;
    }

    void ClearEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                enemies.Remove(enemy);
            }
        }
    }
}
