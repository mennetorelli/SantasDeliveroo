using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI TimerText;

    private float _timeLeft = 0;
    private bool _counting;
    private string _text;

    private void Awake()
    {
        _text = TimerText.text;
    }

    void Update()
    {
        if (_counting)
        {
            _timeLeft -= Time.deltaTime;
            TimerText.text = $"{ _text } { _timeLeft:0} ";
            if (_timeLeft < 0)
            {
                _counting = false;
            }
        }

    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    /// <param name="timer">The value of the timer.</param>
    public void StartTimer(float timer)
    {
        _timeLeft = timer;
        _counting = true;
    }
}
