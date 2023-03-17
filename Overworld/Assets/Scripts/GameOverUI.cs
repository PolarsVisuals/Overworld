using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public Spawner Spawner;

    public Image fadePlane;
    public GameObject gameOverUI;
    public TextMeshProUGUI roundText;

    void Awake()
    {
        gameOverUI.SetActive(false);
    }

    public void OnGameOver()
    {
        gameOverUI.SetActive(true);
        //StartCoroutine(Fade(Color.clear, Color.black, 1));
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        roundText.text = Spawner.currentRound.ToString();
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
}
