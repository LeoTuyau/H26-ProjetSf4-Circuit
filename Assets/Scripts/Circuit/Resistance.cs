using System.Collections.Generic;
using UnityEngine;

public class Resistance : Composante
{
    [SerializeField] bool attach1 = false;
    [SerializeField] bool attach2 = false;
    [SerializeField] GameObject anchor1;
    [SerializeField] GameObject anchor2;
    public float Valeur { get; private set; } // Ohms

    public Resistance(string nom, float valeur) : base(nom)
    {
        attach1 = false;
        attach2 = false;
        Valeur = valeur;
    }
    public void setAnchor1(GameObject anchor)
    {
        anchor1 = anchor;
        attach1 = true;
    }
    public void setAnchor2(GameObject anchor)
    {
        anchor2 = anchor;
        attach2 = true;
    }
    public void removeAnchor1()
    {
        anchor1 = null;
        attach1 = false;
    }
    public void removeAnchor2()
    {
        anchor2 = null;
        attach2 = false;
    }
    public void removeAnchor(GameObject anchor)
    {
        if (anchor1 == anchor)
        {
            anchor1 = null;
            attach1 = false;
        }
        else
        {
            anchor2 = null;
            attach2 = false;
        }
    }
    public bool getAttach1() { return attach1; }
    public bool getAttach2() { return attach2; }
}