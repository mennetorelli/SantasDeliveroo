using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Configurable Properties")]
    [Tooltip("How fast the camera rotates")]
    public float RotationSpeed = 8;
    [Tooltip("How fast the camera moves around")]
    public float MoveSpeed = 8;

    //Movement variables
    private const float InternalMoveSpeed = 4;
    private Vector3 _moveTarget;
    private Vector3 _moveDirection;

    //Rotation variables
    private bool _rightMouseDown = false;
    private const float InternalRotationSpeed = 4;
    private Quaternion _rotationTarget;
    private Vector2 _mouseDelta;

    private float _timeScale { get => Time.timeScale != 0 ? Time.timeScale : 1; }

    void Start()
    {
        //Set the initial position and rotation value
        _rotationTarget = transform.rotation;
        _moveTarget = transform.position;
    }

    void Update()
    {
        //Sets the move target position based on the move direction. Must be done here as there's no logic for the input system to calculate holding down an input
        _moveTarget += (transform.forward * _moveDirection.z + transform.right * _moveDirection.x) * Time.deltaTime / _timeScale * MoveSpeed;

        //Lerp the camera rig to a new move target position
        transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime / _timeScale * InternalMoveSpeed);

        //Set the target rotation based on the mouse delta position and our rotation speed
        _rotationTarget *= Quaternion.Euler(-_mouseDelta.y * Time.deltaTime / _timeScale * RotationSpeed, _mouseDelta.x * Time.deltaTime / _timeScale * RotationSpeed, 0);
        _rotationTarget = Quaternion.Euler(_rotationTarget.eulerAngles.x, _rotationTarget.eulerAngles.y, 0);

        //Slerp the camera rig's rotation based on the new target
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotationTarget, Time.deltaTime / _timeScale * InternalRotationSpeed);
    }

    /// <summary>
    /// Sets the direction of movement based on the input provided by the player
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //Read the input value that is being sent by the Input System
        Vector2 value = context.ReadValue<Vector2>();

        //Store the value as a Vector3, making sure to move the Y input on the Z axis.
        _moveDirection = new Vector3(value.x, 0, value.y);
    }

    /// <summary>
    /// Sets whether the player has the right mouse button down
    /// </summary>
    /// <param name="context"></param>
    public void OnRotateToggle(InputAction.CallbackContext context)
    {
        _rightMouseDown = context.ReadValue<float>() == 1;
    }

    /// <summary>
    /// Sets the rotation target quaternion if the right mouse button is pushed when the player is moving the mouse
    /// </summary>
    /// <param name="context"></param>
    public void OnRotate(InputAction.CallbackContext context)
    {
        // If the right mouse is down then we'll read the mouse delta value. If it is not, we'll clear it out.
        // Note: Clearing the mouse delta prevents a 'death spin' from occuring if the player flings the mouse really fast in a direction.
        _mouseDelta = _rightMouseDown ? context.ReadValue<Vector2>() : Vector2.zero;
    }
}
