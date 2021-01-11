using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SantaBehaviour : MonoBehaviour, ISelectable
{
    public GameObject PathFeedback;
    public float Speed = 1f;
    
    private readonly List<KeyValuePair<Vector3, UnityAction>> _instructionsQueue = new List<KeyValuePair<Vector3, UnityAction>>();
    private readonly List<GameObject> _pathList = new List<GameObject>();
    private KeyValuePair<Vector3, UnityAction> _currentInstruction;
    private GameObject _currentPath;

    public GameObject NextTarget;
    public List<GameObject> PickedUpGifts;


    void Awake()
    {
        PickedUpGifts = new List<GameObject>();
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

    public void Selected()
    {

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

        // Create a line indicating the path of this action.
        LineRenderer lineRenderer = PathFeedback.GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Starting point of the line.
        lineRenderer.SetPosition(0, _instructionsQueue.Count > 0 ? _instructionsQueue[_instructionsQueue.Count - 1].Key : (_currentPath != null ? _currentPath.transform.position : transform.position));
        // Ending point of the line.
        lineRenderer.SetPosition(1, targetPosition);

        // Instantiate the line renderer.
        _pathList.Add(Instantiate(PathFeedback, targetPosition, PathFeedback.transform.rotation));
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
        float step = (Speed / (origin - destination).magnitude) * Time.fixedDeltaTime;
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
        float start_time = Time.time;
        Vector3 initial_scale_value = transform.localScale;
        while (ratio < 1)
        {
            // update the ratio value at every frame
            ratio = (Time.time - start_time) / duration;
            // apply the new scale
            transform.localScale = Vector3.Lerp(initial_scale_value, Vector3.zero, ratio); 
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
