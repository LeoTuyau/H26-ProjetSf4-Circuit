using System.Collections.Generic;
using UnityEngine;

public class Resistance : Composante
{

    public float Valeur { get; private set; } // Ohms

    public Resistance(string nom, float valeur) : base(nom)
    {
        Valeur = valeur;
    }
    
}