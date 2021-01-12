using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class House : SelectableElementBase
{
    public List<Gift> RequestedGifts { get; set; }
    public string HouseAddress { get; set; }

    void Awake()
    {
        RequestedGifts = new List<Gift>();

        // Assignment of random gift id
        string randomStr = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 6)
            .Select(s => s[Random.Range(0, s.Length)]).ToArray());
        HouseAddress = $"No. {Random.Range(0, 100)} {randomStr} Street";
    }

    void Start()
    {
        gameObject.layer = 10;
    }

    void OnTriggerEnter(Collider other)
    {
        Santa currentSanta = other.GetComponent<Santa>();
        if (currentSanta != null && currentSanta.NextTarget == gameObject)
        {
            List<Gift> deliverableGifts = currentSanta.CollectedGifts.Intersect(RequestedGifts).ToList();
            if (deliverableGifts.Count > 0)
            {
                currentSanta.CollectedGifts.RemoveAll(x => deliverableGifts.Contains(x));
                currentSanta.UpdateSpeed();
                currentSanta.UpdateInfoInPanel();
                RequestedGifts.RemoveAll(x => deliverableGifts.Contains(x));
                UpdateInfoInPanel();
            }
        }
    }

    public override void UpdateInfoInPanel()
    {
        List<string> elementProperties = new List<string>()
        {
             $"House owner: { HouseAddress }"
        };
        foreach (Gift gift in RequestedGifts)
        {
            elementProperties.Add($"Requested gift: { gift.Id }");
        }
        ElementDetailsPanel.Instance.ShowPanel(Icon, elementProperties);
    }
}
