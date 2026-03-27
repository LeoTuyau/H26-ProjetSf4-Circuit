using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{

    List<Composante> resistances;
    List<Composante> piles;
    ItemSpawner itemSpawner;
    [SerializeField] BouttonFil BtnFil;
    [SerializeField] MouseManager mouseManager;
    bool modeFil = false;

    void Start()
    {
        itemSpawner = GetComponent<ItemSpawner>();
        foreach (GameObject go in itemSpawner.getResistances())
        {
            resistances.Add(new Composante());
        }
        foreach (GameObject go in itemSpawner.getPiles())
        {
            resistances.Add(new Composante());
        }


    }
    void Update()
    {
        
    }
    public bool VerifierCircuitFerme()
    {
        HashSet<Composante> visited = new HashSet<Composante>();

        foreach (var start in piles)
        {
            if (DFS(start, start, visited, null))
                return true;
        }

        return false;
    }

    private bool DFS(Composante current, Composante target, HashSet<Composante> visited, Composante parent)
    {
        visited.Add(current);

        foreach (var voisin in current.getConnexions())
        {
            if (voisin == parent)
                continue;

            if (voisin == target)
                return true;

            if (!visited.Contains(voisin))
            {
                if (DFS(voisin, target, visited, current))
                    return true;
            }
        }

        return false;
    }

    public void ToggleFil()
    {
        if (!modeFil)
        {
            itemSpawner.spawnAnchors();
            BtnFil.SetSelected(true);
            modeFil = true;
            mouseManager.SetMode("fil");
        }
        else
        {
            itemSpawner.removeAnchors();
            BtnFil.SetSelected(false);
            modeFil = false;
            mouseManager.SetMode("defaut");
        }
    }
    public void ToggleFil(bool modeFil)
    {
        if (this.modeFil!=modeFil)
        {
            ToggleFil();
        }
    }
}


