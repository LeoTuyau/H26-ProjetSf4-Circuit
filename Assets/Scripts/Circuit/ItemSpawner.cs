using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject pile;
    [SerializeField] GameObject resistance;
    [SerializeField] GameObject node;
    [SerializeField] MouseManager mm;
    [SerializeField] CircuitManager cm;
    [SerializeField] GameObject anchor;
    [SerializeField] GameObject fil;

    void Start()
    {

    }

    void Update()
    {

    }
    public void spawnPile()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        GameObject newPile = Instantiate(pile, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.Euler(0, 0, 0));
        cm.AddPile(newPile);
        mm.DragButtonStart(newPile);
    }
    public void spawnResistance()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        GameObject newResistance = Instantiate(resistance, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.Euler(0, 0, 0));
        cm.AddResistance(newResistance);
        mm.DragButtonStart(newResistance);
    }
    public void spawnNode()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        GameObject newNode = Instantiate(node, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.Euler(0, 0, 0));
        cm.AddNode(newNode);
        mm.DragButtonStart(newNode);
    }
    public GameObject SpawnFil(GameObject filConnexion1, GameObject filConnexion2)
    {
        Vector3 pos = (filConnexion1.transform.position + filConnexion2.transform.position) / 2;
        GameObject fil1 = Instantiate(fil, pos, Quaternion.LookRotation(Vector3.Cross(filConnexion1.transform.position - filConnexion2.transform.position, new Vector3(0, 0, 1))));
        fil1.transform.localScale = new Vector3(1, (filConnexion1.transform.position - filConnexion2.transform.position).magnitude, 1);

        fil1.GetComponent<Fil>().SetAttaches(filConnexion1.GetComponent<Anchor>().GetAttache(),filConnexion2.GetComponent<Anchor>().GetAttache());
        fil1.GetComponent<Fil>().SetAnchorOffsets(filConnexion1, filConnexion2);
        
        return fil1;
    }
    public List<GameObject> spawnAnchors(List<GameObject> piles, List<GameObject> resistances, List<GameObject> nodes)
    {
        List<GameObject> anchors = new List<GameObject>();
        foreach (GameObject pile in piles)
        {
            anchors = AddAnchors(anchors, anchor, pile, 0.5f);
        }
        foreach (GameObject resistance in resistances)
        {
            anchors = AddAnchors(anchors, anchor, resistance, 1);
        }
        foreach (GameObject node in nodes)
        {
            anchors = AddAnchors(anchors, anchor, node, 0);
        }
        return anchors;
    }
    private List<GameObject> AddAnchors(List<GameObject> anchors, GameObject anchor, GameObject parent, float offset)
    {
        Vector3 rotation = parent.transform.eulerAngles;
        if (!parent.CompareTag("Node"))
        {
            if (!parent.GetComponent<Composante>().getAttach1())
            {
                GameObject a1 = Instantiate(anchor, parent.transform.position + parent.transform.right * -offset, Quaternion.Euler(0, 0, 0));
                a1.GetComponent<Anchor>().SetAttache(parent);
                a1.GetComponent<Anchor>().SetOffset(-offset);
                parent.GetComponent<Composante>().setAnchor1(a1);
                anchors.Add(a1);
            }
            if (!parent.GetComponent<Composante>().getAttach2())
            {
                GameObject a2 = Instantiate(anchor, parent.transform.position + parent.transform.right * offset, Quaternion.Euler(0, 0, 0));
                a2.GetComponent<Anchor>().SetAttache(parent);
                a2.GetComponent<Anchor>().SetOffset(offset);
                parent.GetComponent<Composante>().setAnchor2(a2);
                anchors.Add(a2);
            }
        }
        else
        {
            GameObject a1 = Instantiate(anchor,parent.transform.position + Vector3.back, Quaternion.Euler(0, 0, 0));
            a1.GetComponent <Anchor>().SetAttache(parent);
            a1.GetComponent <Anchor>().SetOffset(0f);
            parent.GetComponent<Node>().AddAnchor(a1);
            anchors.Add(a1);
        }
            return anchors;
    }
    public void removeAnchors(List<GameObject> anchors)
    {
        foreach (GameObject anchor in anchors)
        {
            if (anchor.GetComponent<Anchor>().GetAttache().GetComponent<Pile>() != null) //Si pile
            {
                anchor.GetComponent<Anchor>().GetAttache().GetComponent<Pile>().RemoveAnchor(anchor); //Enlever le Anchor de l'objet
            }
            else if (anchor.GetComponent<Anchor>().GetAttache().GetComponent<Resistance>() != null) //Si resistance
            {
                anchor.GetComponent<Anchor>().GetAttache().GetComponent<Resistance>().RemoveAnchor(anchor); //Enlever le Anchor de l'objet
            }
            Destroy(anchor); // Detruire le Anchor

        }
    }
}
