using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class House : SelectableElementBase
{
    public List<Color> Colors;
    public Material DisabledMaterial;
    public Renderer ColoredRenderer;
    
    private List<Renderer> _renderers;

    public List<Gift> RequestedGifts { get; set; }
    public string HouseAddress { get; set; }
    public bool Highlight { get; set; }

    private List<Renderer> _houseRenderers;

    void Awake()
    {
        RequestedGifts = new List<Gift>();
        _renderers = GetComponentsInChildren<Renderer>().ToList();

        // Assignment of random house addresss.
        string randomStr = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 6)
            .Select(s => s[Random.Range(0, s.Length)]).ToArray());
        HouseAddress = $"No. {Random.Range(0, 100)} {randomStr} Street";

        _houseRenderers = GetComponentsInChildren<Renderer>().ToList();
    }

    void Start()
    {
        gameObject.layer = 10;
    }

    void Update()
    {
        if (Highlight)
        {
            float lerp = Mathf.PingPong(Time.time, 1.5f) / 1.5f;
            foreach (Renderer renderer in _houseRenderers)
            {
                renderer.material.Lerp(renderer.material, renderer.material, lerp);
            }
        }
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
                RequestedGifts.RemoveAll(x => deliverableGifts.Contains(x));
                ElementDetailsPanel.Instance.UpdatePanel();
                GameManager.Instance.DecreaseGiftsCounter(deliverableGifts.Count);
                MessagePanel.Instance.ShowMessage($"Ho ho ho! {deliverableGifts.Count} delivered to { HouseAddress }", Color.black);
            }
            else
            {
                MessagePanel.Instance.ShowMessage($"Nothing in the sleigh matches the requested gifts", Color.red);
            }
        }
    }

    public override List<string> FormatDetails()
    {
        List<string> elementProperties = new List<string>()
        {
             $"House owner: { HouseAddress }"
        };
        foreach (Gift gift in RequestedGifts)
        {
            elementProperties.Add($"Requested gift: { gift.Id }");
        }
        return elementProperties;
    }

    void OnEnable()
    {
        // Assignment of random color to the material.
        ColoredRenderer.material.color = Colors[Random.Range(0, Colors.Count - 1)];
    }

    void OnDisable()
    {
        // Setting the house material to a grayish material.
        foreach (Renderer renderer in _renderers)
        {
            renderer.material = DisabledMaterial;
        }
    }

    public override void Selected()
    {
        Debug.Log("Nothing to override");
    }

    public override void Deselected()
    {
        Debug.Log("Nothing to override");
    }
}
