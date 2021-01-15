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

    // Actions are stored in a list of pairs <santa's destination, action>.
    private readonly List<KeyValuePair<Vector3, UnityAction>> _actionsList = new List<KeyValuePair<Vector3, UnityAction>>();
    // The visual representation of the path as a sequence of GameObjects containing line renderers.
    private readonly List<GameObject> _path = new List<GameObject>();

    private double _speed;
    private double _initialSpeed;

    private bool _isExecuting;

    private Coroutine _executingAction;

    public GameObject TargetElement { get; set; }
    public List<Gift> CollectedGifts { get; set; }
    public Befana IsChasedBy { get; set; }

    public bool IsAutomaticMode { get; set; }

    protected override void Awake()
    {
        base.Awake();

        _initialSpeed = Random.Range(LoadSettings.Instance.SelectedLevel.SantasMinSpeed, LoadSettings.Instance.SelectedLevel.SantasMaxSpeed);
        _speed = _initialSpeed;

        CollectedGifts = new List<Gift>();
    }

    void Update()
    {
        // If there are actions in the list, take the first and execute it.
        if (!_isExecuting && _actionsList.Count > 0)
        {
            _isExecuting = true;
            _actionsList[0].Value.Invoke();
        }
    }

    public void AddActionToQueue(Vector3 destination, UnityAction action, bool appendAction)
    {
        // If there is a queue of actions and the user has not pressed ctrl, clear the queue 
        // (except the first action, which is in execution and will be removed at completion).
        if (!appendAction && _actionsList.Count > 0)
        {
            _actionsList.RemoveRange(1, _actionsList.Count - 1);
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Destroy(_path[i + 1]);
            }
            _path.RemoveRange(1, _path.Count - 1);
        }

        // Add the action in the queue.
        _actionsList.Add(new KeyValuePair<Vector3, UnityAction>(destination, action));

        Vector3 origin = GetStartingPoint(appendAction);

        // Instantiate a step of the path and adds it to the list.
        GameObject visualFeedback = Instantiate(PathVisualFeedback, destination, Quaternion.identity);
        _path.Add(visualFeedback);


        // Draw the step.
        PathRenderer visualFeedbackDrawer = visualFeedback.GetComponent<PathRenderer>();
        visualFeedbackDrawer.DrawDestination(destination);
        visualFeedbackDrawer.DrawPath(appendAction ? origin : transform.position, destination);

        if (!appendAction && _isExecuting)
        {
            StopCoroutine(_executingAction);

            // The action is overwritten, remove it from the list and destroy the visual feedback of the step.
            OnActionCompleted();
        }
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
        // If there are more than one action in queue, 
        // then check whether the user has pressed ctrl (appendAction value), 
        // If appendAction = true, the starting point is the destination of the last action,
        // otherwise the the starting point is the destination of the first action (the one in execution).
        if (_path.Count > 0)
        {
            origin = appendAction ? _path[_path.Count - 1].transform.position : transform.position;
        }

        return origin;
    }

    /// <summary>
    /// Represents an action: move Santa from one point to another and, if it is a collect/deliver action, 
    /// store a reference of the current target object.
    /// </summary>
    /// <param name="targetElement">The target object, if the action is a collect or deliver one.</param>
    public void ExecuteAction(GameObject targetElement = null)
    {
        // Saves a reference of the target object for collect/deliver actions,
        // to check if santa collides with the right object during the execution.
        TargetElement = targetElement;

        // Update the color of the current step in the path according to the presence of a target object
        // (collect/deliver action) or not (move action).
        foreach (Renderer step in _path[0].transform.GetComponentsInChildren<Renderer>())
        {
            step.material.color = targetElement != null ? Color.cyan : Color.green;
        }

        // Santa looks in the direction of the target position.
        Vector3 lookDirection = _actionsList[0].Key - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rotation.x = 0f;
        transform.rotation = rotation;

        // Start moving Santa.
        _executingAction = StartCoroutine(MoveSanta(transform.position, _actionsList[0].Key));
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
        OnActionCompleted();
    }

    /// <summary>
    /// Removes the first action and the step of the past from the respective lists.
    /// </summary>
    void OnActionCompleted()
    {
        _actionsList.Remove(_actionsList[0]);
        Destroy(_path[0]);
        _path.Remove(_path[0]);
        _isExecuting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // If the colliding object is a target object and corresponds to the target of the action
        // (i.e. it's not an unintentional collision).
        if (other.GetComponent<ITarget>() != null && other.gameObject == TargetElement)
        {
            other.GetComponent<ITarget>().TargetReached(this);

            //To avoid repeating the triggering of the target reached method.
            TargetElement = null;
        }
    }

    /// <summary>
    /// Smoothly scales the object, then deactivates it.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    public IEnumerator Deactivate()
    {
        OnDeselect();
        MessagePanel.Instance.ShowMessage("A Santa has been chased by a Befana!", Color.red);
        if (InteractionManager.Instance.SelectedElement == this)
        {
            ElementDetailsPanel.Instance.HidePanel();
            InteractionManager.Instance.SelectedElement = null;
        }
        
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

        GameManager.Instance.RemoveSanta(this);
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

    public void AutomaticMode()
    {
        IsAutomaticMode = !IsAutomaticMode;

        if (IsAutomaticMode)
        {
            // Deactivate all the befanas.
            foreach (Befana befana in GameManager.Instance.Befanas)
            {
                befana.gameObject.SetActive(false);
            }

            // Simulate the position of Santa at each step: the starting point is Santa's transform.
            Vector3 currentPlannedPosition = transform.position;

            // Simulate the gifts present in the map.
            List<SelectableElementBase> giftsInMap = new List<SelectableElementBase>(GameManager.Instance.GiftsInMap);

            // Simulate the gifts collected at each step.
            List<Gift> giftsPlannedToCollect = new List<Gift>(CollectedGifts);

            // Simulate the number of gifts to deliver.
            int plannedGiftsToDeliverCounter = GameManager.Instance.GiftsToDeliverCounter;

            while (giftsInMap.Count > 0 && plannedGiftsToDeliverCounter > 0)
            {
                // If Santa hasn't collected any gift, no houses can be target object.
                if (giftsPlannedToCollect.Count == 0)
                {
                    Gift nearestGift = (Gift)SelectNearestarget(giftsInMap, currentPlannedPosition);
                    currentPlannedPosition = nearestGift.DestinationArea.bounds.center;

                    giftsPlannedToCollect.Add(nearestGift);
                    giftsInMap.Remove(nearestGift);

                    AddActionToQueue(currentPlannedPosition, () => ExecuteAction(nearestGift.gameObject), true);
                }
                // If Santa has collected 5 gifts, only houses can be target objects.
                else if (giftsPlannedToCollect.Count == SleighCapacity)
                {
                    House nearestHouse = (House)SelectNearestarget(giftsPlannedToCollect.Select(el => (SelectableElementBase)el.DestinationHouse).ToList(), currentPlannedPosition);

                    currentPlannedPosition = nearestHouse.DestinationArea.bounds.center;

                    List<Gift> deliverableGifts = Enumerable.Intersect(nearestHouse.RequestedGifts, giftsPlannedToCollect).ToList();
                    giftsPlannedToCollect = giftsPlannedToCollect.Except(deliverableGifts).ToList();

                    plannedGiftsToDeliverCounter -= (deliverableGifts.Count);

                    AddActionToQueue(currentPlannedPosition, () => ExecuteAction(nearestHouse.gameObject), true);
                }
                // If Santa has collected a number of object greater than zero but lesser than 5.
                else
                {
                    // Merge the list of remaining gifts with houses associated to collected gifts.
                    List<SelectableElementBase> possibleTargets =
                        giftsPlannedToCollect.Select(el => el.DestinationHouse)
                        .Concat(giftsInMap)
                        .ToList();

                    SelectableElementBase nearestObject = SelectNearestarget(possibleTargets, currentPlannedPosition);
                    currentPlannedPosition = nearestObject.DestinationArea.bounds.center;

                    if (nearestObject.GetType() == typeof(Gift))
                    {
                        giftsPlannedToCollect.Add((Gift)nearestObject);
                        giftsInMap.Remove(nearestObject);
                    }
                    if (nearestObject.GetType() == typeof(House))
                    {
                        House nearestHouse = (House)nearestObject;

                        List<Gift> deliverableGifts = Enumerable.Intersect(nearestHouse.RequestedGifts, giftsPlannedToCollect).ToList();
                        giftsPlannedToCollect = giftsPlannedToCollect.Except(deliverableGifts).ToList();

                        plannedGiftsToDeliverCounter -= (deliverableGifts.Count);
                    }

                    AddActionToQueue(currentPlannedPosition, () => ExecuteAction(nearestObject.gameObject), true);
                }
            }
        }
        else
        {
            // Activates all the befanas.
            foreach (Befana befana in GameManager.Instance.Befanas)
            {
                befana.gameObject.SetActive(true);
            }

            StopCoroutine(_executingAction);

            // Delete all the actions planned and the path.
            _actionsList.Clear();
            foreach (GameObject step in _path)
            {
                Destroy(step);
            }
            _path.Clear();
            _isExecuting = false;
        }
    }

    SelectableElementBase SelectNearestarget(List<SelectableElementBase> objectList, Vector3 currentPlannedPosition)
    {
        return objectList.OrderBy(x => Vector3.Distance(currentPlannedPosition, x.transform.position)).ToList()[0];
    }
}
