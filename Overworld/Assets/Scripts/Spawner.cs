using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    [Header("Rounds")]
    public int[] enemyCountPerRound;
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

    public Transform[] spawnPoints;
    public float spawnTime = 2f;
    public GameObject skelly;

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
    }

    private void Update()
    {
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

    IEnumerator LoadGameOver()
    {
        yield return new WaitForSeconds(5);
        gameOverUI.OnGameOver();
    }
}
