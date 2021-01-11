using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelConfiguration SelectedLevel;

    public Collider SantasSpawnArea;
    public Collider BefanasSpawnArea;
    public Collider GiftsSpawnArea;
    public Collider PlaygroundArea;

    public Transform HousesParent;

    private List<Transform> _selectedHouses;

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
        // Selection of random houses from the map.
        System.Random rnd = new System.Random();
        _selectedHouses = HousesParent.GetComponentsInChildren<Transform>()
            .OrderBy(_ => rnd.Next())
            .Take(SelectedLevel.NumberOfHouses)
            .ToList();

        foreach (Transform house in _selectedHouses)
        {
            house.gameObject.AddComponent<HouseManager>();
        }

        // Spawn the Santas inside their spawn area.
        Vector3 santasSpawnAreaOrigin = SantasSpawnArea.bounds.center;
        float santasSpawnAreaMinX = santasSpawnAreaOrigin.x - SantasSpawnArea.bounds.extents.x;
        float santasSpawnAreaMaxX = santasSpawnAreaOrigin.x + SantasSpawnArea.bounds.extents.x;
        float santasSpawnAreaMinY = santasSpawnAreaOrigin.y - SantasSpawnArea.bounds.extents.y;
        float santasSpawnAreaMaxY = santasSpawnAreaOrigin.y + SantasSpawnArea.bounds.extents.y;
        float santasSpawnAreaMinZ = santasSpawnAreaOrigin.z - SantasSpawnArea.bounds.extents.z;
        float santasSpawnAreaMaxZ = santasSpawnAreaOrigin.z + SantasSpawnArea.bounds.extents.z;

        for (int i = 0; i < SelectedLevel.NumberOfSantas; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(santasSpawnAreaMinX, santasSpawnAreaMaxX),
                Random.Range(santasSpawnAreaMinY, santasSpawnAreaMaxY),
                Random.Range(santasSpawnAreaMinZ, santasSpawnAreaMaxZ));

            Instantiate(SelectedLevel.SantaPrefab, randomPosition, SelectedLevel.SantaPrefab.transform.rotation);
        }

        // Spawn the Befanas inside their spawn area.
        Vector3 befanasSpawnAreaOrigin = BefanasSpawnArea.bounds.center;
        float befanasSpawnAreaMinX = befanasSpawnAreaOrigin.x - BefanasSpawnArea.bounds.extents.x;
        float befanasSpawnAreaMaxX = befanasSpawnAreaOrigin.x + BefanasSpawnArea.bounds.extents.x;
        float befanasSpawnAreaMinY = befanasSpawnAreaOrigin.y - BefanasSpawnArea.bounds.extents.y;
        float befanasSpawnAreaMaxY = befanasSpawnAreaOrigin.y + BefanasSpawnArea.bounds.extents.y;
        float befanasSpawnAreaMinZ = befanasSpawnAreaOrigin.z - BefanasSpawnArea.bounds.extents.z;
        float befanasSpawnAreaMaxZ = befanasSpawnAreaOrigin.z + BefanasSpawnArea.bounds.extents.z;

        for (int i = 0; i < SelectedLevel.NumberOfBefanas; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(befanasSpawnAreaMinX, befanasSpawnAreaMaxX),
                Random.Range(befanasSpawnAreaMinY, befanasSpawnAreaMaxY),
                Random.Range(befanasSpawnAreaMinZ, befanasSpawnAreaMaxZ));

            Instantiate(SelectedLevel.BefanaPrefab, randomPosition, SelectedLevel.BefanaPrefab.transform.rotation);
        }

        // Spawn the gifts inside their spawn area.
        Vector3 giftsSpawnAreaOrigin = GiftsSpawnArea.bounds.center;
        float giftsSpawnAreaMinX = giftsSpawnAreaOrigin.x - GiftsSpawnArea.bounds.extents.x;
        float giftsSpawnAreaMaxX = giftsSpawnAreaOrigin.x + GiftsSpawnArea.bounds.extents.x;
        float giftsSpawnAreaMinY = giftsSpawnAreaOrigin.y - GiftsSpawnArea.bounds.extents.y;
        float giftsSpawnAreaMaxY = giftsSpawnAreaOrigin.y + GiftsSpawnArea.bounds.extents.y;
        float giftsSpawnAreaMinZ = giftsSpawnAreaOrigin.z - GiftsSpawnArea.bounds.extents.z;
        float giftsSpawnAreaMaxZ = giftsSpawnAreaOrigin.z + GiftsSpawnArea.bounds.extents.z;

        for (int i = 0; i < SelectedLevel.NumberOfGifts; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(giftsSpawnAreaMinX, giftsSpawnAreaMaxX),
                Random.Range(giftsSpawnAreaMinY, giftsSpawnAreaMaxY),
                Random.Range(giftsSpawnAreaMinZ, giftsSpawnAreaMaxZ));

            Instantiate(SelectedLevel.GiftPrefab, randomPosition, SelectedLevel.GiftPrefab.transform.rotation);
        }

        // Calculate the play area boundaries.
        Vector3 PlayGroundAreaOrigin = PlaygroundArea.bounds.center;
        MaxRangeXZ = PlayGroundAreaOrigin.x + PlaygroundArea.bounds.extents.x;
        MinHeight = PlayGroundAreaOrigin.y - PlaygroundArea.bounds.extents.y;
        MaxHeight = PlayGroundAreaOrigin.y + PlaygroundArea.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
