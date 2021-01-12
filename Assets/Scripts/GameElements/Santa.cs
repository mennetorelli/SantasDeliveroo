using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Santa : SelectableElementBase
{
    [Tooltip("The number of gifts Santa can collect.")]
    public int SleighCapacity = 5;
    [Tooltip("The visual feedback that represents the path of Santa.")]
    public GameObject PathVisualFeedback;
    
    private readonly List<KeyValuePair<Vector3, UnityAction>> _instructionsQueue = new List<KeyValuePair<Vector3, UnityAction>>();
    private readonly List<GameObject> _pathList = new List<GameObject>();

    private double _speed;
    private double _initialSpeed;

    private bool _isExecuting;

    public GameObject NextTarget { get; set; }
    public List<Gift> CollectedGifts { get; set; }
    public Vector3 Origin { get => _instructionsQueue.Count > 0 ? _instructionsQueue[_instructionsQueue.Count - 1].Key : (_pathList.Count > 0 ? _pathList[0].transform.position : transform.position); }

    void Awake()
    {
        _initialSpeed = Random.Range(LoadSettings.Instance.SelectedLevel.SantasMinSpeed, LoadSettings.Instance.SelectedLevel.SantasMaxSpeed);
        _speed = _initialSpeed;
       
        CollectedGifts = new List<Gift>();
    }

    void Update()
    {
        // If there are instrucions to execute, do it.
        if (!_isExecuting && _instructionsQueue.Count > 0)
        {
            _isExecuting = true;
            _instructionsQueue[0].Value.Invoke();
        }
    }

    public void AddActionToQueue(Vector3 targetPosition, UnityAction action, bool appendInstruction)
    {
        if (!appendInstruction && _instructionsQueue.Count > 0)
        {
            _instructionsQueue.RemoveRange(1, _instructionsQueue.Count - 1);
            for (int i = 0; i < _pathList.Count - 1; i++)
            {
                Destroy(_pathList[i + 1]);
            }
            _pathList.RemoveRange(1, _pathList.Count - 1);
        }

        // Add the action in the queue.
        _instructionsQueue.Add(new KeyValuePair<Vector3, UnityAction>(targetPosition, action));

        // Instantiate a step of the path and adds it to the list.
        GameObject visualFeedback = Instantiate(PathVisualFeedback, targetPosition, transform.rotation, transform);
        _pathList.Add(visualFeedback);
        
        Vector3 origin = new Vector3();
        if (_pathList.Count == 0)
        {
            origin = transform.position;
        }
        if (_pathList.Count == 1)
        {
            origin = _pathList[0].transform.position;
        }
        if (_pathList.Count > 2)
        {
            origin = appendInstruction ? _pathList[_pathList.Count - 2].transform.position : _pathList[0].transform.position;
        }

        PathRenderer visualFeedbackDrawer = visualFeedback.GetComponent<PathRenderer>();
        visualFeedbackDrawer.DrawPath(origin, targetPosition);
        visualFeedbackDrawer.DrawDestination(targetPosition);
    }

    public void ExecuteAction(GameObject target = null)
    {
        NextTarget = target;
        foreach (Renderer path in _pathList[0].transform.GetComponentsInChildren<Renderer>())
        {
            path.material.color = target != null ? Color.cyan : Color.green;
        }
        Vector3 origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.LookRotation(_instructionsQueue[0].Key);
        StartCoroutine(MoveSanta(origin, _instructionsQueue[0].Key));
    }

    IEnumerator MoveSanta(Vector3 origin, Vector3 destination)
    {
        float step = (float)((_speed / (origin - destination).magnitude)* Time.fixedDeltaTime);
        float currentStep = 0;
        while (currentStep <= 1.0f)
        {
            // Goes from 0 to 1, incrementing by step each time.
            currentStep += step;
            // Move objectToMove closer to destination.
            transform.position = Vector3.Lerp(origin, destination, currentStep);
            // Leave the routine and return here in the next frame.
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination;

        _instructionsQueue.Remove(_instructionsQueue[0]);
        Destroy(_pathList[0]);
        _pathList.Remove(_pathList[0]);
        _isExecuting = false;
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

    public void UpdateSpeed()
    {
        double step = _initialSpeed / 2 / SleighCapacity;
        _speed = _initialSpeed - (step * CollectedGifts.Count);
    }

    public override List<string> FormatDetails()
    {
        List<string> elementProperties = new List<string>()
        {
            $"Speed: { _speed }",
            $"Remaining sleigh capacity: { 5 - CollectedGifts.Count } gifts"
        };
        foreach (Gift gift in CollectedGifts)
        {
            elementProperties.Add($"Gift in sleigh: { gift.Id }");
        }
        return elementProperties;
    }

    public override void Selected()
    {
        foreach (GameObject path in _pathList)
        {
            Debug.Log("seleziono" + path);
            path.SetActive(true);
        }
    }

    public override void Deselected()
    {
        foreach (GameObject path in _pathList)
        {
            Debug.Log("deseleziono"+path);
            path.SetActive(false);
        }
    }
}
