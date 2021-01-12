using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI Level;
    public Timer Timer;
    public TextMeshProUGUI Gifts;
    public TextMeshProUGUI CameraMode;

    private string _giftsText;
    private string _cameraModeText;

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

    // Start is called before the first frame update
    void Start()
    {
        Level.text = $"{ Level.text } { LoadSettings.Instance.SelectedLevel.Time }";
        Timer.StartTimer(LoadSettings.Instance.SelectedLevel.Time);
        _giftsText = Gifts.text;
        _cameraModeText = CameraMode.text;

        UpdateGifts(LoadSettings.Instance.SelectedLevel.GiftsToDeliver);
        UpdateCameraMode(false);
    }

    public void UpdateGifts(int gifts)
    {
        Gifts.text = $"{ _giftsText } { gifts }";
    }

    public void UpdateCameraMode(bool mode)
    {
        CameraMode.text = $"{ _cameraModeText } { (mode ? "Tactical" : "Free" ) }";
    }
}
