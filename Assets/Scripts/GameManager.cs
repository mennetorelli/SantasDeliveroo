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
    private List<SelectableElementBase> _remainingSantas;
    
    // Public properties as shorthands of play area boundaries.
    public float MinHeight { get => PlaygroundArea.bounds.center.y - PlaygroundArea.bounds.extents.y; }
    public float MaxHeight { get => PlaygroundArea.bounds.center.y + PlaygroundArea.bounds.extents.y; }
    public float MaxRangeXZ { get => PlaygroundArea.bounds.center.x + PlaygroundArea.bounds.extents.x; }

    public float TimeLeft { get; set; }

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

        // Spawn the Santas inside their spawn area, and reference them.
        // This list is needed in case two Befanas have chased the same Santa.
        _remainingSantas = SpawnObjects(level.SantaPrefab, level.NumberOfSantas, SantasSpawnArea);

        // Spawn the Befanas inside their spawn area.
        SpawnObjects(level.BefanaPrefab, level.NumberOfBefanas, BefanasSpawnArea);

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

        // Spawn the gifts inside the play area area, and reference them.
        List<SelectableElementBase> instantiatedGifts = SpawnObjects(level.GiftPrefab, level.NumberOfGifts, PlaygroundArea);

        // Assign each gift to one house.
        for (int i = 0; i < instantiatedGifts.Count; i++)
        {
            Gift gift = (Gift)instantiatedGifts[i];
            gift.Id = $"Gift { i + 1 }";
            House currentHouse = selectedHouses[i % selectedHouses.Count];
            gift.DestinationHouse = currentHouse;
            currentHouse.RequestedGifts.Add(gift);
        }

        // Initialize the other properties of the level.
        TimeLeft = level.Time;
        _giftsToDeliverCounter = level.GiftsToDeliver;
    }

    /// <summary>
    /// Spawn a category of objects inside an area.
    /// </summary>
    /// <param name="ObjectToSpawn">The prefab of the object to spawn.</param>
    /// <param name="numberOfObjects">The number of objects to spawn.</param>
    /// <param name="area">The area in which to spawn the objects.</param>
    /// <returns>The list of instantiated objects.</returns>
    List<SelectableElementBase> SpawnObjects(GameObject ObjectToSpawn, int numberOfObjects, Collider area)
    {
        List<SelectableElementBase> instantiatedObjects = new List<SelectableElementBase>();
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(area.bounds.center.x - area.bounds.size.x / 2, area.bounds.center.x + area.bounds.size.x / 2),
                Random.Range(area.bounds.center.y - area.bounds.size.y / 2, area.bounds.center.y + area.bounds.size.y / 2),
                Random.Range(area.bounds.center.z - area.bounds.size.z / 2, area.bounds.center.z + area.bounds.size.z / 2));

            instantiatedObjects.Add(Instantiate(ObjectToSpawn, randomPosition, ObjectToSpawn.transform.rotation).GetComponent<SelectableElementBase>());
        }
        return instantiatedObjects;
    }

    void Update()
    {
        TimeLeft -= Time.deltaTime;
        // Time out.
        if (TimeLeft < 0)
        {
            // Reset the timer, so that we don't enter the if condition multiple times.
            TimeLeft = 0;

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
    public void RemoveSanta(Santa santaToRemove)
    {
        _remainingSantas.Remove(santaToRemove);
        GameInfoPanel.Instance.UpdateSantas(_remainingSantas.Count);
        if (_remainingSantas.Count == 0)
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
