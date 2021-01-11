using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Collider SantasSpawnArea;
    public Collider BefanasSpawnArea;
    public Collider GiftsSpawnArea;
    public Collider PlaygroundArea;

    public GameObject EndGamePopup;

    public Transform HousesParent;

    private int _giftsToDeliverCounter;
    private int _remainingSantasCounter;
    private float _timeLeft;

    public float MinHeight { get; set; }
    public float MaxHeight { get; set; }
    public float MaxRangeXZ { get; set; }

    public static GameManager Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        // Singleton implementation.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        LevelConfiguration level = LoadManager.Instance.SelectedLevel;

        // Selection of random houses from the map.
        System.Random rnd = new System.Random();
        List<House> selectedHouses = HousesParent.GetComponentsInChildren<House>()
            .OrderBy(_ => rnd.Next())
            .Take(LoadManager.Instance.SelectedLevel.NumberOfHouses)
            .ToList();

        foreach (House house in selectedHouses)
        {
            house.enabled = true;
        }

        // Spawn the Santas inside their spawn area.
        Vector3 santasSpawnAreaOrigin = SantasSpawnArea.bounds.center;
        float santasSpawnAreaMinX = santasSpawnAreaOrigin.x - SantasSpawnArea.bounds.extents.x;
        float santasSpawnAreaMaxX = santasSpawnAreaOrigin.x + SantasSpawnArea.bounds.extents.x;
        float santasSpawnAreaMinY = santasSpawnAreaOrigin.y - SantasSpawnArea.bounds.extents.y;
        float santasSpawnAreaMaxY = santasSpawnAreaOrigin.y + SantasSpawnArea.bounds.extents.y;
        float santasSpawnAreaMinZ = santasSpawnAreaOrigin.z - SantasSpawnArea.bounds.extents.z;
        float santasSpawnAreaMaxZ = santasSpawnAreaOrigin.z + SantasSpawnArea.bounds.extents.z;

        for (int i = 0; i < level.NumberOfSantas; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(santasSpawnAreaMinX, santasSpawnAreaMaxX),
                Random.Range(santasSpawnAreaMinY, santasSpawnAreaMaxY),
                Random.Range(santasSpawnAreaMinZ, santasSpawnAreaMaxZ));

            Instantiate(level.SantaPrefab, randomPosition, level.SantaPrefab.transform.rotation);
        }

        // Spawn the Befanas inside their spawn area.
        Vector3 befanasSpawnAreaOrigin = BefanasSpawnArea.bounds.center;
        float befanasSpawnAreaMinX = befanasSpawnAreaOrigin.x - BefanasSpawnArea.bounds.extents.x;
        float befanasSpawnAreaMaxX = befanasSpawnAreaOrigin.x + BefanasSpawnArea.bounds.extents.x;
        float befanasSpawnAreaMinY = befanasSpawnAreaOrigin.y - BefanasSpawnArea.bounds.extents.y;
        float befanasSpawnAreaMaxY = befanasSpawnAreaOrigin.y + BefanasSpawnArea.bounds.extents.y;
        float befanasSpawnAreaMinZ = befanasSpawnAreaOrigin.z - BefanasSpawnArea.bounds.extents.z;
        float befanasSpawnAreaMaxZ = befanasSpawnAreaOrigin.z + BefanasSpawnArea.bounds.extents.z;

        for (int i = 0; i < level.NumberOfBefanas; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(befanasSpawnAreaMinX, befanasSpawnAreaMaxX),
                Random.Range(befanasSpawnAreaMinY, befanasSpawnAreaMaxY),
                Random.Range(befanasSpawnAreaMinZ, befanasSpawnAreaMaxZ));

            Instantiate(level.BefanaPrefab, randomPosition, level.BefanaPrefab.transform.rotation);
        }

        // Spawn the gifts inside their spawn area.
        Vector3 giftsSpawnAreaOrigin = GiftsSpawnArea.bounds.center;
        float giftsSpawnAreaMinX = giftsSpawnAreaOrigin.x - GiftsSpawnArea.bounds.extents.x;
        float giftsSpawnAreaMaxX = giftsSpawnAreaOrigin.x + GiftsSpawnArea.bounds.extents.x;
        float giftsSpawnAreaMinY = giftsSpawnAreaOrigin.y - GiftsSpawnArea.bounds.extents.y;
        float giftsSpawnAreaMaxY = giftsSpawnAreaOrigin.y + GiftsSpawnArea.bounds.extents.y;
        float giftsSpawnAreaMinZ = giftsSpawnAreaOrigin.z - GiftsSpawnArea.bounds.extents.z;
        float giftsSpawnAreaMaxZ = giftsSpawnAreaOrigin.z + GiftsSpawnArea.bounds.extents.z;

        List<Gift> instantiatedGifts = new List<Gift>();
        for (int i = 0; i < level.NumberOfGifts; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(giftsSpawnAreaMinX, giftsSpawnAreaMaxX),
                Random.Range(giftsSpawnAreaMinY, giftsSpawnAreaMaxY),
                Random.Range(giftsSpawnAreaMinZ, giftsSpawnAreaMaxZ));

            instantiatedGifts.Add(Instantiate(level.GiftPrefab, randomPosition, level.GiftPrefab.transform.rotation).GetComponent<Gift>());
        }

        // Assign each gift to one house.
        for (int i = 0; i < instantiatedGifts.Count; i++)
        {
            House currentHouse = selectedHouses[i % selectedHouses.Count];
            instantiatedGifts[i].DestinationHouse = currentHouse;
            currentHouse.RequestedGifts.Add(instantiatedGifts[i]);
        }

        // Calculate the play area boundaries.
        Vector3 PlayGroundAreaOrigin = PlaygroundArea.bounds.center;
        MaxRangeXZ = PlayGroundAreaOrigin.x + PlaygroundArea.bounds.extents.x;
        MinHeight = PlayGroundAreaOrigin.y - PlaygroundArea.bounds.extents.y;
        MaxHeight = PlayGroundAreaOrigin.y + PlaygroundArea.bounds.extents.y;

        _timeLeft = level.Time;
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Oh no! Time's out!",
                primaryButtonText: "Menu",
                secondaryButtonText: "Try again",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => SceneManager.LoadScene("MenuScene"));
        }
    }

    public void DecreaseGiftsCounter(int deliveredGifts) 
    {
        _giftsToDeliverCounter -= deliveredGifts;
        if (_giftsToDeliverCounter == 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Congratulations! You helped the santas to deliver all the gifts!",
                primaryButtonText: "Menu",
                secondaryButtonText: "Next level!",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => SceneManager.LoadScene("MenuScene"));
        }
    }

    public void DecreaseSantasCounter()
    {
        _remainingSantasCounter--;
        if (_giftsToDeliverCounter == 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Oh no! All the Santas have been chased by the Befanas!",
                primaryButtonText: "Menu",
                secondaryButtonText: "Try again",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => SceneManager.LoadScene("MenuScene"));
        }
    }

    
}
