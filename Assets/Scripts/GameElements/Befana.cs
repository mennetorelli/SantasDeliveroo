using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Befana : SelectableElementBase
{
    [Tooltip("The collider representing the detection range of the Befana.")]
    public SphereCollider DetectionRange;

    private int _speed;
    private int _actionDuration;
    private readonly int _maxDistance = 10;

    private Santa _detectedSanta;
    private Vector3 _randomPosition;

    protected override void Awake()
    {
        base.Awake();

        // Initialize parameters according to level configuration.
        LevelConfiguration selectedLevel = LoadSettings.Instance.SelectedLevel;
        _speed = Random.Range(selectedLevel.BefanasMinSpeed, selectedLevel.BefanasMaxSpeed);
        _actionDuration = Random.Range(selectedLevel.BefanasMinActionDuration, selectedLevel.BefanasMaxActionDuration);
        DetectionRange.radius = Random.Range(selectedLevel.BefanasMinDetectionRange, selectedLevel.BefanasMaxDetectionRange);

        // Update Befana's random action every _actionDuration seconds.
        InvokeRepeating(nameof(UpdateRandomPosition), 0, _actionDuration);
    }


    // Update is called once per frame
    void Update()
    {
        // if there is a detected santa which is not already chased by other befanas.
        if (_detectedSanta != null && _detectedSanta.IsChasedBy == this)
        {
            Highlight.SetActive(true);

            Quaternion rotation = Quaternion.LookRotation(_detectedSanta.transform.position - transform.position);
            rotation.x = 0f;
            transform.rotation = rotation;

            transform.position = Vector3.MoveTowards(transform.position, _detectedSanta.transform.position, _speed * Time.deltaTime);

            // If the Befana has come quite near the Santa, deactivate both of them.
            // Used this instead of a OnTriggerEnter to not interfere with the collider of the detection range
            if (Vector3.Distance(transform.position, _detectedSanta.transform.position) < 0.5f)
            {
                StartCoroutine(_detectedSanta.Deactivate());
                StartCoroutine(Deactivate());

                // Set _detectedSanta = null to not enter again in the if condition and avoid starting again the coroutines.
                _detectedSanta = null;
            }
        }
        else
        {
            Highlight.SetActive(false);

            Quaternion rotation = Quaternion.LookRotation(_randomPosition - transform.position);
            rotation.x = 0f;
            transform.rotation = rotation;

            transform.position = Vector3.MoveTowards(transform.position, _randomPosition, _speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Generates a new random position used for the free movements of the Befana.
    /// </summary>
    void UpdateRandomPosition()
    {
        _randomPosition = new Vector3(
                Mathf.Clamp(Random.Range(transform.position.x - _maxDistance, transform.position.x + _maxDistance),
                    -GameManager.Instance.MaxRangeXZ, GameManager.Instance.MaxRangeXZ),
                Mathf.Clamp(Random.Range(transform.position.y - _maxDistance, transform.position.y + _maxDistance),
                    GameManager.Instance.MinHeight, GameManager.Instance.MaxHeight),
                Mathf.Clamp(Random.Range(transform.position.z - _maxDistance, transform.position.z + _maxDistance),
                    -GameManager.Instance.MaxRangeXZ, GameManager.Instance.MaxRangeXZ));
    }

    void OnTriggerEnter(Collider other)
    {
        if (_detectedSanta == null)
        {
            _detectedSanta = other.GetComponent<Santa>();

            // If the detected Santa is not chased by any Befana, now it's followed by this Befana.
            if (_detectedSanta.IsChasedBy == null)
            {
                _detectedSanta.IsChasedBy = this;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == _detectedSanta)
        {
            _detectedSanta.IsChasedBy = null;
            _detectedSanta = null;
        }
    }

    /// <summary>
    /// Smoothly scales the object, then deactivates it.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    IEnumerator Deactivate()
    {
        float step = 0;
        float duration = 1f;
        float startTime = Time.time;
        Vector3 initialScaleValue = transform.localScale;
        while (step < 1)
        {
            // update the ratio value at every frame.
            step = (Time.time - startTime) / duration;
            // apply the new scale.
            transform.localScale = Vector3.Lerp(initialScaleValue, Vector3.zero, step);
            yield return new WaitForEndOfFrame();
        }

        OnDeselect();
        gameObject.SetActive(false);
    }

    public override List<string> FormatProperties()
    {
        return new List<string>()
        {
            $"Speed: { _speed } m/s",
            $"Detection range: { DetectionRange.radius } m"
        };
    }
}
