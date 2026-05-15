using UnityEngine;

/// <summary>
/// Gère l'animation d'ouverture et de fermeture du menu d'aide.
/// 
/// Le menu glisse entre deux positions ancrées (pointA et pointB)
/// via une interpolation exponentielle (smooth damping), donnant
/// un effet de glissement fluide qui décélère en approchant la cible.
/// 
/// - pointA : position du menu fermé (caché)
/// - pointB : position du menu ouvert (visible)
/// 
/// Déclenché par la touche H via <see cref="MouseManager"/>.
/// </summary>
public class HelpMenu : MonoBehaviour
{
    /// <summary>Vrai si le menu est actuellement ouvert (ou en train de s'ouvrir).</summary>
    private bool Active;

    /// <summary>
    /// Facteur de lissage du mouvement.
    /// Plus la valeur est élevée, plus le glissement est rapide.
    /// Utilisé dans la formule : 1 - exp(-smooth × deltaTime).
    /// </summary>
    [SerializeField] private float smooth = 50;

    /// <summary>Position ancrée cible quand le menu est fermé.</summary>
    [SerializeField] private Vector2 pointA;

    /// <summary>Position ancrée cible quand le menu est ouvert.</summary>
    [SerializeField] private Vector2 pointB;

    private bool goA = false; // Vrai si le menu se dirige vers pointA (fermeture)
    private bool goB = false; // Vrai si le menu se dirige vers pointB (ouverture)

    private RectTransform rectTransform; // RectTransform du panneau UI animé

    /// <summary>
    /// Initialise le menu en état ouvert (Active = true) et récupère le RectTransform.
    /// </summary>
    void Start()
    {
        Active = true;
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Chaque frame : anime le glissement du menu vers sa position cible.
    /// Utilise une interpolation exponentielle pour un effet de décélération naturel.
    /// Snap automatique quand la distance à la cible est inférieure à 0.1 unités.
    /// </summary>
    void Update()
    {
        if (goA)
        {
            // Glissement vers pointA (fermeture)
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                pointA,
                1 - Mathf.Exp(-smooth * Time.deltaTime));

            // Snap final pour éviter une oscillation infinie autour de la cible
            if (Vector2.Distance(rectTransform.anchoredPosition, pointA) < 0.1f)
            {
                rectTransform.anchoredPosition = pointA;
                goA = false;
            }
        }
        else if (goB)
        {
            // Glissement vers pointB (ouverture)
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                pointB,
                1 - Mathf.Exp(-smooth * Time.deltaTime));

            // Snap final pour éviter une oscillation infinie autour de la cible
            if (Vector2.Distance(rectTransform.anchoredPosition, pointB) < 0.1f)
            {
                rectTransform.anchoredPosition = pointB;
                goB = false;
            }
        }
    }

    /// <summary>
    /// Bascule l'état du menu entre ouvert et fermé.
    /// - Si ouvert  → lance le glissement vers pointA (fermeture)
    /// - Si fermé   → lance le glissement vers pointB (ouverture)
    /// Appelé par <see cref="MouseManager"/> sur la touche H.
    /// </summary>
    public void Toggle()
    {
        if (Active)
        {
            goA = true;
            goB = false;
            Active = false;
        }
        else
        {
            goB = true;
            goA = false;
            Active = true;
        }
    }
}