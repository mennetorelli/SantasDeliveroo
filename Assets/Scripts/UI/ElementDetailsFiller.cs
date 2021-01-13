using TMPro;
using UnityEngine;

public class ElementDetailsFiller : MonoBehaviour
{
    public TextMeshProUGUI ElementProperty;

    public void Fill(string property)
    {
        ElementProperty.text = property;
    }
}
