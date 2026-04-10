using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Composante : MonoBehaviour
{
    List<Composante> connexions = new List<Composante>();
    int nbConnexions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public List<Composante> getConnexions()
    {
        return connexions;
    }
    public int getNbConnexions()
    {
        return nbConnexions;
    }
}
