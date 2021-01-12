using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputController : MonoBehaviour
{
    public GameObject RayIntersectorPlane;
    public VisualFeedbackDrawer FeedbackDrawer;
    
    private bool _appendInstructionInQueue;
    private float _yOffset;

    public Santa SelectedSanta { get; set; }

    private bool _moveStantaModeEnabled;
    private bool MoveStantaModeEnabled 
    { 
        get => _moveStantaModeEnabled; 
        set 
        {
            _moveStantaModeEnabled = value;
            RayIntersectorPlane.SetActive(value);
            _yOffset = 0;
        } 
    }

    void Update()
    {
        if (MoveStantaModeEnabled && SelectedSanta != null)
        {
            RayIntersectorPlane.transform.position = new Vector3(RayIntersectorPlane.transform.position.x, SelectedSanta.transform.position.y, RayIntersectorPlane.transform.position.z);

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("RayIntersectorPlane")))
            {
                FeedbackDrawer.DrawPath(SelectedSanta.Origin, new Vector3(hit.point.x, hit.point.y + _yOffset, hit.point.z));
            }
        }
    }

    public void OnMouseLeftDown(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Santa", "Befana", "Target")))
        {
            // Show info of the selected object.
            hit.transform.GetComponent<SelectableElementBase>().UpdateInfoInPanel();

            // If the selected object is a Santa, then update the SelectedSanta and activate the Intersector plane,
            // else set the SelectedSanta to null and deactivate the Intersector plane. 
            if (hit.transform.GetComponent<Santa>() != null)
            {
                SelectedSanta = hit.transform.GetComponent<Santa>();
            }
            else
            {
                SelectedSanta = null;
            }
        }
        // If the user has selected nothing, set the SelectedSanta to null and deactivate the Intersector plane. 
        else
        {
            ElementDetailsPanel.Instance.HidePanel();
            SelectedSanta = null;
        }
    }

    public void OnMouseRightDown(InputAction.CallbackContext context)
    {
        if (SelectedSanta != null)
        {
            if (!MoveStantaModeEnabled)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Target")))
                {
                    SelectedSanta.AddActionToQueue(
                        new Vector3(hit.transform.position.x, hit.transform.position.y + hit.collider.bounds.extents.y, hit.transform.position.z), 
                        () => SelectedSanta.ExecuteAction(hit.collider.gameObject),
                        _appendInstructionInQueue);
                }
            }
            else
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("RayIntersectorPlane")))
                {
                    SelectedSanta.AddActionToQueue(new Vector3(hit.point.x, hit.point.y + _yOffset, hit.point.z), () => SelectedSanta.ExecuteAction(), _appendInstructionInQueue);
                    MoveStantaModeEnabled = false;
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
        _appendInstructionInQueue = context.ReadValue<float>() > 0;
    }

    public void OnShiftKeyPressed(InputAction.CallbackContext context)
    {
        if (SelectedSanta != null)
        {
            MoveStantaModeEnabled = !MoveStantaModeEnabled;
        }
    }

}
