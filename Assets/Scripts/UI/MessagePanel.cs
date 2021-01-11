using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePanel : MonoBehaviour
{
    public TextMeshProUGUI Message;

    // Start is called before the first frame update
    public static MessagePanel Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        // Singleton implementation.
        if (Instance == null)
        {
            Instance = this;

            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowMessage(string message, Color color)
    {
        gameObject.SetActive(true);
        Message.text = message;
        Message.color = color;
        StartCoroutine(WaitAndDeactivate());
    }

    IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(3f);
        Message.text = string.Empty;
        Message.color = Color.black;
        gameObject.SetActive(false);
    }
}
