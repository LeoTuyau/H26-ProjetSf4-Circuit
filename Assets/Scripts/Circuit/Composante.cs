using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Composante : MonoBehaviour
{
    public string Nom { get; private set; }
    protected List<Composante> connexions = new List<Composante>();

    public Composante(string nom)
    {
        Nom = nom;
    }

    public void Connecter(Composante autre)
    {
        if (!connexions.Contains(autre))
        {
            connexions.Add(autre);
            autre.Connecter(this);
        }
    }

    public List<Composante> GetConnexions()
    {
        return connexions;
    }
}