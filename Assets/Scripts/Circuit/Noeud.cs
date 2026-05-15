using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Point de connexion entre composantes du circuit électrique.
/// 
/// Chaque <see cref="Composante"/> possède deux noeuds :
/// - Noeud1 (borne –) : référence basse tension
/// - Noeud2 (borne +) : référence haute tension
/// 
/// Un fil relie toujours exactement deux noeuds ensemble via <see cref="Connecter"/>.
/// Le <see cref="CircuitManager"/> assigne les potentiels et index après chaque simulation MNA.
/// </summary>
public class Noeud : MonoBehaviour
{
    // ─── Références ───────────────────────────────────────────────────

    [SerializeField] private Composante parent; // Composante à laquelle ce noeud appartient (null si noeud libre)
    [SerializeField] private int borne;         // 1 = borne –, 2 = borne +, 0 = noeud libre
    [SerializeField] private float offset;      // Distance en unités monde entre le centre de la composante et ce noeud

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // ─── Connexions ───────────────────────────────────────────────────

    /// <summary>Liste des noeuds directement connectés à celui-ci par des fils.</summary>
    private List<Noeud> voisins = new List<Noeud>();

    // ─── Simulation ───────────────────────────────────────────────────

    /// <summary>
    /// Potentiel électrique à ce noeud en volts.
    /// Calculé et assigné par le <see cref="CircuitManager"/> après chaque résolution MNA.
    /// </summary>
    public float Potentiel { get; set; }

    /// <summary>
    /// Index de ce noeud dans la matrice de conductance G.
    /// Assigné par le <see cref="CircuitManager"/> avant chaque simulation.
    /// -1 indique que ce noeud est la référence (GND) ou appartient au même groupe que GND.
    /// </summary>
    public int Index { get; set; } = -1;

    /// <summary>
    /// Cherche parmi les voisins un potentiel différent de celui de ce noeud,
    /// en regardant l'autre borne de la composante connectée.
    /// 
    /// Utilisé pour déterminer la direction du courant dans un fil,
    /// indépendamment de l'ordre dans lequel les connexions ont été établies.
    /// Retourne <see cref="Potentiel"/> si aucun potentiel significativement différent n'est trouvé.
    /// </summary>
    public float PotentielInterne
    {
        get
        {
            foreach (Noeud voisin in voisins)
            {
                Composante comp = voisin.Parent;
                if (comp == null) continue;

                float p1 = comp.PotentielNoeud1;
                float p2 = comp.PotentielNoeud2;

                // Si le voisin est la borne –, regarde le potentiel de la borne + (et vice-versa)
                if (voisin == comp.Noeud1 && Mathf.Abs(p2 - Potentiel) > 0.0001f)
                    return p2;
                if (voisin == comp.Noeud2 && Mathf.Abs(p1 - Potentiel) > 0.0001f)
                    return p1;
            }
            return Potentiel;
        }
    }

    // ─── API ──────────────────────────────────────────────────────────

    /// <summary>Composante parente de ce noeud. Null si noeud libre.</summary>
    public Composante Parent => parent;

    /// <summary>Numéro de borne : 1 = borne –, 2 = borne +, 0 = noeud libre.</summary>
    public int Borne => borne;

    /// <summary>Décalage latéral en unités monde par rapport au centre de la composante parente.</summary>
    public float Offset => offset;

    /// <summary>Liste des noeuds voisins connectés par des fils.</summary>
    public List<Noeud> Voisins => voisins;

    /// <summary>Assigne la composante parente (appelé par <see cref="ItemSpawner"/>).</summary>
    public void SetParent(Composante c) => parent = c;

    /// <summary>Définit le numéro de borne (appelé par <see cref="ItemSpawner"/>).</summary>
    public void SetBorne(int b) => borne = b;

    /// <summary>Définit le décalage latéral par rapport à la composante (appelé par <see cref="ItemSpawner"/>).</summary>
    public void SetOffset(float o) => offset = o;

    /// <summary>
    /// Connecte ce noeud à un autre de façon bidirectionnelle.
    /// Si la connexion existe déjà, n'en crée pas de doublon.
    /// Appelé par <see cref="CircuitManager.AddFil"/> lors de la création d'un fil.
    /// </summary>
    /// <param name="autre">Le noeud à connecter.</param>
    public void Connecter(Noeud autre)
    {
        if (!voisins.Contains(autre))
        {
            voisins.Add(autre);
            autre.voisins.Add(this);
        }
    }

    /// <summary>
    /// Déconnecte ce noeud d'un autre de façon bidirectionnelle.
    /// N'agit que si la connexion existe.
    /// </summary>
    /// <param name="autre">Le noeud à déconnecter.</param>
    public void Deconnecter(Noeud autre)
    {
        if (voisins.Remove(autre))
            autre.voisins.Remove(this);
    }

    // ─── Visuel ───────────────────────────────────────────────────────

    [SerializeField] private Renderer rend;            // Renderer du noeud pour changer son matériau
    [SerializeField] private Material matDefaut;       // Matériau normal (non sélectionné)
    [SerializeField] private Material matSelectionne;  // Matériau mis en évidence (sélectionné en mode fil)

    private bool selectionne = false;

    /// <summary>Vrai si ce noeud est actuellement sélectionné en mode fil.</summary>
    public bool Selectionne => selectionne;

    /// <summary>
    /// Bascule l'état de sélection du noeud et met à jour son matériau visuellement.
    /// Appelé par <see cref="MouseManager"/> lors d'un clic en mode fil.
    /// </summary>
    public void ToggleSelect()
    {
        selectionne = !selectionne;
        if (rend != null)
            rend.sharedMaterial = selectionne ? matSelectionne : matDefaut;
    }

    /// <summary>
    /// Force la désélection du noeud et restaure son matériau par défaut.
    /// Appelé après la création d'un fil pour réinitialiser l'état visuel.
    /// </summary>
    public void Deselectionner()
    {
        selectionne = false;
        if (rend != null)
            rend.sharedMaterial = matDefaut;
    }

    // ─── Position ─────────────────────────────────────────────────────

    /// <summary>
    /// Chaque frame : si le noeud appartient à une composante, suit sa position
    /// en se plaçant à <see cref="offset"/> unités sur l'axe droit de la composante.
    /// Permet au noeud de rester collé à sa composante quand elle est déplacée ou tournée.
    /// </summary>
    void Update()
    {
        if (parent != null)
            transform.position = parent.transform.position
                + parent.transform.right * offset;
    }
}