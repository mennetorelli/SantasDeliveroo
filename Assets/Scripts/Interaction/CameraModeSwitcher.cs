using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraModeSwitcher : MonoBehaviour
{
    [Header("Camera rigs")]
    [Tooltip("Camera rig for free mode")]
    public Transform FreeMode;
    [Tooltip("Camera rig for tactical mode")]
    public Transform TacticalMode;

    // The current camera mode. False if free camera mode, true if tactical camera mode.
    private bool _cameraMode;

    /// <summary>
    /// Changes the camera mode from free to tactical and vice versa.
    /// </summary>
    /// <param name="context"></param>
    public void OnSpacebarPressed(InputAction.CallbackContext context)
    {
        _cameraMode = !_cameraMode;
        GameInfoPanel.Instance.UpdateCameraMode(_cameraMode);

        FreeMode.gameObject.SetActive(!_cameraMode);
        TacticalMode.gameObject.SetActive(_cameraMode);
    }
}
