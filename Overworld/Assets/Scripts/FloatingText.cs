using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float amount;

    public TextMeshProUGUI text;
    public Transform position;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);

    void Start()
    {
        Vector3 imagePos = Camera.main.WorldToScreenPoint(position.position + textOffset);

        text.transform.position = imagePos;

        text.text = amount.ToString();

        RemoveText(text);
    }

    void RemoveText(TextMeshProUGUI text)
    {
        Destroy(text.gameObject, 1);
    }
}