using TMPro;
using UnityEngine;

/// <summary>
/// ForceAttraction gère un objet possédant une charge électrique.
/// Il change de couleur selon le signe de la charge et affiche sa valeur.
/// </summary>
public class ForceAttraction : MonoBehaviour
{
    /// <summary>
    /// Texte affichant la valeur de la charge dans l’interface.
    /// </summary>
    [SerializeField] private TextMeshPro myText;

    /// <summary>
    /// Valeur de la charge électrique de l’objet.
    /// Positive ou négative selon le type de charge.
    /// </summary>
    [SerializeField] float charge = 5f;

    /// <summary>
    /// Matériau utilisé lorsque la charge est négative (charge "rouge").
    /// </summary>
    [SerializeField] Material redMaterial;

    /// <summary>
    /// Matériau utilisé lorsque la charge est positive (charge "bleue").
    /// </summary>
    [SerializeField] Material blueMaterial;

    /// <summary>
    /// Renderer de l’objet utilisé pour changer son apparence visuelle.
    /// </summary>
    private Renderer rend;

    /// <summary>
    /// Initialisation du composant Renderer.
    /// </summary>
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    /// <summary>
    /// Mise à jour appelée chaque frame.
    /// Change la couleur de l’objet selon le signe de la charge.
    /// </summary>
    void Update()
    {
        if (charge < 0)
        {
            rend.material = redMaterial;
        }
        else
        {
            rend.material = blueMaterial;
        }
    }

    /// <summary>
    /// Définit la charge de l’objet et met à jour l’affichage texte.
    /// </summary>
    /// <param name="charge">Nouvelle valeur de charge en Coulombs.</param>
    public void setCharge(float charge)
    {
        this.charge = charge;
        myText.text = charge.ToString("F1") + "C";
    }

    /// <summary>
    /// Retourne la valeur actuelle de la charge.
    /// </summary>
    public float getCharge()
    {
        return charge;
    }
}