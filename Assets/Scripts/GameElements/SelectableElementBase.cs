using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface used to enable an object being dselected with mouse. 
/// Pipeline components and Resources must inherit from this interface.
/// </summary>
public abstract class SelectableElementBase : MonoBehaviour
{
    public Sprite Icon;

    /// <summary>
    /// Triggers when user selects the element with the mouse.
    /// </summary>
    public abstract void Selected();

    /// <summary>
    /// Triggers when has deselected the element.
    /// </summary>
    public abstract void Deselected();

    /// <summary>
    /// Triggers when has deselected the element.
    /// </summary>
    public abstract List<string> FormatDetails();
}
