using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gift : SelectableElementBase
{
    public List<Color> Colors;

    public string Id { get; set; }
    public House DestinationHouse { get; set; }

    public Santa CollectedBySanta { get; set; }


    void Awake()
    {
        // Assignment of random color to the material.
        transform.GetChild(0).GetComponent<Renderer>().material.color = Colors[Random.Range(0, Colors.Count - 1)];

        // Assignment of a gift id, composed of color and random number
        Id = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 10)
            .Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

    void OnTriggerEnter(Collider other)
    {
        Santa currentSanta = other.GetComponent<Santa>();
        // If the colliding Santa is aiming to collect this gift, i.e. it is not an accidental collision.
        if (currentSanta != null && currentSanta.NextTarget == gameObject)
        {
            if (currentSanta.CollectedGifts.Count < currentSanta.SleighCapacity)
            {
                CollectedBySanta = currentSanta;
                currentSanta.CollectedGifts.Add(this);
                currentSanta.UpdateSpeed();
                StartCoroutine(Deactivate());
                ElementDetailsPanel.Instance.UpdatePanel();
                MessagePanel.Instance.ShowMessage($"Gift collected! Gift's destination: {DestinationHouse.HouseAddress}", Color.black);
            }
            else
            {
                MessagePanel.Instance.ShowMessage("Sleigh full! Deliver some gifts before collecting another one.", Color.red);
            }
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

    public override List<string> FormatProperties()
    {
        return new List<string>()
        {
            $"ID: {Id}",
            $"Destination house: {DestinationHouse.HouseAddress}"
        };
    }

    public override void OnSelect()
    {
        base.OnSelect();
        DestinationHouse.Highlight.SetActive(true);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        DestinationHouse.Highlight.SetActive(false);
    }
}
