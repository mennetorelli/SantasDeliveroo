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
    public float FadeDuration = 0.15f;

    private bool _tacticalMode;

    void Awake()
    {
        FreeModeFadeImage.gameObject.SetActive(true);
        TacticalModeFadeImage.gameObject.SetActive(true);

        FreeModeFadeImage.canvasRenderer.SetAlpha(0);
        TacticalModeFadeImage.canvasRenderer.SetAlpha(1);
    }

    /// <summary>
    /// Triggers when the user changes the camera mode from free to tactical and vice versa.
    /// </summary>
    /// <param name="context">Holds context information of the state of the Action and the values of the controls.</param>
    public void OnCameraViewChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _tacticalMode = !_tacticalMode;
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {
        // If we have changed to free mode.
        if (!_tacticalMode)
        {
            // Fade out and wait for the completion before switching the camera.
            TacticalModeFadeImage.CrossFadeAlpha(1, FadeDuration, false);
            yield return new WaitForSeconds(FadeDuration);

            // Set active the appropriate camera rig.
            FreeMode.gameObject.SetActive(true);
            TacticalMode.gameObject.SetActive(false);

            // Fade in.
            FreeModeFadeImage.CrossFadeAlpha(0, FadeDuration, false);
        }
        // If we have changed to tactical mode.
        else
        {
            // Fade out and wait for the completion before switching the camera.
            FreeModeFadeImage.CrossFadeAlpha(1, FadeDuration, false);
            yield return new WaitForSeconds(FadeDuration);

            // Set active the appropriate camera rig.
            TacticalMode.gameObject.SetActive(true);
            FreeMode.gameObject.SetActive(false);

            // Fade in.
            TacticalModeFadeImage.CrossFadeAlpha(0, FadeDuration, false);
        }

        GameInfoPanel.Instance.UpdateCameraMode(_tacticalMode);
    }
}
