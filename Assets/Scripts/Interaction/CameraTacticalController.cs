using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTacticalController : MonoBehaviour
{
    [Header("Configurable Properties")]
    [Tooltip("How fast the camera rotates")]
    public float RotationSpeed = 16;
    [Tooltip("How fast the camera moves around")]
    public float MoveSpeed = 8;

    // Reference to the camera.
    private Camera _camera;

    // Movement variables
    private const float InternalMoveSpeed = 4;
    private Vector3 _moveTarget;
    private float _moveDeltaY;

    // Rotation variables
    private const float InternalRotationSpeed = 8;
    private Quaternion _rotationAroundY;
    private Quaternion _rotationTarget;
    private float _rotateDeltaAroundY;

    void Awake()
    {
        // References the tactical camera.
        _camera = GetComponentInChildren<Camera>();

        // Initializes the camera rotation: the camera looks to the center of the map.
        Vector3 lookDirection = new Vector3(0, 0, 0) - _camera.transform.position;
        _camera.transform.rotation = Quaternion.LookRotation(lookDirection);

        // Set the initial position and rotation value
        _rotationAroundY = transform.rotation;
        _moveTarget = transform.position;
    }

    void Update()
    {
        // Set the move target position based on the move delta y value. Must be done here as there's no logic for the input system to calculate holding down an input
        _moveTarget += transform.up * _moveDeltaY * Time.deltaTime * MoveSpeed;

        // Lerp the camera rig to a new move target position
        transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * InternalMoveSpeed);

        //Set the target rotation based on the mouse delta position and our rotation speed
        _rotationAroundY *= Quaternion.AngleAxis(_rotateDeltaAroundY * Time.deltaTime * RotationSpeed, Vector3.up);

        //Slerp the camera rig's rotation based on rotation around y-axis amount.
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotationAroundY, Time.deltaTime * InternalRotationSpeed);

        // Slerp the camera rotation based on the center of the map.
        Vector3 lookDirection = new Vector3(0, 0, 0) - _camera.transform.position;
        _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * InternalRotationSpeed);

    }

    /// <summary>
    /// Sets the direction of the rotation and movement based on the input provided by the player
    /// </summary>
    /// <param name="context"></param>
    public void OnMoveWASD(InputAction.CallbackContext context)
    {
        // Read the input value that is being sent by the Input System
        Vector2 value = context.ReadValue<Vector2>();

        // Store the x value in a variable indicating the delta movement on the y-axis.
        _moveDeltaY = value.y;

        // Store the y value in a variable indicating the delta rotation around the y-axis.
        _rotateDeltaAroundY = -value.x;
    }
}