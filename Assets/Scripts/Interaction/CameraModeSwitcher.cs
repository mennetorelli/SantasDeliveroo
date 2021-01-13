using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Contains the logic to weitch between free mode and tactical mode.
/// </summary>
public class CameraModeSwitcher : MonoBehaviour
{
    [Header("Camera rigs")]
    [Tooltip("Camera rig for free mode")]
    public Transform FreeMode;
    [Tooltip("Camera rig for tactical mode")]
    public Transform TacticalMode;

    [Header("Fade in/out")]
    [Tooltip("Image for free mode camera.")]
    public RawImage FreeModeFadeImage;
    [Tooltip("Image for tactical mode camera.")]
    public RawImage TacticalModeFadeImage;
    [Tooltip("Fade duration.")]
    public float FadeDuration = 0.2f;

    // The current camera mode. False if free camera mode, true if tactical camera mode.
    private bool _cameraMode;

    void Awake()
    {
        FreeModeFadeImage.gameObject.SetActive(true);
        TacticalModeFadeImage.gameObject.SetActive(true);

        FreeModeFadeImage.canvasRenderer.SetAlpha(0);
        TacticalModeFadeImage.canvasRenderer.SetAlpha(1);
    }

    /// <summary>
    /// Changes the camera mode from free to tactical and vice versa.
    /// </summary>
    /// <param name="context"></param>
    public void CameraViewChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _cameraMode = !_cameraMode;
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {
        // If we have changed to free mode.
        if (!_cameraMode)
        {
            TacticalModeFadeImage.CrossFadeAlpha(1, FadeDuration, false);
            yield return new WaitForSeconds(FadeDuration);

            // Set active the appropriate camera rig.
            FreeMode.gameObject.SetActive(true);
            TacticalMode.gameObject.SetActive(false);

            FreeModeFadeImage.CrossFadeAlpha(0, FadeDuration, false);
        }
        // If we have changed to tactical mode.
        else
        {
            FreeModeFadeImage.CrossFadeAlpha(1, FadeDuration, false);
            yield return new WaitForSeconds(FadeDuration);

            // Set active the appropriate camera rig.
            TacticalMode.gameObject.SetActive(true);
            FreeMode.gameObject.SetActive(false);

            TacticalModeFadeImage.CrossFadeAlpha(0, FadeDuration, false);
        }

        GameInfoPanel.Instance.UpdateCameraMode(_cameraMode);
    }
}
