using System.Collections.Generic;
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
    public void SpawnFil(GameObject filConnexion1, GameObject filConnexion2)
    {
        Vector3 pos = (filConnexion1.transform.position + filConnexion2.transform.position) / 2;
        Instantiate (fil, pos, Quaternion.Euler(Mathf.Asin(Vector3.Cross(filConnexion1.transform.position-filConnexion2.transform.position,new Vector3(1,0,0)).magnitude), 0,0));
    }
    public List<GameObject> spawnAnchors(List<GameObject> piles, List<GameObject> resistances)
    {
        List<GameObject> anchors = new List<GameObject>();
        foreach (GameObject pile in piles)
        {
            GameObject a1 = Instantiate(anchor, pile.transform.position + new Vector3(0.5f, 0, 0), Quaternion.Euler(0, 0, 0));
            a1.GetComponent<Anchor>().SetAttache(pile);
            anchors.Add(a1);
            GameObject a2 = Instantiate(anchor, pile.transform.position + new Vector3 (-0.5f, 0, 0), Quaternion.Euler(0, 0, 0));
            a2.GetComponent<Anchor>().SetAttache(pile);
            anchors.Add(a2);
        }
        foreach (GameObject resistance in resistances)
        {
            GameObject a1 = Instantiate(anchor, resistance.transform.position + new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0));
            a1.GetComponent<Anchor>().SetAttache(resistance);
            anchors.Add(a1);
            GameObject a2 = Instantiate(anchor, resistance.transform.position + new Vector3(-1, 0, 0), Quaternion.Euler(0, 0, 0));
            a2.GetComponent<Anchor>().SetAttache(resistance);
            anchors.Add(a2);
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
