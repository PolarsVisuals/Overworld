using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCrossahir : MonoBehaviour
{
    public Vector3 enemyPos;
    public Vector3 offset;

    private GameObject playerCanvas;

    // Update is called once per frame
    void Update()
    {
        Vector3 imagePos = Camera.main.WorldToScreenPoint(enemyPos + offset);

        transform.position = imagePos;
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
