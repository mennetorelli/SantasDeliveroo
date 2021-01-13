using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "ScriptableObjects/ScriptableObjectScriptableObject", order = 1)]
public class LevelConfiguration : ScriptableObject
{
    [Header("Level")]
    [Tooltip("The level number.")]
    public int Id;

    [Header("General settings")]
    [Tooltip("The number of houses that are initialized.")]
    public int NumberOfHouses;
    [Tooltip("The number of gifts that are generated.")]
    public int NumberOfGifts;
    [Tooltip("How many gifts needs to be delivered.")]
    public int GiftsToDeliver;
    [Tooltip("The maximum time (in seconds) for delivering the gifts.")]
    public int Time;

    [Header("Santas settings")]
    [Tooltip("The number of Santas to be generated.")]
    public int NumberOfSantas;
    [Tooltip("The minimum speed a Santa can have (the speed of a unit is constant but initialized randomly).")]
    public int SantasMinSpeed;
    [Tooltip("The maximum speed a Santa can have (the speed of a unit is constant but initialized randomly).")]
    public int SantasMaxSpeed;

    [Header("Befanas settings")]
    [Tooltip("The number of Befanas to be generated.")]
    public int NumberOfBefanas;
    [Tooltip("The minimum speed a Befana can have (the speed of a unit is constant but initialized randomly).")]
    public int BefanasMinSpeed;
    [Tooltip("The maximum speed a Befana can have (the speed of a unit is constant but initialized randomly).")]
    public int BefanasMaxSpeed;
    [Tooltip("The minimum duration of a Befana's random action.")]
    public int BefanasMinActionDuration;
    [Tooltip("The maximum duration of a Befana's random action.")]
    public int BefanasMaxActionDuration;
    [Tooltip("The maximum value of the detection range of the befanas.")]
    public int BefanasMinDetectionRange;
    [Tooltip("The maximum value of the detection range of the befanas.")]
    public int BefanasMaxDetectionRange;


    [Header("Prefabs")]
    [Tooltip("Prefab of a Santa.")]
    public GameObject SantaPrefab;
    [Tooltip("Prefab of a Befana.")]
    public GameObject BefanaPrefab;
    [Tooltip("Prefab of a Gift.")]
    public GameObject GiftPrefab;
}
