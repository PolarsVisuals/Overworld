using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject firstButton;
    public GameObject[] pannels;
    public int pannelNum;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void Play()
    {
        SceneManager.LoadScene("Tut");
    }

    private void Update()
    {
        if(pannels != null)
        {
            for (int i = 0; i < pannels.Length; i++)
            {
                if (pannelNum == i)
                {
                    pannels[i].SetActive(true);
                }
                else
                {
                    pannels[i].SetActive(false);
                }
            }
        }
    }

    public void Load()
    {
        SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Replay()
    {
        SceneManager.LoadScene("Main");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void leftPanel()
    {
        pannelNum--;
        if(pannelNum == - 1)
        {
            pannelNum = 2;
        }
    }

    public void RightPanel()
    {
        pannelNum++;
        if (pannelNum == 3)
        {
            pannelNum = 0;
        }
    }
}
