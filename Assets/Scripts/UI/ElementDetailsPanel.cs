using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDetailsPanel : MonoBehaviour
{
    [Header("Element panel properties")]
    public GameObject ElementDetailsPrefab;
    public Transform ElementDetailsContainer;
    public Transform IconTransform;

    [Tooltip("MouseInputManagerReference. Needed to keep track of the selected element.")]
    public MouseInputController MouseInputManager;

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


    public void ShowPanel()
    {
        gameObject.SetActive(true);
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        if (gameObject.activeSelf)
        {
            ResetPanel();

            _icon.sprite = MouseInputManager.SelectedElement.Icon;
            foreach (var item in MouseInputManager.SelectedElement.FormatDetails())
            {
                GameObject component = Instantiate(ElementDetailsPrefab, ElementDetailsContainer.transform);
                component.GetComponent<ElementDetailsFiller>().Fill(item);
            }
        }
    }

    public void HidePanel()
    {
        ResetPanel();
        gameObject.SetActive(false);
    }

    void ResetPanel()
    {
        for (int i = 0; i < ElementDetailsContainer.childCount; i++)
        {
            Destroy(ElementDetailsContainer.GetChild(i).gameObject);
        }
    }
}
