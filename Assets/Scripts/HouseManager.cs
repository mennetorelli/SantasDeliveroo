using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HouseManager : MonoBehaviour, ITarget, ISelectable
{
    public string HouseName;
    public List<GameObject> RequestedGifts;

    void Awake()
    {
        gameObject.layer = 10;
    }

    void OnTriggerEnter(Collider other)
    {
        SantaBehaviour currentSanta = other.GetComponent<SantaBehaviour>();
        if (currentSanta != null && currentSanta.NextTarget == gameObject)
        {
            List<GameObject> deliverableGifts = currentSanta.PickedUpGifts.Intersect(RequestedGifts).ToList();
            Debug.Log(deliverableGifts);
            if (deliverableGifts.Count > 0)
            {
                currentSanta.PickedUpGifts.RemoveAll(x => deliverableGifts.Contains(x));
                RequestedGifts.RemoveAll(x => deliverableGifts.Contains(x));
                Debug.Log(RequestedGifts);
            }
        }
    }

    public void Selected()
    {
        
    }
}
