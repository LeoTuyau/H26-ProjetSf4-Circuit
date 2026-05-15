using UnityEngine;

/// <summary>
/// Classe de base pour tout élément d'un circuit.
/// Chaque composante possède deux noeuds : noeud1 (borne –) et noeud2 (borne +).
/// </summary>
public class Composante : MonoBehaviour
{
    [SerializeField] protected Noeud noeud1; // borne –
    [SerializeField] protected Noeud noeud2; // borne +

    public Noeud Noeud1 => noeud1;
    public Noeud Noeud2 => noeud2;

    public void SetNoeud1(Noeud n) => noeud1 = n;
    public void SetNoeud2(Noeud n) => noeud2 = n;

    // ─── Simulation ───────────────────────────────────────────────────

    public virtual float Tension    => 0f;
    public virtual float ValeurOhms => 0f;
    public float Courant { get; set; }
    public void SetCourant(float c) => Courant = c;
    public float Puissance => Tension * Courant;

    // ─── Potentiels aux bornes ────────────────────────────────────────

    public float PotentielNoeud1 => noeud1 != null ? noeud1.Potentiel : 0f;
    public float PotentielNoeud2 => noeud2 != null ? noeud2.Potentiel : 0f;
}