using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Composante : MonoBehaviour
{
    public string Nom { get; private set; }
    [SerializeField] protected List<Composante> connexions = new List<Composante>();
    [SerializeField] protected bool attach1 = false;
    [SerializeField] protected bool attach2 = false;
    [SerializeField] protected GameObject anchor1;
    [SerializeField] protected GameObject anchor2;

    public Composante(string nom)
    {
        Nom = nom;
    }

    public void Connecter(Composante autre)
    {
        if (connexions!=null && !connexions.Contains(autre))
        {
            connexions.Add(autre);
            autre.Connecter(this);
        }
    }

    public List<Composante> GetConnexions()
    {
        return connexions;
    }

    //Visuel
    public bool getAttach1() { return attach1; }
    public bool getAttach2() { return attach2; }
    public void setAttach1(bool attache) { attach1 = attache; }
    public void setAttach2(bool attache) { attach2 = attache; }
    public void setAnchor1(GameObject anchor)
    {
        anchor1 = anchor;
    }
    public void setAnchor2(GameObject anchor)
    {
        anchor2 = anchor;
    }
    public int getAnchor(GameObject anchor) // Trouver quel anchor on a
    {
        if (anchor == anchor1) return 1;
        if (anchor == anchor2) return 2;
        return 0;
    }
    public void removeAnchor1()
    {
        anchor1 = null;
    }
    public void removeAnchor2()
    {
        anchor2 = null;
    }
    public void removeAnchor(GameObject anchor)
    {
        if (anchor1 == anchor)
        {
            anchor1 = null;
        }
        else
        {
            anchor2 = null;
        }
    }
}