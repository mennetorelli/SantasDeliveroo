using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Every target object should implement this interface.
/// </summary>
public interface ITarget
{
    /// <summary>
    /// Invoked when a santa has collided with the target object.
    /// </summary>
    /// <param name="santa">The santa that triggered the collision.</param>
    void TargetReached(Santa santa);
}

