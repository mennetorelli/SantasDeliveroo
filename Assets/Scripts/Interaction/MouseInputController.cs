using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputController : MonoBehaviour
{
    public GameObject RayIntersectorPlane;

    private PathRenderer _feedbackDrawer;

    private bool _appendActionInQueue;
    private bool _moveSantaModeEnabled;
    private float _yOffset;

    public SelectableElementBase SelectedElement { get; set; }

    void Awake()
    {
        _feedbackDrawer = RayIntersectorPlane.GetComponentInChildren<PathRenderer>();
    }

    void Update()
    {
        if (_moveSantaModeEnabled && SelectedElement != null && SelectedElement.GetType() == typeof(Santa))
        {
            Santa selectedSanta = (Santa)SelectedElement;

            RayIntersectorPlane.SetActive(true);
            RayIntersectorPlane.transform.position = new Vector3(RayIntersectorPlane.transform.position.x, selectedSanta.transform.position.y, RayIntersectorPlane.transform.position.z);
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("RayIntersectorPlane")))
            {
                _feedbackDrawer.DrawPath(selectedSanta.Origin, new Vector3(hit.point.x, hit.point.y + _yOffset, hit.point.z));
            }
        }
        else
        {
            RayIntersectorPlane.SetActive(false);
        }
    }

    public void OnMouseLeftDown(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Santa", "Befana", "Target")))
        {
            SelectableElementBase selectedElement = hit.transform.GetComponent<SelectableElementBase>();
            
            // If the user is not selecting the same element, instruct the current element it has been selected,
            // save a reference of the selected element and update the element details panel.
            if (selectedElement != SelectedElement)
            {
                // If we changed selected element, the previous one is no longer selected.
                if (SelectedElement != null)
                {
                    SelectedElement.Deselected();
                }

                selectedElement.Selected();
                SelectedElement = selectedElement;
                ElementDetailsPanel.Instance.ShowPanel();
            }
        }
        // If the user has selected nothing, set the SelectedElement to null and deactivate the Intersector plane. 
        else
        {
            if (SelectedElement != null)
            {
                SelectedElement.Deselected();
            }

            ElementDetailsPanel.Instance.HidePanel();
            SelectedElement = null;
        }
    }

    public void OnMouseRightDown(InputAction.CallbackContext context)
    {
        if (SelectedElement != null && SelectedElement.GetType() == typeof(Santa))
        {
            Santa selectedSanta = (Santa)SelectedElement;
            if (!_moveSantaModeEnabled)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Target")))
                {
                    selectedSanta.AddActionToQueue(
                        new Vector3(hit.transform.position.x, hit.transform.position.y + hit.collider.bounds.extents.y, hit.transform.position.z), 
                        () => selectedSanta.ExecuteAction(hit.collider.gameObject),
                        _appendActionInQueue);
                }
            }
            else
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("RayIntersectorPlane")))
                {
                    selectedSanta.AddActionToQueue(
                        new Vector3(hit.point.x, hit.point.y + _yOffset, hit.point.z), 
                        () => selectedSanta.ExecuteAction(), 
                        _appendActionInQueue);
                }
            }
        }
    }

    public void OnMouseMiddleScroll(InputAction.CallbackContext context)
    {
        _yOffset = Mathf.Clamp(_yOffset + context.ReadValue<Vector2>().normalized.y / 10, 
            GameManager.Instance.MinHeight - RayIntersectorPlane.transform.position.y, 
            GameManager.Instance.MaxHeight - RayIntersectorPlane.transform.position.y);
    }

    public void OnCtrlKeyPressed(InputAction.CallbackContext context)
    {
        _appendActionInQueue = context.ReadValue<float>() > 0;
    }

    public void OnShiftKeyPressed(InputAction.CallbackContext context)
    {
        _moveSantaModeEnabled = context.ReadValue<float>() > 0;
        _yOffset = 0;
    }

}
