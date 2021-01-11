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


    public void ShowPanel(Sprite icon, List<string> properties)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < ElementDetailsContainer.childCount; i++)
        {
            Destroy(ElementDetailsContainer.GetChild(i).gameObject);
        }
        _icon.sprite = icon;
        foreach (var item in properties)
        {
            GameObject component = Instantiate(ElementDetailsPrefab, ElementDetailsContainer.transform);
            component.GetComponent<ElementDetailsFiller>().Fill(item);
        }
    }

    public void HidePanel()
    {
        for (int i = 0; i < ElementDetailsContainer.childCount; i++)
        {
            Destroy(ElementDetailsContainer.GetChild(i).gameObject);
        }
        gameObject.SetActive(false);
    }
}
