using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stores the selected level among scenes.
/// </summary>
public class LoadSettings : MonoBehaviour
{
    public LevelConfiguration SelectedLevel { get; set; }

    public static LoadSettings Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Singleton implementation.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            // Persistency among scenes.
            DontDestroyOnLoad(this);
        }
    }

    /// <summary>
    /// Saves the selected level and loads the game scene.
    /// </summary>
    /// <param name="level">The selected level.</param>
    public void LoadScene(LevelConfiguration level)
    {
        SelectedLevel = level;
        SceneManager.LoadScene("GameScene");
    }
}
