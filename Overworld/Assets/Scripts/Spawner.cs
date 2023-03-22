using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    [Header("Rounds")]
    public int[] enemyCountPerRound;
    public int enemyCountMulti;
    public int multiplier;

    public int enemyCount;
    public int currentRound;

    public bool roundOver;
    public bool maximumSpawned;
    public bool canSpawn;

    public TextMeshProUGUI roundNumText;

    [Header("Score")]
    public int currentScore;
    public int scorePerEnemy;
    public TextMeshProUGUI scoreText;

    public bool gameOver;
    public GameOverUI gameOverUI;

    bool multiplierRound;

    public Transform[] areas;
    public Transform[] spawnPoints;
    public float spawnTime = 2f;
    public GameObject skelly;

    private Transform player;

    public List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        currentRound = 1;
        enemyCount = 0;

        currentScore = 0;

        roundOver = true;
        maximumSpawned = false;
        canSpawn = true;

        gameOver = false;
        multiplierRound = false;

        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        Transform closetArea = GetClosestArea(areas);
        Debug.DrawLine(player.position, closetArea.position, Color.red);
        for(int i = 0; i < 4; i++)
        {
            spawnPoints[i] = closetArea.GetChild(i);
        }

        ClearEnemies();

        roundNumText.text = currentRound.ToString();
        scoreText.text = currentScore.ToString();

        if (!gameOver)
        {
            if (roundOver)
            {
                if (currentRound == 1 && !maximumSpawned && canSpawn)
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
                    spawnTime = spawnTime / 2;
                    SpawnMulti();
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

        if(GameObject.Find("Player").GetComponent<LivingEntity>().dead == true)
        {
            gameOver = true;
            StartCoroutine(LoadGameOver());
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
    void SpawnMulti()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject enemy = Instantiate(skelly, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

        enemies.Add(enemy);

        enemyCount++;

        canSpawn = false;

        StartCoroutine(Cooldown());

        if (enemyCount == enemyCountPerRound[5] + multiplier)
        {
            enemyCountPerRound[5] = enemyCountPerRound[5] + multiplier;
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
            if (enemy == null || enemy.GetComponent<Enemy>().activeTarget == false)
            {
                enemies.Remove(enemy);
                currentScore = currentScore + scorePerEnemy;
            }
        }
    }

    Transform GetClosestArea(Transform[] areas)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform area in areas)
        {
            float dist = Vector3.Distance(area.position, player.position);
            if (dist < minDist)
            {
                tMin = area;
                minDist = dist;
            }
        }
        return tMin;
    }

    IEnumerator LoadGameOver()
    {
        yield return new WaitForSeconds(5);
        gameOverUI.OnGameOver();
    }
}
