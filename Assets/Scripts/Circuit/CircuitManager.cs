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
    public void AddFil(List<GameObject> anchors)
    {
        GameObject a1 = anchors[0];
        GameObject a2 = anchors[1];

        Composante composante1 = a1.GetComponent<Anchor>().GetAttache().GetComponent<Composante>();
        Composante composante2 = a2.GetComponent<Anchor>().GetAttache().GetComponent<Composante>();

        composante1.Connecter(composante2);

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
    
}


