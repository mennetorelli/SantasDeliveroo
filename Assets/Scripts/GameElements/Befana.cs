using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Befana : SelectableElementBase
{
    private int _speed;
    private int _actionDuration;
    private int _fovRadius;
    private int _maxDistance = 10;

    private Transform _detectedSanta;
    private Vector3 _randomPosition;

    void Awake()
    {
        // Initialize parameters according to level configuration.
        LevelConfiguration selectedLevel = LoadSettings.Instance.SelectedLevel;
        _speed = UnityEngine.Random.Range(selectedLevel.BefanasMinSpeed, selectedLevel.BefanasMaxSpeed);
        _actionDuration = UnityEngine.Random.Range(selectedLevel.BefanasMinActionDuration, selectedLevel.BefanasMaxActionDuration);
        _fovRadius = 100;//UnityEngine.Random.Range(selectedLevel.BefanasMinFOVRadius, selectedLevel.BefanasMaxFOVRadius);
        
        // Set the FOV radius of the Befana.
        GetComponent<SphereCollider>().radius = _fovRadius;

        // Update Befana's random action every _actionDuration seconds.
        InvokeRepeating(nameof(UpdateRandomPosition), 0, _actionDuration);
    }


    // Update is called once per frame
    void Update()
    {
        if (_detectedSanta != null)
        {
            transform.rotation = Quaternion.LookRotation(_detectedSanta.position);
            transform.position = Vector3.MoveTowards(transform.position, _detectedSanta.position, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _detectedSanta.position) < 1.1f)
            {
                StartCoroutine(Deactivate());
                StartCoroutine(_detectedSanta.GetComponent<Santa>().Deactivate());
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(_randomPosition);
            transform.position = Vector3.MoveTowards(transform.position, _randomPosition, _speed * Time.deltaTime);
        }
    }

    void UpdateRandomPosition()
    {
        _randomPosition = new Vector3(
                Mathf.Clamp(UnityEngine.Random.Range(transform.position.x - _maxDistance, transform.position.x + _maxDistance),
                    -GameManager.Instance.MaxRangeXZ, GameManager.Instance.MaxRangeXZ),
                Mathf.Clamp(UnityEngine.Random.Range(transform.position.y - _maxDistance, transform.position.y + _maxDistance),
                    GameManager.Instance.MinHeight, GameManager.Instance.MaxHeight),
                Mathf.Clamp(UnityEngine.Random.Range(transform.position.z - _maxDistance, transform.position.z + _maxDistance),
                    -GameManager.Instance.MaxRangeXZ, GameManager.Instance.MaxRangeXZ));
    }

    void OnTriggerEnter(Collider other)
    {
        if (_detectedSanta == null)
        {
            _detectedSanta = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == _detectedSanta)
        {
            _detectedSanta = null;
        }
    }

    public IEnumerator Deactivate()
    {
        float ratio = 0;
        float duration = 1f;
        float startTime = Time.time;
        Vector3 initialScaleValue = transform.localScale;
        while (ratio < 1)
        {
            // update the ratio value at every frame
            ratio = (Time.time - startTime) / duration;
            // apply the new scale
            transform.localScale = Vector3.Lerp(initialScaleValue, Vector3.zero, ratio);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }

    public override List<string> FormatDetails()
    {
        return  new List<string>()
        {
            $"Speed: { _speed }",
            $"FOV radius: { _fovRadius }"
        };
    }

    public override void Selected()
    {
        Debug.Log("Nothing to override");
    }

    public override void Deselected()
    {
        Debug.Log("Nothing to override");
    }
}
