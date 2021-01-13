using UnityEngine;

/// <summary>
/// Blends between two materials.
/// </summary>
public class HighlightElement : MonoBehaviour
{
    public Material material1;
    public Material material2;
    public float duration = 0.5f;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();

        // At start, use the first material.
        _renderer.material = material1;
    }

    void Update()
    {
        // Ping-pong between the materials over the duration.
        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        _renderer.material.Lerp(material1, material2, lerp);
    }
}
