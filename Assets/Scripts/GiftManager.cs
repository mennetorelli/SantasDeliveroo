using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiftManager : MonoBehaviour, ITarget, ISelectable
{
    public string Id;
    public Color Color;
    public GameObject DestinationHouse;

    void Awake()
    {
        System.Random rnd = new System.Random();

        // Assignment of random gift id
        string id = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());

        // Assignment of random color
        Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        GetComponent<Renderer>().material.color = Color;
    }

    void OnTriggerEnter(Collider other)
    {
        SantaBehaviour currentSanta = other.GetComponent<SantaBehaviour>();
        if (currentSanta != null && currentSanta.NextTarget == gameObject)
        {
            currentSanta.PickedUpGifts.Add(gameObject);
            gameObject.SetActive(false);
        }
    }


    public void Selected()
    {

    }
}
