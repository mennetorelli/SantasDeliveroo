using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the free view camera.
/// </summary>
public class CameraFreeController : MonoBehaviour
{
    [Header("Configurable Properties")]
    [Tooltip("How fast the camera rotates")]
    public float RotationSpeed = 8;
    [Tooltip("How fast the camera moves around")]
    public float MoveSpeed = 8;

    // Movement variables
    private const float InternalMoveSpeed = 4;
    private Vector3 _moveTarget;
    private Vector3 _moveDirection;

    // Rotation variables
    private bool _rightMouseDown = false;
    private const float InternalRotationSpeed = 4;
    private Quaternion _rotationTarget;
    private Vector2 _mouseDelta;

    void Awake()
    {
        // Set the initial position and rotation value
        _rotationTarget = transform.rotation;
        _moveTarget = transform.position;
    }

    void Update()
    {
        // Set the move target position based on the move direction. Must be done here as there's no logic for the input system to calculate holding down an input
        _moveTarget += (transform.forward * _moveDirection.z + transform.up * _moveDirection.y + transform.right * _moveDirection.x) * Time.deltaTime * MoveSpeed;

        // Clamp x,y,z values of _moveTarget so that the camera doesn't exceed the boundaries.
        _moveTarget.x = Mathf.Clamp(_moveTarget.x, -GameManager.Instance.MaxRangeXZ - 5f, GameManager.Instance.MaxRangeXZ + 5f);
        _moveTarget.y = Mathf.Clamp(_moveTarget.y, GameManager.Instance.MinHeight, GameManager.Instance.MaxHeight + 5f);
        _moveTarget.z = Mathf.Clamp(_moveTarget.z, -GameManager.Instance.MaxRangeXZ - 5f, GameManager.Instance.MaxRangeXZ + 5f);

        // Lerp the camera rig to a new move target position
        transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * InternalMoveSpeed);

        // Set the target rotation based on the mouse delta position and our rotation speed
        _rotationTarget *= Quaternion.Euler(-_mouseDelta.y * Time.deltaTime * RotationSpeed, _mouseDelta.x * Time.deltaTime * RotationSpeed, 0);
        _rotationTarget = Quaternion.Euler(_rotationTarget.eulerAngles.x, _rotationTarget.eulerAngles.y, 0);

        // Slerp the camera rig's rotation based on the new target
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotationTarget, Time.deltaTime * InternalRotationSpeed);
    }

    /// <summary>
    /// Sets the direction of movement (left/right - forward/back) based on the input provided by the player
    /// </summary>
    /// <param name="context"></param>
    public void OnMoveWASD(InputAction.CallbackContext context)
    {
        // Read the input value that is being sent by the Input System
        Vector2 value = context.ReadValue<Vector2>();

        // Store the value as a Vector3, making sure to move the Y input on the Z axis.
        _moveDirection = new Vector3(value.x, 0, value.y);
    }

    /// <summary>
    /// Sets the direction of movement (up/down) based on the input provided by the player
    /// </summary>
    /// <param name="context"></param>
    public void OnMoveQE(InputAction.CallbackContext context)
    {
        // Read the input value that is being sent by the Input System
        float value = context.ReadValue<Vector2>().y;

        // Store the value as a Vector3, making sure to move the Y input on the Z axis.
        _moveDirection = new Vector3(0, value, 0);
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
