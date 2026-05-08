using UnityEngine;
 
/// <summary>
/// À mettre sur chaque prefab de composante (Pile, Resistance).
/// Affiche le BoiteInfo au survol de la souris.
/// </summary>
public class ComposanteHover : MonoBehaviour
{
    private Composante composante;
 
    void Awake()
    {
        composante = GetComponent<Composante>();
    }
 
    void OnMouseEnter()
    {
        if (composante == null) return;
        BoiteInfo.Instance.Afficher(BuildTexte());
    }
 
    void OnMouseExit()
    {
        BoiteInfo.Instance.Cacher();
    }
 
    void OnMouseOver()
    {
        // Mettre à jour en temps réel (courant change pendant la simulation)
        if (composante == null) return;
        BoiteInfo.Instance.Afficher(BuildTexte());
    }
 
    private string BuildTexte()
    {
        string texte = "";
 
        // Tension — affiché pour Pile et Resistance
        if (composante is Pile)
            texte += $"Tension : {composante.Tension:F2} V";
 
        // Résistance — affiché seulement pour Resistance
        if (composante is Resistance)
            texte += $"Résistance : {composante.ValeurOhms:F2} Ω";
 
        // Courant — affiché pour tous
        texte += $"Courant : {composante.Courant:F3} A";
 
        return texte.TrimEnd();
    }
}