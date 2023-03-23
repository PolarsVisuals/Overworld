using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject firstButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void Play()
    {
        SceneManager.LoadScene("Tut");
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
}
