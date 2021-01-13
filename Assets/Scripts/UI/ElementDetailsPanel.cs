using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the bottom-right panel with the details of the selected element.
/// </summary>
public class ElementDetailsPanel : MonoBehaviour
{
    [Header("Element panel properties")]
    public GameObject ElementDetailsPrefab;
    public Transform ElementDetailsContainer;
    public Transform IconTransform;

    [Tooltip("Input manager reference. Needed to keep track of the selected element.")]
    public InteractionManager InputManager;

    private Image _icon;

    public static ElementDetailsPanel Instance
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

            _icon = IconTransform.GetComponent<Image>();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Show the element details panel and updates its content.
    /// </summary>
    public void ShowPanel()
    {
        gameObject.SetActive(true);
        UpdatePanel();
    }

    /// <summary>
    /// If the panel is active, updates its content.
    /// </summary>
    public void UpdatePanel()
    {
        if (gameObject.activeSelf)
        {
            // If the panel is active, then there is an object selected.
            // Update the UI of the object selected.
            InputManager.SelectedElement.OnSelect();

            ResetPanel();

            _icon.sprite = InputManager.SelectedElement.Icon;
            foreach (var item in InputManager.SelectedElement.FormatProperties())
            {
                GameObject component = Instantiate(ElementDetailsPrefab, ElementDetailsContainer.transform);
                component.GetComponent<ElementDetailsFiller>().Fill(item);
            }
        }
    }

    /// <summary>
    /// Resets and hides the element details panel.
    /// </summary>
    public void HidePanel()
    {
        ResetPanel();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Clears the content of the element details panel.
    /// </summary>
    void ResetPanel()
    {
        for (int i = 0; i < ElementDetailsContainer.childCount; i++)
        {
            Destroy(ElementDetailsContainer.GetChild(i).gameObject);
        }
    }
}
