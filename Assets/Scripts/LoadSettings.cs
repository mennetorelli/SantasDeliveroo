using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void LoadScene(LevelConfiguration level)
    {
        SelectedLevel = level;
        SceneManager.LoadScene("GameScene");
    }
}
