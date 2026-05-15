using UnityEngine;

/// <summary>
/// Affiche un tooltip d'informations électriques au survol d'une composante.
/// 
/// À placer sur chaque prefab de composante (Pile, Résistance, etc.).
/// Communique avec le singleton <see cref="BoiteInfo"/> pour afficher
/// et masquer le panneau d'information.
/// 
/// Le tooltip est mis à jour en temps réel pendant le survol pour
/// refléter les valeurs simulées (courant, tension) qui changent à chaque frame.
/// 
/// Nécessite un Collider sur le GameObject pour que les événements
/// OnMouseEnter / OnMouseExit / OnMouseOver soient déclenchés par Unity.
/// </summary>
public class ComposanteHover : MonoBehaviour
{
    /// <summary>Référence à la composante électrique portée par ce GameObject.</summary>
    private Composante composante;

    /// <summary>
    /// Récupère la composante électrique au démarrage.
    /// </summary>
    void Awake()
    {
        composante = GetComponent<Composante>();
    }

    /// <summary>
    /// Déclenché quand la souris entre sur le collider de la composante.
    /// Affiche immédiatement le tooltip avec les valeurs courantes.
    /// </summary>
    void OnMouseEnter()
    {
        if (composante == null) return;
        BoiteInfo.Instance.Afficher(BuildTexte());
    }

    /// <summary>
    /// Déclenché quand la souris quitte le collider de la composante.
    /// Masque le tooltip.
    /// </summary>
    void OnMouseExit()
    {
        BoiteInfo.Instance.Cacher();
    }

    /// <summary>
    /// Déclenché chaque frame tant que la souris reste sur le collider.
    /// Met à jour le tooltip en temps réel pour refléter les valeurs
    /// simulées qui évoluent pendant la simulation (courant, tension).
    /// </summary>
    void OnMouseOver()
    {
        if (composante == null) return;
        BoiteInfo.Instance.Afficher(BuildTexte());
    }

    /// <summary>
    /// Construit le texte affiché dans le tooltip selon le type de composante.
    /// 
    /// Contenu affiché :
    /// - <see cref="Pile"/>       : Tension (V) + Courant (A)
    /// - <see cref="Resistance"/> : Résistance (Ω) + Courant (A)
    /// - Autres                   : Courant (A) uniquement
    /// </summary>
    /// <returns>Texte formaté prêt à être affiché dans le <see cref="BoiteInfo"/>.</returns>
    private string BuildTexte()
    {
        string texte = "";

        if (composante is Pile)
            texte += $"Tension : {composante.Tension:F2} V\n";

        if (composante is Resistance)
            texte += $"Résistance : {composante.ValeurOhms:F2} Ω\n";

        texte += $"Courant : {composante.Courant:F3} A";

        return texte.TrimEnd();
    }
}