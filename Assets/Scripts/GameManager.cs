using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Collider SantasSpawnArea;
    public Collider BefanasSpawnArea;
    public Collider PlaygroundArea;

    public GameObject EndGamePopup;

    public Transform HousesParent;

    private int _giftsToDeliverCounter;
    private int _remainingSantasCounter;
    private float _timeLeft;

    public float MinHeight { get; set; }
    public float MaxHeight { get; set; }
    public float MaxRangeXZ { get; set; }

    public List<Santa> AvailableSantas { get; set; }

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
        LevelConfiguration level = LoadSettings.Instance.SelectedLevel;

        // Selection of random houses from the map.
        System.Random rnd = new System.Random();
        List<House> selectedHouses = HousesParent.GetComponentsInChildren<House>()
            .OrderBy(_ => rnd.Next())
            .Take(LoadSettings.Instance.SelectedLevel.NumberOfHouses)
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

        AvailableSantas = new List<Santa>();
        for (int i = 0; i < level.NumberOfSantas; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(santasSpawnAreaMinX, santasSpawnAreaMaxX),
                Random.Range(santasSpawnAreaMinY, santasSpawnAreaMaxY),
                Random.Range(santasSpawnAreaMinZ, santasSpawnAreaMaxZ));

            AvailableSantas.Add(Instantiate(level.SantaPrefab, randomPosition, level.SantaPrefab.transform.rotation).GetComponent<Santa>());
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
        Vector3 giftsSpawnAreaOrigin = PlaygroundArea.bounds.center;
        float giftsSpawnAreaMinX = giftsSpawnAreaOrigin.x - PlaygroundArea.bounds.extents.x;
        float giftsSpawnAreaMaxX = giftsSpawnAreaOrigin.x + PlaygroundArea.bounds.extents.x;
        float giftsSpawnAreaMinY = giftsSpawnAreaOrigin.y - PlaygroundArea.bounds.extents.y;
        float giftsSpawnAreaMaxY = giftsSpawnAreaOrigin.y + PlaygroundArea.bounds.extents.y;
        float giftsSpawnAreaMinZ = giftsSpawnAreaOrigin.z - PlaygroundArea.bounds.extents.z;
        float giftsSpawnAreaMaxZ = giftsSpawnAreaOrigin.z + PlaygroundArea.bounds.extents.z;

        List<Gift> instantiatedGifts = new List<Gift>();
        for (int i = 0; i < level.NumberOfGifts; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(giftsSpawnAreaMinX, giftsSpawnAreaMaxX),
                Random.Range(giftsSpawnAreaMinY, giftsSpawnAreaMaxY),
                Random.Range(giftsSpawnAreaMinZ, giftsSpawnAreaMaxZ));

            Gift gift = Instantiate(level.GiftPrefab, randomPosition, level.GiftPrefab.transform.rotation).GetComponent<Gift>();
            instantiatedGifts.Add(gift);
            gift.Id = $"{ i + 1 }";
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

        // Initialize the other settings of the level.
        _timeLeft = level.Time;
        _giftsToDeliverCounter = level.GiftsToDeliver;
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Oh no! Time's out!",
                secondaryButtonEnabled: true,
                primaryButtonText: "Menu",
                secondaryButtonText: "Try again",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => SceneManager.LoadScene("GameScene")); 
        }
    }

    public void DecreaseGiftsCounter(int deliveredGifts) 
    {
        _giftsToDeliverCounter -= deliveredGifts;
        GameInfoPanel.Instance.UpdateGifts(_giftsToDeliverCounter);
        if (_giftsToDeliverCounter == 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Congratulations! You helped the santas to deliver all the gifts!",
                primaryButtonText: "Menu",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"));
        }
    }

    public void DecreaseSantasCounter()
    {
        _remainingSantasCounter--;
        if (_remainingSantasCounter == 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Oh no! All the Santas have been chased by the Befanas!", 
                secondaryButtonEnabled: true,
                primaryButtonText: "Menu",
                secondaryButtonText: "Try again",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => SceneManager.LoadScene("MenuScene"));
        }
    }

    
}
