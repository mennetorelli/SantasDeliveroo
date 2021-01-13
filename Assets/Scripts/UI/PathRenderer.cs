using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    [Header("Parameters")]
    public float StartWidth = 0.1f;
    public float EndWidth = 0.1f;
    public int VertexNumber = 100;
    public float Radius = 0.5f;

    [Header("Line renderers")]
    [Tooltip("Reference to the line renderer which is in charge of drawing lines.")]
    public LineRenderer LineRenderer;
    [Tooltip("Reference to the line renderer which is in charge of drawing circles.")]
    public LineRenderer CircleRenderer;

    /// <summary>
    /// Draws a line given two points.
    /// </summary>
    /// <param name="origin">The first point.</param>
    /// <param name="destination">The second point.</param>
    public void DrawPath(Vector3 origin, Vector3 destination)
    {
        LineRenderer.startWidth = StartWidth;
        LineRenderer.endWidth = EndWidth;
        LineRenderer.positionCount = 2;

        // Starting point of the line.
        LineRenderer.SetPosition(0, origin);
        // Ending point of the line.
        LineRenderer.SetPosition(1, destination);
    }

    /// <summary>
    /// Draws a circle on a specified point.
    /// </summary>
    /// <param name="destination">The destination point where to draw the circle.</param>
    public void DrawDestination(Vector3 destination)
    {
        CircleRenderer.startWidth = StartWidth;
        CircleRenderer.endWidth = EndWidth;
        CircleRenderer.loop = true;
        float angle = 2 * Mathf.PI / VertexNumber;
        CircleRenderer.positionCount = VertexNumber;

        for (int i = 0; i < VertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(
                new Vector4(Mathf.Cos(angle * i), 0, Mathf.Sin(angle * i), 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(-Mathf.Sin(angle * i), 0, Mathf.Cos(angle * i), 0),
                new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(Radius, 0, 0);
            CircleRenderer.SetPosition(i, destination + rotationMatrix.MultiplyPoint(initialRelativePosition));
        }
    }
}
