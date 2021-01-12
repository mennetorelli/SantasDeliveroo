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
    private KeyValuePair<Vector3, UnityAction> _currentInstruction;
    private GameObject _currentPath;

    private double _speed;
    private int _initialSpeed;

    public GameObject NextTarget { get; set; }
    public List<Gift> CollectedGifts { get; set; }
    public Vector3 Origin { get => _instructionsQueue.Count > 0 ? _instructionsQueue[_instructionsQueue.Count - 1].Key : (_currentPath != null ? _currentPath.transform.position : transform.position); }

    void Awake()
    {
        _initialSpeed = Random.Range(LoadSettings.Instance.SelectedLevel.SantasMinSpeed, LoadSettings.Instance.SelectedLevel.SantasMaxSpeed);
        _speed = _initialSpeed;
       
        CollectedGifts = new List<Gift>();
    }

    void Update()
    {
        // If there are instrucions to execute, do it.
        if (_currentInstruction.Value == null && _instructionsQueue.Count > 0)
        {
            // Referencing the first instruction and path of the lists.
            _currentInstruction = _instructionsQueue[0];
            _currentPath = _pathList[0];
            _currentPath.GetComponent<Renderer>().material.color = Color.green;

            // Removing the first instruction and path from the lists.
            _instructionsQueue.Remove(_currentInstruction);
            _pathList.Remove(_currentPath);

            // Invoking the current UnityEvent.
            _currentInstruction.Value.Invoke();
        }
    }

    public void AddActionToQueue(Vector3 targetPosition, UnityAction action, bool appendInstruction)
    {
        if (!appendInstruction)
        {
            _instructionsQueue.Clear();
            foreach (GameObject path in _pathList)
            {
                Destroy(path);
            }
            _pathList.Clear();
        }

        // Add the action in the queue.
        _instructionsQueue.Add(new KeyValuePair<Vector3, UnityAction>(targetPosition, action));

        //// Create a line indicating the path of this action.
        //LineRenderer lineRenderer = PathVisualFeedback.GetComponent<LineRenderer>();
        //lineRenderer.startColor = Color.black;
        //lineRenderer.endColor = Color.black;
        //lineRenderer.startWidth = 0.01f;
        //lineRenderer.endWidth = 0.01f;
        //lineRenderer.positionCount = 2;
        //lineRenderer.useWorldSpace = true;

        //// Starting point of the line.
        //lineRenderer.SetPosition(0, _instructionsQueue.Count > 0 ? _instructionsQueue[_instructionsQueue.Count - 1].Key : (_currentPath != null ? _currentPath.transform.position : transform.position));
        //// Ending point of the line.
        //lineRenderer.SetPosition(1, targetPosition);

        //// Instantiate the line renderer.
        //_pathList.Add(Instantiate(PathVisualFeedback, targetPosition, PathVisualFeedback.transform.rotation));

        GameObject visualFeedback = Instantiate(PathVisualFeedback);
        _pathList.Add(visualFeedback);

        Vector3 origin = Origin;
 
        visualFeedback.GetComponent<VisualFeedbackDrawer>().DrawPath(origin, targetPosition);
        visualFeedback.GetComponent<VisualFeedbackDrawer>().DrawDestination(targetPosition);
    }

    public void ExecuteAction(GameObject target = null)
    {
        NextTarget = target;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.LookRotation(_currentInstruction.Key);
        StartCoroutine(MoveSanta(origin, _currentInstruction.Key));
    }

    IEnumerator MoveSanta(Vector3 origin, Vector3 destination)
    {
        float step = (float)((_speed / (origin - destination).magnitude) * Time.fixedDeltaTime);
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
        Destroy(_currentPath);
        // To set the next path to green.
        _currentPath = null;
        _currentInstruction = new KeyValuePair<Vector3, UnityAction>();
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.GetComponent<BefanaBehaviour>())
    //    {
    //        StartCoroutine(Deactivate());
    //        StartCoroutine(other.gameObject.GetComponent<BefanaBehaviour>().Deactivate());
    //    }
    //}

    IEnumerator Deactivate()
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
        _speed -= _initialSpeed / 2 / CollectedGifts.Count;
    }

    public override void UpdateInfoInPanel()
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
        ElementDetailsPanel.Instance.ShowPanel(Icon, elementProperties);
    }
}
