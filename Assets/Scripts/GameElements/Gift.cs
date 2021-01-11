using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gift : SelectableElementBase, ITarget
{
    public List<Color> Colors;

    public string Id { get; set; }
    public House DestinationHouse { get; set; }


    void Awake()
    {
        // Assignment of random color to the material.
        GetComponent<Renderer>().material.color = Colors[Random.Range(0, Colors.Count - 1)];

        // Assignment of a gift id, composed of color and random number
        Id = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 10)
            .Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

    void OnTriggerEnter(Collider other)
    {
        Santa currentSanta = other.GetComponent<Santa>();
        if (currentSanta != null && currentSanta.NextTarget == gameObject)
        {
            if (currentSanta.CollectedGifts.Count < currentSanta.SleighCapacity)
            {
                currentSanta.CollectedGifts.Add(this);
                currentSanta.UpdateSpeed();
                currentSanta.UpdateInfoInPanel();
                gameObject.SetActive(false);
                MessagePanel.Instance.ShowMessage($"Gift collected! Gift's destination: {DestinationHouse.HouseAddress}", Color.black);
            }
            else
            {
                MessagePanel.Instance.ShowMessage("Sleigh full! Deliver some gifts before collecting another one.", Color.red);
            }
        }
    }

    public override void UpdateInfoInPanel()
    {
        List<string> elementProperties = new List<string>()
        {
            $"ID: {Id}",
            $"Destination house: {DestinationHouse.HouseAddress}"
        };
        ElementDetailsPanel.Instance.ShowPanel(Icon, elementProperties);
    }
}
