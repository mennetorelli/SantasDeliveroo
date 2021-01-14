using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the top-left menu, with informations about the gameplay.
/// </summary>
public class GameInfoPanel : MonoBehaviour
{
    [Tooltip("Reference to the label containing the level.")]
    public TextMeshProUGUI Level;
    [Tooltip("Reference to to the label containing the time left.")]
    public TextMeshProUGUI Timer;
    [Tooltip("Reference to the label containing the number of gifts to deliver.")]
    public TextMeshProUGUI Gifts;
    [Tooltip("Reference to the label containing the number of remaining santas.")]
    public TextMeshProUGUI Santas;
    [Tooltip("Reference to the label containing the active camera mode.")]
    public TextMeshProUGUI CameraMode;
    [Tooltip("Reference to controls info panel.")]
    public GameObject ControlsInfoPanel;

    private string _giftsText;
    private string _timerText;
    private string _cameraModeText;
    private string _santasText;
    private bool _controlsInfoPanelActive;

    public static GameInfoPanel Instance
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
        Timer.text = $"{ _timerText } { GameManager.Instance.TimeLeft:0} ";
    }

    // Start is called before the first frame update
    void Start()
    {
        Level.text = $"{ Level.text } { LoadSettings.Instance.SelectedLevel.Id }";
        _timerText = Timer.text;
        _giftsText = Gifts.text;
        _santasText = Santas.text;
        _cameraModeText = CameraMode.text;

        UpdateGifts(LoadSettings.Instance.SelectedLevel.GiftsToDeliver);
        UpdateSantas(LoadSettings.Instance.SelectedLevel.NumberOfSantas);
        UpdateCameraMode(false);
    }

    public void UpdateGifts(int gifts)
    {
        Gifts.text = $"{ _giftsText } { gifts }";
    }

    public void UpdateSantas(int santas)
    {
        Santas.text = $"{ _santasText } { santas }";
    }

    public void UpdateCameraMode(bool mode)
    {
        CameraMode.text = $"{ _cameraModeText } { (mode ? "Tactical" : "Free") }";
    }


    /// <summary>
    /// Triggered when the user presses the key to enable/disable the controls info panel.
    /// </summary>
    /// <param name="context"></param>
    public void OnShowControlsInfoPanel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _controlsInfoPanelActive = !_controlsInfoPanelActive;
            ControlsInfoPanel.SetActive(_controlsInfoPanelActive);
        }
    }
}
