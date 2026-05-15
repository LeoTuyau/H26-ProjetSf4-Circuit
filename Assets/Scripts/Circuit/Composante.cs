using UnityEngine;

/// <summary>
/// Classe de base abstraite pour tout élément d'un circuit électrique.
/// 
/// Chaque composante possède deux noeuds :
/// - Noeud1 : borne négative (–), référence basse tension
/// - Noeud2 : borne positive (+), référence haute tension
/// 
/// Les classes dérivées (Pile, Resistance, etc.) surchargent
/// <see cref="Tension"/> et <see cref="ValeurOhms"/> selon leur nature.
/// Le <see cref="CircuitManager"/> met à jour <see cref="Courant"/> après chaque simulation.
/// </summary>
public class Composante : MonoBehaviour
{
    [SerializeField] protected Noeud noeud1; // Borne négative (–)
    [SerializeField] protected Noeud noeud2; // Borne positive (+)

    /// <summary>Borne négative (–) de la composante.</summary>
    public Noeud Noeud1 => noeud1;

    /// <summary>Borne positive (+) de la composante.</summary>
    public Noeud Noeud2 => noeud2;

    /// <summary>Définit la borne négative par code (utilisé à l'instanciation).</summary>
    public void SetNoeud1(Noeud n) => noeud1 = n;

    /// <summary>Définit la borne positive par code (utilisé à l'instanciation).</summary>
    public void SetNoeud2(Noeud n) => noeud2 = n;

    // ─── Simulation ───────────────────────────────────────────────────

    /// <summary>
    /// Tension aux bornes de la composante en volts.
    /// Surchargée par les classes dérivées :
    /// - <see cref="Pile"/> : retourne la tension nominale de la pile
    /// - <see cref="Resistance"/> : retourne R × I (loi d'Ohm)
    /// Valeur par défaut : 0.
    /// </summary>
    public virtual float Tension => 0f;

    /// <summary>
    /// Résistance de la composante en ohms.
    /// Surchargée par les classes dérivées :
    /// - <see cref="Pile"/> : retourne 0 (source idéale, pas de résistance interne)
    /// - <see cref="Resistance"/> : retourne la valeur configurée
    /// Valeur par défaut : 0.
    /// </summary>
    public virtual float ValeurOhms => 0f;

    /// <summary>
    /// Courant traversant la composante en ampères.
    /// Mis à jour par le <see cref="CircuitManager"/> après chaque résolution MNA.
    /// </summary>
    public float Courant { get; set; }

    /// <summary>
    /// Définit le courant traversant la composante.
    /// Appelé par le <see cref="CircuitManager"/> après chaque simulation.
    /// </summary>
    /// <param name="c">Courant en ampères.</param>
    public void SetCourant(float c) => Courant = c;

    /// <summary>
    /// Puissance dissipée ou fournie par la composante en watts.
    /// Calculée par P = U × I.
    /// </summary>
    public float Puissance => Tension * Courant;

    // ─── Potentiels aux bornes ────────────────────────────────────────

    /// <summary>
    /// Potentiel électrique à la borne négative (Noeud1), en volts.
    /// Retourne 0 si le noeud n'est pas assigné.
    /// </summary>
    public float PotentielNoeud1 => noeud1 != null ? noeud1.Potentiel : 0f;

    /// <summary>
    /// Potentiel électrique à la borne positive (Noeud2), en volts.
    /// Retourne 0 si le noeud n'est pas assigné.
    /// </summary>
    public float PotentielNoeud2 => noeud2 != null ? noeud2.Potentiel : 0f;

    // ─── Mode slider ──────────────────────────────────────────────────

    /// <summary>
    /// Active ou désactive le mode slider sur la composante.
    /// Méthode virtuelle vide par défaut — surchargée par <see cref="Pile"/>
    /// et <see cref="Resistance"/> pour afficher leur slider UI respectif.
    /// </summary>
    /// <param name="modeSlider">Vrai pour afficher le slider, faux pour le masquer.</param>
    public virtual void ToggleModeSlider(bool modeSlider) { }
}