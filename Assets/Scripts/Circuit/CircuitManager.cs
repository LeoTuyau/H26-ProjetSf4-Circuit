using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{

    [SerializeField] List<GameObject> resistances = new List<GameObject>();
    [SerializeField] List<GameObject> piles = new List<GameObject>();
    [SerializeField] List<GameObject> fils = new List<GameObject>();
    [SerializeField] List<GameObject> nodes = new List<GameObject>();
    [SerializeField] List<GameObject> anchors;

    [SerializeField] ItemSpawner itemSpawner;
    [SerializeField] BouttonFil BtnFil;
    [SerializeField] MouseManager mouseManager;

    [SerializeField] TMP_Text tmp;

    bool modeFil = false;

    void Start()
    {
        itemSpawner = GetComponent<ItemSpawner>();
    }
    void Update()
    {
        if (circuitFerme())
        {
            tmp.text = "Circuit fermé";
            SimulerCircuit();
        }
        else
        {
            tmp.text = "Circuit ouvert";
        }
    }

    public void ToggleFil()
    {
        if (!modeFil)
        {
            anchors = itemSpawner.spawnAnchors(piles,resistances,nodes);
            BtnFil.SetSelected(true);
            modeFil = true;
            mouseManager.SetMode("fil");
        }
        else
        {
            itemSpawner.removeAnchors(anchors);
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
    public void AddFil(List<GameObject> anchors)
    {
        GameObject a1 = anchors[0];
        GameObject a2 = anchors[1];

        Composante composante1 = a1.GetComponent<Anchor>().GetAttache().GetComponent<Composante>();
        Composante composante2 = a2.GetComponent<Anchor>().GetAttache().GetComponent<Composante>();

        int borne1 = composante1.getAnchor(a1);
        int borne2 = composante2.getAnchor(a2);

        // Si c1 est un Node, il hérite de la borne de c2 et vice versa
        if (composante1.CompareTag("Node")) borne1 = borne2;
        if (composante2.CompareTag("Node")) borne2 = borne1;

        composante1.Connecter(composante2, borne1, borne2);

        switch (composante1.getAnchor(a1))
        {
            case 1:
                composante1.setAttach1(true);
                break;
            case 2:
                composante1.setAttach2(true);
                break;
        }

        switch (composante2.getAnchor(a2))
        {
            case 1:
                composante2.setAttach1(true);
                break;
            case 2:
                composante2.setAttach2(true);
                break;
        }
        
        fils.Add(itemSpawner.SpawnFil(a1, a2));
    }
    public void AddPile(GameObject pile)
    {
        piles.Add(pile);
    }
    public void AddResistance(GameObject resistance)
    {
        resistances.Add(resistance);
    }
    public void AddNode(GameObject node)
    {
        nodes.Add(node);
    }
    // ─── Circuit fermé (DFS) ──────────────────────────────────────────

    bool circuitFerme()
    {
        if (piles.Count == 0) return false;

        foreach (GameObject pileGO in piles)
        {
            Composante pile = pileGO.GetComponent<Composante>();
            if (pile == null) continue;
            if (!pile.getAttach1() || !pile.getAttach2()) continue;

            Composante depart = null;

            foreach ((Composante voisin, int borne) in pile.GetConnexions())
            {
                if (borne == 2) depart = voisin; // borne + = départ
            }

            if (depart == null) continue;

            // destination = la pile elle-même, via borne –
            HashSet<Composante> visites = new HashSet<Composante>();
            visites.Add(depart); // ne pas revenir au départ immédiatement

            if (DFS(depart, pile, visites, pile))
                return true;
        }

        return false;
    }

    bool DFS(Composante courant, Composante pileCible, HashSet<Composante> visites, Composante pileDepart)
    {
        foreach ((Composante voisin, int borne) in courant.GetConnexions())
        {
            // On est revenu à la pile cible via sa borne – (borne 1) → circuit fermé !
            if (voisin == pileCible && borne == 1) return true;

            if (visites.Contains(voisin)) continue;
            visites.Add(voisin);

            if (DFS(voisin, pileCible, visites, pileDepart))
                return true;
        }

        return false;
    }

    // ─── Simulation (série simple) ────────────────────────────────────

    void SimulerCircuit()
    {
        float tensionTotale = 0f;
        float resistanceTotale = 0f;

        foreach (GameObject g in piles)
        {
            Pile p = g.GetComponent<Pile>();
            if (p != null) tensionTotale += p.Tension;
        }

        foreach (GameObject g in resistances)
        {
            Resistance r = g.GetComponent<Resistance>();
            if (r != null) resistanceTotale += r.ValeurOhms;
        }

        if (resistanceTotale <= 0f) return;

        float courant = tensionTotale / resistanceTotale;

        foreach (GameObject g in piles)
            g.GetComponent<Composante>()?.SetCourant(courant);
        foreach (GameObject g in resistances)
            g.GetComponent<Composante>()?.SetCourant(courant);
    }
}


