using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualFeedbackDrawer : MonoBehaviour
{
    public float StartWidth = 0.1f;
    public float EndWidth = 0.1f;
    public int VertexNumber = 100;
    public float Radius = 0.4f;

    public void DrawPath(Vector3 origin, Vector3 destination)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = StartWidth;
        lineRenderer.endWidth = EndWidth;
        lineRenderer.positionCount = 2;

        // Starting point of the line.
        lineRenderer.SetPosition(0, origin);
        // Ending point of the line.
        lineRenderer.SetPosition(1, destination);
    }

    public void DrawDestination(Vector3 destination)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = StartWidth;
        lineRenderer.endWidth = EndWidth;
        lineRenderer.loop = true;
        float angle = 2 * Mathf.PI / VertexNumber;
        lineRenderer.positionCount = VertexNumber;

        for (int i = 0; i < VertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(
                new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));
            //Matrix4x4 rotationMatrix = new Matrix4x4(
            //    new Vector4(Mathf.Cos(angle * i), 0, Mathf.Sin(angle * i), 0),
            //    new Vector4(0, 1, 0, 0),
            //    new Vector4(-1 * Mathf.Sin(angle * i), 0, Mathf.Cos(angle * i), 0),
            //    new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, Radius, 0);
            lineRenderer.SetPosition(i, destination + rotationMatrix.MultiplyPoint(initialRelativePosition));
        }
    }
}
