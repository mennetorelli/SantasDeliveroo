using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class used to enable an object being selected with mouse.
/// </summary>
public abstract class SelectableElementBase : MonoBehaviour
{
    [Tooltip("The icon representing the selectable object in the element details panel.")]
    public Sprite Icon;
    [Tooltip("If the object is selected, this GameObject is activated.")]
    public GameObject Selection;
    [Tooltip("Highlights this object for drawing user attention.")]
    public GameObject Highlight;
    [Tooltip("If this collider is specified and the object is a target object, the Santa will move to the center of the collider area when performing an action.")]
    public Collider DestinationArea;

    /// <summary>
    /// Triggers when the user selects the element with a right-click.
    /// </summary>
    public virtual void OnSelect()
    {
        Selection.SetActive(true);
    }

    /// <summary>
    /// Triggers when the user has deselected the element.
    /// </summary>
    public virtual void OnDeselect()
    {
        Selection.SetActive(false);
    }

    /// <summary>
    /// Formats the element properties to show them in the element details panel.
    /// </summary>
    public abstract List<string> FormatProperties();
}
