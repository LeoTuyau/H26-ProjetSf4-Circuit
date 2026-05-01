using System.Collections.Generic;
using UnityEngine;

public class GraphScript : MonoBehaviour
{
    public RectTransform graphContainer;
    public GameObject pointPrefab;

    private List<float> values = new List<float>();
    private int maxPoints = 100;

    public float maxAmplitude = 1f;

    public void AddValue(float value)
    {
        values.Add(value);

        if (values.Count > maxPoints)
            values.RemoveAt(0);

        DrawGraph();
    }

    void DrawGraph()
    {
        foreach (Transform child in graphContainer)
{
    if (child.CompareTag("GraphStatic"))
        continue;

    Destroy(child.gameObject);
}
        float width = graphContainer.sizeDelta.x;
        float height = graphContainer.sizeDelta.y;

        for (int i = 0; i < values.Count; i++)
        {
            float x = i * (width / maxPoints) - width / 2;
            float y = (values[i] / maxAmplitude) * (height / 2);

            GameObject point = Instantiate(pointPrefab, graphContainer);
            point.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }
}