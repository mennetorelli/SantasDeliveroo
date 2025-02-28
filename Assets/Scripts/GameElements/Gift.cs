﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gift : SelectableElementBase, ITarget
{
    public List<Color> Colors;

    public string Id { get; set; }
    public House DestinationHouse { get; set; }

    public Santa CollectedBySanta { get; set; }


    protected override void Awake()
    {
        base.Awake();

        // Assignment of random color to the material.
        transform.GetChild(0).GetComponent<Renderer>().material.color = Colors[Random.Range(0, Colors.Count - 1)];

        // Assignment of a gift id, composed of color and random number
        Id = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 10)
            .Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

    public void TargetReached(Santa santa)
    {
        // To avoid two santa collecting the gift simultaneously.
        GetComponent<Collider>().enabled = false;

        if (santa.CollectedGifts.Count < santa.SleighCapacity)
        {
            CollectedBySanta = santa;
            santa.CollectedGifts.Add(this);
            santa.UpdateSpeed();
            GameManager.Instance.GiftsInMap.Remove(this);
            StartCoroutine(Deactivate());
            ElementDetailsPanel.Instance.UpdatePanel();
            MessagePanel.Instance.ShowMessage($"Gift collected! Gift's destination: {DestinationHouse.HouseAddress}", Color.black);
        }
        else
        {
            MessagePanel.Instance.ShowMessage("Sleigh full! Deliver some gifts before collecting another one.", Color.red);
        }
    }

    /// <summary>
    /// Smoothly scales the object, then deactivates it.
    /// </summary>
    /// <returns>IEnumerator.</returns>
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
