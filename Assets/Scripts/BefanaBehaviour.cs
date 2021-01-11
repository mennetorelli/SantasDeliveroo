using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BefanaBehaviour : MonoBehaviour
{
    public float Speed = 1f;
    public float MaxDistance = 3f;

    private Transform _santaDetected;
    private Vector3 _randomPosition;

    void Awake()
    {
        InvokeRepeating(nameof(UpdateRandomPosition), 0, 3f);
    }


    // Update is called once per frame
    void Update()
    {
        if (_santaDetected != null)
        {
            transform.rotation = Quaternion.LookRotation(_santaDetected.position);
            transform.position = Vector3.MoveTowards(transform.position, _santaDetected.position, Speed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(_randomPosition);
            transform.position = Vector3.MoveTowards(transform.position, _randomPosition, Speed * Time.deltaTime);
        }
    }

    void UpdateRandomPosition()
    {
        _randomPosition = new Vector3(
                Mathf.Clamp(UnityEngine.Random.Range(transform.position.x - MaxDistance, transform.position.x + MaxDistance),
                    -GameManager.Instance.MaxRangeXZ, GameManager.Instance.MaxRangeXZ),
                Mathf.Clamp(UnityEngine.Random.Range(transform.position.y - MaxDistance, transform.position.y + MaxDistance),
                    GameManager.Instance.MinHeight, GameManager.Instance.MaxHeight),
                Mathf.Clamp(UnityEngine.Random.Range(transform.position.z - MaxDistance, transform.position.z + MaxDistance),
                    -GameManager.Instance.MaxRangeXZ, GameManager.Instance.MaxRangeXZ));
    }

    void OnTriggerEnter(Collider other)
    {
        if (_santaDetected == null)
        {
            _santaDetected = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == _santaDetected)
        {
            _santaDetected = null;
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
}
