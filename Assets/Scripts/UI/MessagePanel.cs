using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the top-right panel with messages from the game.
/// </summary>
public class MessagePanel : MonoBehaviour
{
    [Tooltip("Reference to the text of the panel.")]
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

    /// <summary>
    /// Shows a message in a given color.
    /// </summary>
    /// <param name="message">The message to be shown.</param>
    /// <param name="color">The color of the message to be shown.</param>
    public void ShowMessage(string message, Color color)
    {
        gameObject.SetActive(true);
        Message.text = message;
        Message.color = color;
        StartCoroutine(WaitAndDeactivate());
    }

    /// <summary>
    /// Waits 3 seconds, then deactivates the panel message.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(5f);
        Message.text = string.Empty;
        Message.color = Color.black;
        gameObject.SetActive(false);
    }
}
