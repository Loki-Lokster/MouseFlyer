using UnityEngine;

public class LineDrawer : MonoBehaviour

// Not working, was trying to draw a line between center of screen and mouse position in Alternative flight mode
{
    private LineRenderer lineRenderer;

    void Start()
    {
        // Create a new GameObject with a LineRenderer
        GameObject lineObject = new GameObject("Line");
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set the number of points to the line
        lineRenderer.positionCount = 2;

        // Set properties
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
    }

    public void SetStartPoint(Vector3 startPoint)
    {
        lineRenderer.SetPosition(0, startPoint);
    }

    public void SetEndPoint(Vector3 endPoint)
    {
        lineRenderer.SetPosition(1, endPoint);
    }
}
