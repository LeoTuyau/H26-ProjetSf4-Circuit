using System.Collections.Generic;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{

    List<GameObject> resistances;
    List<GameObject> piles;
    bool circuitFerme = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ItemSpawner itemSpawner = GetComponent<ItemSpawner>();
        List<GameObject> resistances = itemSpawner.getResistances();
        List<GameObject> piles = itemSpawner.getPiles();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool VerifierCircuitFerme()
    {
        for (int i = 0; i < piles.Count; i++)
        {

        }


        return false;
    }
}
