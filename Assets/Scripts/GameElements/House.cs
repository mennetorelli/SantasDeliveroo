using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class House : SelectableElementBase
{
    [Tooltip("The possible colors of this house.")]
    public List<Color> Colors;
    [Tooltip("Reference to the renderer where the colored material is applied.")]
    public Renderer ColoredRenderer;
    [Tooltip("The material of an inactive house.")]
    public Material DisabledMaterial;

    // When a house is inactive, all the renderers of the object have the DisabledMaterial.
    private List<Renderer> _renderers;

    public List<Gift> RequestedGifts { get; set; }
    public string HouseAddress { get; set; }

    void Awake()
    {
        RequestedGifts = new List<Gift>();
        _renderers = GetComponentsInChildren<Renderer>().ToList();

        // Assignment of random house addresss.
        string randomStr = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 6)
            .Select(s => s[Random.Range(0, s.Length)]).ToArray());
        HouseAddress = $"No. {Random.Range(0, 100)} {randomStr} Street";

        // Assignment of random color to the material.
        ColoredRenderer.material.color = Colors[Random.Range(0, Colors.Count - 1)];
    }

    void OnTriggerEnter(Collider other)
    {
        Santa currentSanta = other.GetComponent<Santa>();
        // If the colliding Santa is aiming to deliver at this house, i.e. it is not an accidental collision.
        if (currentSanta != null && currentSanta.NextTarget == gameObject)
        {
            List<Gift> deliverableGifts = currentSanta.CollectedGifts.Intersect(RequestedGifts).ToList();
            if (deliverableGifts.Count > 0)
            {
                currentSanta.CollectedGifts.RemoveAll(x => deliverableGifts.Contains(x));
                RequestedGifts.RemoveAll(x => deliverableGifts.Contains(x));

                currentSanta.UpdateSpeed();
                Highlight.SetActive(false);
                ElementDetailsPanel.Instance.UpdatePanel();
                GameManager.Instance.DecreaseGiftsCounter(deliverableGifts.Count);
                MessagePanel.Instance.ShowMessage($"Ho ho ho! {string.Join(", ", deliverableGifts.Select(el => el.Id))} delivered to { HouseAddress }", Color.black);

                // Each delivered gift is no more collected by a Santa.
                foreach (Gift gift in deliverableGifts)
                {
                    gift.CollectedBySanta = null;
                }

                // If no more gifts are requested, disable this house.
                if (RequestedGifts.Count == 0)
                {
                    Destroy(this);
                }
            }
            else
            {
                MessagePanel.Instance.ShowMessage($"No gift in the sleigh matches the gifts requested by this house", Color.red);
            }
        }
    }

    void OnDestroy()
    {
        // The house is no more selectable.
        Destroy(GetComponent<Collider>());
        gameObject.layer = LayerMask.GetMask("Default");


        // Setting the house material to a grayish material.
        foreach (Renderer renderer in _renderers)
        {
            renderer.material = DisabledMaterial;
        }
    }

    public override List<string> FormatProperties()
    {
        return new List<string>()
        {
             $"Address: { HouseAddress }",
             $"Requested gifts: {string.Join(", ", RequestedGifts.Select(el => el.Id))}"
        };
    }

    public override void OnSelect()
    {
        base.OnSelect();
        foreach (Gift gift in RequestedGifts)
        {
            if (gift.CollectedBySanta != null)
            {
                gift.CollectedBySanta.Highlight.SetActive(true);
            }
            else
            {
                gift.Highlight.SetActive(true);
            }
        }
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        foreach (Gift gift in RequestedGifts)
        {
            if (gift.CollectedBySanta != null)
            {
                gift.CollectedBySanta.Highlight.SetActive(false);
            }
            else
            {
                gift.Highlight.SetActive(false);
            }
        }
    }
}
