using System.Collections.Generic;
using UnityEngine;

public class Node : Composante
{

    public override float Tension => 0f;
    public override float ValeurOhms => 0f;

    [SerializeField] private List<GameObject> anchors = new List<GameObject>();

    [SerializeField] private List<GameObject> Voisins = new List<GameObject>();

    [SerializeField] GameObject nodePrefab;


    public void AddAnchor(GameObject anchor)
    {
        anchors.Add(anchor);
        Voisins.Add(anchor.GetComponent<Anchor>().GetAttache());
    }
    public new void RemoveAnchor(GameObject anchor)
    {
        if (anchors.Contains(anchor))
        {
            Voisins.Remove(anchor.GetComponent<Anchor>().GetAttache());
            anchors.Remove(anchor);
        }
    }
    public new int getAnchor(GameObject anchor)
    {
        int index = anchors.IndexOf(anchor);
        return index >= 0 ? index + 1 : 0;
    }
    public List<GameObject> GetAnchors() => anchors;

    public int AnchorCount => anchors.Count;
}