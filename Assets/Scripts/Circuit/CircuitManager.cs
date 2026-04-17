using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{

    [SerializeField] List<GameObject> resistances = new List<GameObject>();
    [SerializeField] List<GameObject> piles = new List<GameObject>();
    [SerializeField] List<GameObject> fils = new List<GameObject>();
    [SerializeField] List<GameObject> anchors;

    [SerializeField] ItemSpawner itemSpawner;
    [SerializeField] BouttonFil BtnFil;
    [SerializeField] MouseManager mouseManager;
    bool modeFil = false;

    void Start()
    {
        itemSpawner = GetComponent<ItemSpawner>();
    }
    void Update()
    {
        
    }

    public void ToggleFil()
    {
        if (!modeFil)
        {
            anchors = itemSpawner.spawnAnchors(piles,resistances);
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
    public void AddFil(List<Anchor> anchors)
    {
        GameObject attache1 = anchors[0].GameObject();
        GameObject attache2 = anchors[1].GameObject();

        if (attache1.GetComponent<Anchor>().GetAttache().GetComponent<Pile>() != null)
        {
            attache1.GetComponent<Anchor>().GetAttache().GetComponent<Pile>().setAnchor1(attache1);
        }
        else if (attache1.GetComponent<Anchor>().GetAttache().GetComponent<Resistance>() != null)
        {
            attache1.GetComponent<Anchor>().GetAttache().GetComponent<Resistance>().setAnchor1(attache1);
        }


        if (attache2.GetComponent<Anchor>().GetAttache().GetComponent<Pile>() != null)
        {
            attache2.GetComponent<Anchor>().GetAttache().GetComponent<Pile>().setAnchor2(attache2);
        }
        else if (attache2.GetComponent<Anchor>().GetAttache().GetComponent<Resistance>() != null)
        {
            attache2.GetComponent<Anchor>().GetAttache().GetComponent<Resistance>().setAnchor2(attache2);
        }

        fils.Add(itemSpawner.SpawnFil(attache1, attache2));
    }
    public void AddPile(GameObject pile)
    {
        piles.Add(pile);
    }
    public void AddResistance(GameObject resistance)
    {
        resistances.Add(resistance);
    }
    
}


