using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "ScriptableObjects/ScriptableObjectScriptableObject", order = 1)]
public class LevelConfiguration : ScriptableObject
{
    public int Id;
    public int NumberOfHouses;
    public int NumberOfGifts;
    public int NumberOfSantas;
    public int SantasMinSpeed;
    public int SantasMaxSpeed;
    public int NumberOfBefanas;
    public int BefanasMinSpeed;
    public int BefanasMaxSpeed;
    public int BefanasMinActionDuration;
    public int BefanasMaxActionDuration;
    public int BefanasMinFOVRadius;
    public int BefanasMaxFOVRadius;
    public int GiftsToDeliver;
    public int Time;

    public GameObject SantaPrefab;
    public GameObject BefanaPrefab;
    public GameObject GiftPrefab;
}
