using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class that manages the keyboard/mouse interaction related to the gameplay.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    [Tooltip("This plane is activated when move Santa mode is enabled, to make possible the movement on y-axis.")]
    public GameObject RayIntersectorPlane;
    [Tooltip("Renders the path when move santa mode is enabled.")]
    public PathRenderer PathDrawer;
    [Tooltip("Renders the offset on the y-axis when move santa mode is enabled.")]
    public PathRenderer YOffsetDrawer;

    private bool _appendActionInQueue;
    private bool _moveActionEnabled;
    private float _yOffset;
    private bool _tacticalModeEnabled;

    public SelectableElementBase SelectedElement { get; set; }

    public static InteractionManager Instance
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // If a Santa has been selected and the user has enabled move action mode.
        if (_moveActionEnabled && SelectedElement != null && SelectedElement.GetType() == typeof(Santa))
        {
            Santa selectedSanta = (Santa)SelectedElement;

            // Activate the RayIntersectorPlane and set its height equal to the one of the selected santa.
            RayIntersectorPlane.SetActive(true);
            RayIntersectorPlane.transform.position = new Vector3(RayIntersectorPlane.transform.position.x, selectedSanta.transform.position.y, RayIntersectorPlane.transform.position.z);

            // Draw a path from the player/last waypoint to the point where the uses is hovering on the plane (+ the offset on y-axis).
            // + draw a line which represents the offset on the y-axis.
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("RayIntersectorPlane")))
            {
                Vector3 destination = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                Vector3 destinationWithYOffset = new Vector3(hit.point.x, hit.point.y + _yOffset, hit.point.z);

                PathDrawer.DrawPath(selectedSanta.GetStartingPoint(_appendActionInQueue), destinationWithYOffset);
                PathDrawer.DrawDestination(destinationWithYOffset);
                YOffsetDrawer.DrawPath(destination, destinationWithYOffset);
            }
        }
        else
        {
            RayIntersectorPlane.SetActive(false);
        }
    }

    /// <summary>
    /// Triggers when the user Selects an element with left-click.
    /// </summary>
    /// <param name="context">Holds context information of the state of the Action and the values of the controls.</param>
    public void OnSelectElement(InputAction.CallbackContext context)
    {
        if (context.performed && Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Santa", "Befana", "Target")) && !GameManager.Instance.IsPaused)
        {
            SelectableElementBase selectedElement = hit.transform.GetComponent<SelectableElementBase>();
            Debug.Log(selectedElement);

            // If the user is not selecting the same element, instruct the current element it has been selected,
            // save a reference of the selected element and update the element details panel.
            if (selectedElement != SelectedElement)
            {
                // A different Santa can be selected, so reset the offset on the y-axis.
                _yOffset = 0;

                // If the user has selected a different element, the previous one is no longer selected.
                if (SelectedElement != null)
                {
                    SelectedElement.OnDeselect();
                }

                selectedElement.OnSelect();
                SelectedElement = selectedElement;
                ElementDetailsPanel.Instance.ShowPanel();
            }
        }
        // If the user has selected nothing, set the SelectedElement to null and deactivate the Intersector plane. 
        else
        {
            if (SelectedElement != null)
            {
                SelectedElement.OnDeselect();
            }

            ElementDetailsPanel.Instance.HidePanel();
            SelectedElement = null;
        }
    }

    /// <summary>
    /// Triggers when a Santa is currently selected an the user right-clicks on a target object (house/gift).
    /// </summary>
    /// <param name="context">Holds context information of the state of the Action and the values of the controls.</param>
    public void OnAction(InputAction.CallbackContext context)
    {
        // Tactical mode must be enabled and a Santa must have been selected to perform the action.
        if (_tacticalModeEnabled && context.performed && SelectedElement != null && SelectedElement.GetType() == typeof(Santa) && !GameManager.Instance.IsPaused)
        {
            Santa selectedSanta = (Santa)SelectedElement;
            // Move mode enabled.
            if (_moveActionEnabled)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("RayIntersectorPlane")))
                {
                    // The destination is the point selected on the RayIntersectorPlane (+ the offset on the y-axis).
                    selectedSanta.AddActionToQueue(new Vector3(hit.point.x, hit.point.y + _yOffset, hit.point.z),
                        () => selectedSanta.ExecuteAction(), _appendActionInQueue);
                    // Reset the offset on the y-axis.
                    _yOffset = 0;
                }
            }
            // Collect/deliver mode enabled.
            else
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Target")))
                {
                    // If this object has a DestinationArea specified, move to the center of the area, otherwise move to the center of the hit collider.
                    Vector3 destination = hit.transform.GetComponentInParent<SelectableElementBase>().DestinationArea != null ?
                        hit.transform.GetComponentInParent<SelectableElementBase>().DestinationArea.bounds.center : hit.collider.bounds.center;

                    // The destination is the collider of the target object, send the target object as parameter of ExecuteAction to save its reference.
                    selectedSanta.AddActionToQueue(destination,
                        () => selectedSanta.ExecuteAction(hit.collider.gameObject), _appendActionInQueue);
                }
            }
        }
    }

    /// <summary>
    /// Triggers when the user presses ctrl key.
    /// </summary>
    /// <param name="context">Holds context information of the state of the Action and the values of the controls.</param>
    public void OnActionAppendToggle(InputAction.CallbackContext context)
    {
        _appendActionInQueue = context.ReadValue<float>() > 0;
    }

    /// <summary>
    /// Triggers when the user presses the key which enables action modes.
    /// </summary>
    /// <param name="context">Holds context information of the state of the Action and the values of the controls.</param>
    public void OnMoveActionEnabled(InputAction.CallbackContext context)
    {
        _moveActionEnabled = !_moveActionEnabled && _tacticalModeEnabled;
        // Reset the offset on the y-axis.
        _yOffset = 0;
    }

    /// <summary>
    /// Triggers when the user scrolls with the mouse.
    /// </summary>
    /// <param name="context">Holds context information of the state of the Action and the values of the controls.</param>
    public void OnChangeYOffsetMoveAction(InputAction.CallbackContext context)
    {
        _yOffset = Mathf.Clamp(_yOffset + context.ReadValue<Vector2>().normalized.y / 10,
            GameManager.Instance.MinHeight - RayIntersectorPlane.transform.position.y,
            GameManager.Instance.MaxHeight - RayIntersectorPlane.transform.position.y);
    }

    /// <summary>
    /// Triggers when the user changes the camera mode from free to tactical and vice versa.
    /// </summary>
    /// <param name="context">Holds context information of the state of the Action and the values of the controls.</param>
    public void OnCameraViewChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _tacticalModeEnabled = !_tacticalModeEnabled;
        }
    }
}
