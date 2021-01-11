using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface used to enable an object being dselected with mouse. 
/// Pipeline components and Resources must inherit from this interface.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Handles mouse selection.
    /// </summary>
    void Selected();
}
