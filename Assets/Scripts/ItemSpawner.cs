using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject pile;
    [SerializeField] GameObject resistance;
    private List<GameObject> piles = new List<GameObject>();
    private List<GameObject> resistances = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void spawnPile()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        GameObject newPile = Instantiate(pile, new Vector3(worldPos.x, worldPos.y , 0),Quaternion.Euler(0,0,0));
        piles.Add(newPile);
    }
    public void spawnResistance()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        GameObject newResistance = Instantiate(resistance, new Vector3(worldPos.x-17, worldPos.y-4, -1.28406f), Quaternion.Euler(0, 0, 0));
        resistances.Add(newResistance);
    }
    public List<GameObject> getPiles()
    {
        return piles;
    }
    public List<GameObject> getResistances()
    {
        return resistances;
    }
}
