using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Santa : SelectableElementBase
{
    [Tooltip("The amount of gifts Santa can collect.")]
    public int SleighCapacity = 5;
    [Tooltip("The visual feedback that represents the path of actions.")]
    public GameObject PathVisualFeedback;

    // Actions are stores in a queue of pairs <santa's destination, action>.
    private readonly List<KeyValuePair<Vector3, UnityAction>> _actionsQueue = new List<KeyValuePair<Vector3, UnityAction>>();
    // The visual representation of the path as a sequence of GameObjects containing line renderers.
    private readonly List<GameObject> _path = new List<GameObject>();

    private double _speed;
    private double _initialSpeed;

    private bool _isExecuting;

    public GameObject NextTarget { get; set; }
    public List<Gift> CollectedGifts { get; set; }
    public Vector3 Origin { get => _actionsQueue.Count > 0 ? _actionsQueue[_actionsQueue.Count - 1].Key : (_path.Count > 0 ? _path[0].transform.position : transform.position); }

    void Awake()
    {
        _initialSpeed = Random.Range(LoadSettings.Instance.SelectedLevel.SantasMinSpeed, LoadSettings.Instance.SelectedLevel.SantasMaxSpeed);
        _speed = _initialSpeed;

        CollectedGifts = new List<Gift>();
    }

    void Update()
    {
        // If there are actions in the list, take the first and execute it.
        if (!_isExecuting && _actionsQueue.Count > 0)
        {
            _isExecuting = true;
            _actionsQueue[0].Value.Invoke();
        }
    }

    public void AddActionToQueue(Vector3 destination, UnityAction action, bool appendAction)
    {
        // If there is a queue of actions and the user has not pressed ctrl, clear the queue 
        // (except the first action, which is in execution and will be removed at completion).
        if (!appendAction && _actionsQueue.Count > 0)
        {
            _actionsQueue.RemoveRange(1, _actionsQueue.Count - 1);
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Destroy(_path[i + 1]);
            }
            _path.RemoveRange(1, _path.Count - 1);
        }

        // Add the action in the queue.
        _actionsQueue.Add(new KeyValuePair<Vector3, UnityAction>(destination, action));

        Vector3 origin = GetStartingPoint(appendAction);

        // Instantiate a step of the path and adds it to the list.
        GameObject visualFeedback = Instantiate(PathVisualFeedback, destination, Quaternion.identity);
        _path.Add(visualFeedback);

        // Draw the step.
        PathRenderer visualFeedbackDrawer = visualFeedback.GetComponent<PathRenderer>();
        visualFeedbackDrawer.DrawPath(origin, destination);
        visualFeedbackDrawer.DrawDestination(destination);
    }

    /// <summary>
    /// Calculates the starting point of a new step of the path.
    /// </summary>
    /// <param name="appendAction">Whether the user is appending an action or not.</param>
    /// <returns>The starting point of a step in the path.</returns>
    public Vector3 GetStartingPoint(bool appendAction)
    {
        // Calculation of the starting point of the next step of the path.
        Vector3 origin = new Vector3();

        // If the path is empty (i.e. Santa is idle), the starting point is the position of Santa.
        if (_path.Count == 0)
        {
            origin = transform.position;
        }
        // If Santa is executing an action but there are no more actions in queue, 
        // the starting point is the destination of that action.
        if (_path.Count == 1)
        {
            origin = _path[0].transform.position;
        }
        // If there are more than one action in queue, 
        // then check whether the user has pressed ctrl (appendAction value), 
        // If appendAction = true, the starting point is the destination of the last action,
        // otherwise the the starting point is the destination of the first action (the one in execution).
        if (_path.Count > 1)
        {
            origin = appendAction ? _path[_path.Count - 1].transform.position : _path[0].transform.position;
        }

        return origin;
    }

    /// <summary>
    /// Represents an action: move Santa from one point to another and, if it is a collect or deliver action, 
    /// store a referent of the current target object.
    /// </summary>
    /// <param name="target">The target object, if the action is a collect or deliver one.</param>
    public void ExecuteAction(GameObject target = null)
    {
        // Saves a reference of the target object for collect/deliver actions,
        // to check if santa collides with the right object during the execution.
        NextTarget = target;

        // Update the color of the current step in the path according to the presence of a target object
        // (collect/deliver action) or not (move action).
        foreach (Renderer step in _path[0].transform.GetComponentsInChildren<Renderer>())
        {
            step.material.color = target != null ? Color.cyan : Color.green;
        }

        // Santa looks in the direction of the target position.
        Vector3 lookDirection = _actionsQueue[0].Key - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rotation.x = 0f;
        transform.rotation = rotation;

        // Start moving Santa.
        StartCoroutine(MoveSanta(transform.position, _actionsQueue[0].Key));
    }

    /// <summary>
    /// Moves Santa smoothly from its position to a destination position.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns>IEnumerator.</returns>
    IEnumerator MoveSanta(Vector3 origin, Vector3 destination)
    {
        float step = (float)(_speed / (origin - destination).magnitude * Time.fixedDeltaTime);
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

        // The action is completed, remove it from the list and destroy the visual feedback of the step.
        _actionsQueue.Remove(_actionsQueue[0]);
        Destroy(_path[0]);
        _path.Remove(_path[0]);
        _isExecuting = false;
    }

    /// <summary>
    /// Smoothly scales the object, then deactivates it.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    public IEnumerator Deactivate()
    {
        float step = 0;
        float duration = 1f;
        float startTime = Time.time;
        Vector3 initialScaleValue = transform.localScale;
        while (step < 1)
        {
            // Update the step value at every frame
            step = (Time.time - startTime) / duration;
            // Apply the new scale
            transform.localScale = Vector3.Lerp(initialScaleValue, Vector3.zero, step);
            yield return new WaitForEndOfFrame();
        }

        // If this Santa had collected some gifts, remove the reference to santa from the gifts.
        foreach (Gift gift in CollectedGifts)
        {
            gift.CollectedBySanta = null;
        }

        GameManager.Instance.DecreaseSantasCounter();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the speed of Santa after he has performed a collect/deliver action.
    /// </summary>
    public void UpdateSpeed()
    {
        // The value of the minimum speed is half of the initial speed. 
        // Then for each collected gift, decrement by that value / SleighCapacity
        double minSpeed = _initialSpeed / 2;
        double step = minSpeed / SleighCapacity;
        _speed = _initialSpeed - (step * CollectedGifts.Count);
    }

    public override List<string> FormatProperties()
    {
        return new List<string>()
        {
            $"Speed: { _speed } m/s",
            $"Available sleigh space: { 5 - CollectedGifts.Count } gifts",
            $"Gifts collected:\n{ string.Join(",\n", CollectedGifts.Select( el => $"{ el.Id }, { el.DestinationHouse.HouseAddress }") )}"
        };
    }

    public override void OnSelect()
    {
        base.OnSelect();

        // Show the path.
        foreach (GameObject step in _path)
        {
            step.SetActive(true);
        }

        // For each collected gift, show the destination house.
        foreach (Gift gift in CollectedGifts)
        {
            gift.DestinationHouse.Highlight.SetActive(true);
        }
    }

    public override void OnDeselect()
    {
        base.OnDeselect();

        // Stop showing the path.
        foreach (GameObject step in _path)
        {
            step.SetActive(false);
        }

        // For each collected gift, stop showing the destination house.
        foreach (Gift gift in CollectedGifts)
        {
            gift.DestinationHouse.Highlight.SetActive(false);
        }
    }
}
