using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject pile;
    [SerializeField] GameObject resistance;
    [SerializeField] MouseManager mm;
    private List<GameObject> piles = new List<GameObject>();
    private List<GameObject> resistances = new List<GameObject>();
    [SerializeField] GameObject anchor;
    List<GameObject> anchors = new List<GameObject>();

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
        piles.Add(newPile);
        mm.DragButtonStart(newPile);
    }
    public void spawnResistance()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        GameObject newResistance = Instantiate(resistance, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.Euler(0, 0, 0));
        resistances.Add(newResistance);
        mm.DragButtonStart(newResistance);
    }
    public void spawnAnchors()
    {
        foreach (GameObject pile in piles)
        {
            GameObject a1 = Instantiate(anchor, new Vector3(pile.transform.position.x + 0.5f, pile.transform.position.y, pile.transform.position.z), Quaternion.Euler(0, 0, 0));
            anchors.Add(a1);
            GameObject a2 = Instantiate(anchor, new Vector3(pile.transform.position.x - 0.5f, pile.transform.position.y, pile.transform.position.z), Quaternion.Euler(0, 0, 0));
            anchors.Add(a2);
        }
        foreach (GameObject resistance in resistances)
        {
            GameObject a1 = Instantiate(anchor, new Vector3(resistance.transform.position.x + 1, resistance.transform.position.y, resistance.transform.position.z), Quaternion.Euler(0, 0, 0));
            anchors.Add(a1);
            GameObject a2 = Instantiate(anchor, new Vector3(resistance.transform.position.x - 1, resistance.transform.position.y, resistance.transform.position.z), Quaternion.Euler(0, 0, 0));
            anchors.Add(a2);
        }
    }
    public void removeAnchors()
    {
        foreach (GameObject anchor in anchors)
        {
            Destroy(anchor);
        }
    }
    public List<GameObject> getPiles()
    {
        return piles;
    }
    public List<GameObject> getResistances()
    {
        return resistances;
    }
    public void SpawnFil(filConnexion1, currentObject)
    {
        //A faire
    }
}
