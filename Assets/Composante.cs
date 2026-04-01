using System.Collections.Generic;

public abstract class Composante
{
    public string Nom { get; private set; }
    protected List<Composante> connexions = new List<Composante>();

    public Composante(string nom)
    {
        Nom = nom;
    }

    // Connexion à une autre composante
    public void Connecter(Composante autre)
    {
        if (!connexions.Contains(autre))
        {
            connexions.Add(autre);
            autre.Connecter(this); // Connexion bidirectionnelle
        }
    }

    // Retourne la liste des connexions
    public List<Composante> GetConnexions()
    {
        return connexions;
    }
}