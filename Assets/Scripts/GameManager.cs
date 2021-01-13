using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that manages the initialization of the game and game flow.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("The area in which the Santas will be spawned randomly.")]
    public Collider SantasSpawnArea;
    [Tooltip("The area in which the Befanas will be spawned randomly.")]
    public Collider BefanasSpawnArea;
    [Tooltip("The area in which the gifts will be spawned and in which it is possible to move the camera in free mode.")]
    public Collider PlaygroundArea;

    [Tooltip("Reference the Transform containing all the houses in the game. Used for initializing random houses from the map.")]
    public Transform HousesParent;

    [Tooltip("Reference to the popup GameObject.")]
    public GameObject EndGamePopup;

    private int _giftsToDeliverCounter;
    private int _santasCounter;
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
        List<House> allHouses = HousesParent.GetComponentsInChildren<House>().ToList();
        List<House> selectedHouses = allHouses
            .OrderBy(_ => rnd.Next())
            .Take(LoadSettings.Instance.SelectedLevel.NumberOfHouses)
            .ToList();

        foreach (House house in allHouses.Except(selectedHouses))
        {
            Destroy(house);
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

        // Calculate the play area boundaries.
        Vector3 PlayGroundAreaOrigin = PlaygroundArea.bounds.center;
        MaxRangeXZ = PlayGroundAreaOrigin.x + PlaygroundArea.bounds.extents.x;
        MinHeight = PlayGroundAreaOrigin.y - PlaygroundArea.bounds.extents.y;
        MaxHeight = PlayGroundAreaOrigin.y + PlaygroundArea.bounds.extents.y;

        // Spawn the gifts inside the play area area.
        List<Gift> instantiatedGifts = new List<Gift>();
        for (int i = 0; i < level.NumberOfGifts; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-MaxRangeXZ, MaxRangeXZ),
                Random.Range(MinHeight, MaxHeight),
                Random.Range(-MaxRangeXZ, MaxRangeXZ));

            Gift gift = Instantiate(level.GiftPrefab, randomPosition, level.GiftPrefab.transform.rotation).GetComponent<Gift>();
            instantiatedGifts.Add(gift);
            gift.Id = $"Gift { i + 1 }";
        }

        // Assign each gift to one house.
        for (int i = 0; i < instantiatedGifts.Count; i++)
        {
            House currentHouse = selectedHouses[i % selectedHouses.Count];
            instantiatedGifts[i].DestinationHouse = currentHouse;
            currentHouse.RequestedGifts.Add(instantiatedGifts[i]);
        }

        // Initialize the other settings of the level.
        _timeLeft = level.Time;
        _giftsToDeliverCounter = level.GiftsToDeliver;
        _santasCounter = level.NumberOfSantas;
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;
        // Time out.
        if (_timeLeft < 0)
        {
            // Reset the timer, so that we don't enter the if condition multiple times.
            _timeLeft = 0;

            Popup.Instance.ActivatePopup(
                message: "Oh no! Time's out!",
                secondaryButtonEnabled: true,
                primaryButtonText: "Menu",
                secondaryButtonText: "Try again",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => SceneManager.LoadScene("GameScene"));
        }
    }

    /// <summary>
    /// Decrease the number of gifts to deliver when a santa has performed a delivery action.
    /// If there are no more gifts to deliver, terminates the level.
    /// </summary>
    public void DecreaseGiftsCounter(int deliveredGifts)
    {
        _giftsToDeliverCounter -= deliveredGifts;
        GameInfoPanel.Instance.UpdateGifts(_giftsToDeliverCounter);
        if (_giftsToDeliverCounter == 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Congratulations! All the gifts have been delivered!",
                primaryButtonText: "Menu",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"));
        }
    }

    /// <summary>
    /// Decrease by one the number of active Santas when one of them gets chased by a Befana.
    /// If no more active santas exist, terminates the game.
    /// </summary>
    public void DecreaseSantasCounter()
    {
        _santasCounter--;
        GameInfoPanel.Instance.UpdateSantas(_santasCounter);
        if (_santasCounter == 0)
        {
            Popup.Instance.ActivatePopup(
                message: "Oh no! All the Santas have been chased by the Befanas!",
                secondaryButtonEnabled: true,
                primaryButtonText: "Menu",
                secondaryButtonText: "Try again",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => SceneManager.LoadScene("GameScene"));
        }
    }

    /// <summary>
    /// Pauses the game by showing the popup.
    /// </summary>
    /// <param name="context"></param>
    public void OnPauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Popup.Instance.ActivatePopup(
                message: "Game paused",
                secondaryButtonEnabled: true,
                primaryButtonText: "Menu",
                secondaryButtonText: "Continue",
                primaryCallback: () => SceneManager.LoadScene("MenuScene"),
                secondaryCallback: () => Popup.Instance.DeactivatePopup());
        }
    }
}
