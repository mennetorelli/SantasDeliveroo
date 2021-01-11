using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Class that handles popup menu spawning and update of its content.
/// </summary>
public class Popup : MonoBehaviour
{
    [Tooltip("Reference to the TextMeshProUGUI component of the message.")]
    public TextMeshProUGUI Message;
    [Tooltip("Reference to the primary button.")]
    public Button PrimaryButton;
    [Tooltip("Reference to the secondary button.")]
    public Button SecondaryButton;
    [Tooltip("Reference to the TextMeshProUGUI component of the primary button.")]
    public TextMeshProUGUI PrimaryButtonText;
    [Tooltip("Reference to the TextMeshProUGUI component of the secondary button.")]
    public TextMeshProUGUI SecondaryButtonText;

    public static Popup Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
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
    /// Sets the popup's GameObject to active, and updates its header, message and button's text and callbacks.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="primaryButtonText">The text of the primary button.</param>
    /// <param name="secondaryButtonText">The text of the secondary button.</param>
    /// <param name="primaryCallback">The UnityAction invoked when clicking on the primary button.</param>
    /// <param name="secondaryCallback">The UnityAction invoked when clicking on the secondary button.</param>
    public void ActivatePopup(string message, string primaryButtonText = null, string secondaryButtonText = null, UnityAction primaryCallback = null, UnityAction secondaryCallback = null)
    {
        // Update the menu properties.
        Message.text = message;
        PrimaryButtonText.text = primaryButtonText;
        SecondaryButtonText.text = secondaryButtonText;
        PrimaryButton.onClick.AddListener(primaryCallback);
        SecondaryButton.onClick.AddListener(primaryCallback);

        gameObject.SetActive(true);
    }

    void OnDisable()
    {
        PrimaryButton.onClick.RemoveAllListeners();
        SecondaryButton.onClick.RemoveAllListeners();
    }
}
