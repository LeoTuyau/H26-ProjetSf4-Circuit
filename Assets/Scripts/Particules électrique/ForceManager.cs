using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;

/// <summary>
/// ForceManager simule une interaction de type force électrique (type Coulomb)
/// entre plusieurs objets "balles" dans la scène.
/// Chaque balle exerce une force sur toutes les autres.
/// </summary>
public class ForceManager : MonoBehaviour
{
    /// <summary>
    /// Liste des objets physiques (balles) soumis à la simulation de forces.
    /// </summary>
    [SerializeField] private List<GameObject> listeBalles = new List<GameObject>();

    /// <summary>
    /// Initialisation du script.
    /// Actuellement inutilisée.
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Mise à jour de la simulation physique à chaque frame.
    /// Calcule les forces d’interaction entre toutes les balles
    /// selon une loi de type Coulomb :
    /// F ∝ (q1 * q2) / r²
    /// </summary>
    void Update()
    {
        // Parcours de chaque balle (balle i)
        for (int i = 0; i < listeBalles.Count; i++)
        {
            Vector3 forceR = Vector3.zero;

            // Interaction avec toutes les autres balles (balle j)
            for (int j = 0; j < listeBalles.Count; j++)
            {
                if (j != i)
                {
                    // Direction de i vers j
                    Vector3 direction =
                        (listeBalles[j].transform.position -
                         listeBalles[i].transform.position).normalized;

                    // Distance entre les deux balles
                    float distance =
                        (listeBalles[j].transform.position -
                         listeBalles[i].transform.position).magnitude;

                    // Charges des deux objets
                    float q1 = listeBalles[i].GetComponent<ForceAttraction>().getCharge();
                    float q2 = listeBalles[j].GetComponent<ForceAttraction>().getCharge();

                    // Force de type Coulomb (simplifiée)
                    forceR += -direction * q1 * q2 / (distance * distance);
                }
            }

            // Application de la force résultante sur la balle i
            listeBalles[i].GetComponent<Rigidbody>().AddForce(forceR);
        }
    }

    /// <summary>
    /// Ajoute une nouvelle balle à la simulation.
    /// </summary>
    /// <param name="ball">Objet balle à ajouter.</param>
    public void AddBall(GameObject ball)
    {
        listeBalles.Add(ball);
    }
}

/*
EXTRA (ancien code de référence) :

Rigidbody rb = GetComponent<Rigidbody>();

Vector3 direction = (otherSphere.transform.position - transform.position).normalized;
float distance = (otherSphere.transform.position - transform.position).magnitude;

rb.AddForce(-direction * charge * q2 / (distance * distance), ForceMode.Force);
*/