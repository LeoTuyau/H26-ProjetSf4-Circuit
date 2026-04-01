using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{

    List<Composante> resistances;
    List<Composante> piles;
    List<Composante> fils;
    ItemSpawner itemSpawner;
    [SerializeField] BouttonFil BtnFil;
    [SerializeField] MouseManager mouseManager;
    bool modeFil = false;

    void Start()
    {
        itemSpawner = GetComponent<ItemSpawner>();

        resistances = new List<Composante>();
        piles = new List<Composante>();
        fils = new List<Composante>();

        foreach (GameObject go in itemSpawner.getResistances())
        {
            resistances.Add(go.GetComponent<Composante>());
        }
        foreach (GameObject go in itemSpawner.getPiles())
        {
            piles.Add(go.GetComponent<Composante>());
        }
        foreach (GameObject go in itemSpawner.getFils())
        {
            fils.Add(go.GetComponent<Composante>());
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


    private void DFSChemins(Composante current, Composante target, HashSet<Composante> visited,
                         List<Resistance> cheminActuel, List<List<Resistance>> chemins)
    {
        visited.Add(current);

        if (current is Resistance r)
            cheminActuel.Add(r);

        foreach (var voisin in current.GetConnexions())
        {
            if (!visited.Contains(voisin))
            {
                if (voisin == target)
                {
                    chemins.Add(new List<Resistance>(cheminActuel)); // sauvegarde du chemin
                }
                else
                {
                    DFSChemins(voisin, target, visited, cheminActuel, chemins);
                }
            }
        }

        // Backtracking
        if (current is Resistance)
            cheminActuel.RemoveAt(cheminActuel.Count - 1);

        visited.Remove(current);
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
    public void AddFil(GameObject fil)
    {
        
    }
}


