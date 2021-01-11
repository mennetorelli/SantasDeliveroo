using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "ScriptableObjects/ScriptableObjectScriptableObject", order = 1)]
public class LevelConfiguration : ScriptableObject
{
    public int NumberOfHouses;
    public int NumberOfGifts;
    public int NumberOfSantas;
    public float SantasMinSpeed;
    public float SantasMaxSpeed;
    public int NumberOfBefanas;
    public float BefanasMinSpeed;
    public float BefanasMaxSpeed;
    public int GiftsToDeliver;
    public int Time;

    public GameObject SantaPrefab;
    public GameObject BefanaPrefab;
    public GameObject GiftPrefab;
}
