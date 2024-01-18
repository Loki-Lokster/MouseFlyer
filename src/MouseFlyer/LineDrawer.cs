using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 startPoint;
    private Vector3 endPoint;

    public LineDrawer(Vector3 start, Vector3 end)
    {
        this.startPoint = start;
        this.endPoint = end;
    }

    void Start()
    {
        // Create a new GameObject with a LineRenderer
        GameObject lineObject = new GameObject("Line");
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set the number of points to the line
        lineRenderer.positionCount = 2;

        // Set the start and end points
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        // Customize your line appearance
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
    }

    void Update()
    {
        // Update the line's points if they are dynamic
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
