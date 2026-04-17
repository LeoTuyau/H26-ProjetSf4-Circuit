using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject pile;
    [SerializeField] GameObject resistance;
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
        GameObject newPile = Instantiate(pile, new Vector3(worldPos.x, worldPos.y , 0),Quaternion.Euler(0,0,0));
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
    public GameObject SpawnFil(GameObject filConnexion1, GameObject filConnexion2)
    {
        Vector3 pos = (filConnexion1.transform.position + filConnexion2.transform.position) / 2;
        GameObject fil1 = Instantiate (fil, pos, Quaternion.LookRotation(Vector3.Cross(filConnexion1.transform.position - filConnexion2.transform.position,new Vector3(0,0,1))));
        fil1.transform.localScale = new Vector3(1, (filConnexion1.transform.position - filConnexion2.transform.position).magnitude, 1);
        return fil1;
    }
    public List<GameObject> spawnAnchors(List<GameObject> piles, List<GameObject> resistances)
    {
        List<GameObject> anchors = new List<GameObject>();
        foreach (GameObject pile in piles)
        {
            AddAnchors(anchors, anchor, pile, new Vector3(0.5f, 0, 0));
        }
        foreach (GameObject resistance in resistances)
        {
            AddAnchors(anchors, anchor, resistance, new Vector3(1, 0, 0));
        }
        return anchors;
    }
    private List<GameObject> AddAnchors(List<GameObject> anchors, GameObject anchor, GameObject parent, Vector3 offset)
    {
        string tag = parent.tag;

        if(tag == "Pile")
        {
            if (!parent.GetComponent<Pile>().getAttach1())
            {
                GameObject a1 = Instantiate(anchor, parent.transform.position + new Vector3(-offset.x, 0, 0), Quaternion.Euler(0, 0, 0));
                a1.GetComponent<Anchor>().SetAttache(parent);
                anchors.Add(a1);
            }
            if (!parent.GetComponent<Pile>().getAttach2())
            {
                GameObject a2 = Instantiate(anchor, parent.transform.position + new Vector3(offset.x, 0, 0), Quaternion.Euler(0, 0, 0));
                a2.GetComponent<Anchor>().SetAttache(parent);
                anchors.Add(a2);
            }

        }
        if (tag == "Resistance")
        {
            if (!parent.GetComponent<Resistance>().getAttach1())
            {
                GameObject a1 = Instantiate(anchor, parent.transform.position + new Vector3(-offset.x, 0, 0), Quaternion.Euler(0, 0, 0));
                a1.GetComponent<Anchor>().SetAttache(parent);
                anchors.Add(a1);
            }
            if (!parent.GetComponent<Resistance>().getAttach2())
            {
                GameObject a2 = Instantiate(anchor, parent.transform.position + new Vector3(offset.x, 0, 0), Quaternion.Euler(0, 0, 0));
                a2.GetComponent<Anchor>().SetAttache(parent);
                anchors.Add(a2);
            }
        }
        return anchors;
    }
    public void removeAnchors(List<GameObject> anchors)
    {
        foreach (GameObject anchor in anchors)
        {
                Destroy(anchor);
        }
    }
}
