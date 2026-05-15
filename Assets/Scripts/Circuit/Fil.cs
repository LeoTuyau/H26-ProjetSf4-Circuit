using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Représente un fil électrique reliant deux noeuds du circuit.
/// Responsable de :
/// - Positionner et orienter visuellement le fil entre ses deux noeuds
/// - Animer des flèches se déplaçant dans le sens du courant
/// - Afficher le potentiel électrique en mode debug
/// </summary>
public class Fil : MonoBehaviour
{
    private Noeud noeudA; // Premier noeud du fil
    private Noeud noeudB; // Second noeud du fil

    /// <summary>Potentiel électrique moyen du fil, calculé par le CircuitManager.</summary>
    public float Potentiel;

    [SerializeField] TextMeshPro textPrefab; // Prefab du texte affiché pour le potentiel
    TextMeshPro textP;                       // Instance du texte instanciée en jeu

    private bool modePotentiel = false; // Vrai si l'affichage du potentiel est actif

    [SerializeField] private GameObject fleche;    // Prefab de la flèche animée
    [SerializeField] private float vitesse = 1f; // Vitesse de déplacement des flèches
    [SerializeField] private int nbFleches = 3;  // Nombre de flèches animées sur le fil

    private List<GameObject> fleches = new List<GameObject>(); // Instances des flèches
    private List<float> offsets = new List<float>();      // Position normalisée [0,1] de chaque flèche sur le fil

    /// <summary>
    /// Indique si NoeudA est la borne positive (source du courant).
    /// Stocké au moment de la connexion, indépendant de l'ordre de clic de l'utilisateur.
    /// </summary>
    private bool bornePlusEstA = true;
    public bool BornePlusEstA => bornePlusEstA;
    public void SetBornePlusEstA(bool v) => bornePlusEstA = v;

    // ─── Initialisation ───────────────────────────────────────────────

    /// <summary>
    /// Initialise le fil avec ses deux noeuds et crée les flèches d'animation.
    /// Doit être appelé immédiatement après l'instanciation du fil.
    /// </summary>
    /// <param name="a">Noeud A (premier extrémité du fil).</param>
    /// <param name="b">Noeud B (deuxième extrémité du fil).</param>
    public void SetNoeuds(Noeud a, Noeud b)
    {
        noeudA = a;
        noeudB = b;
        InitFleches();
    }

    /// <summary>
    /// Détruit les anciennes flèches et en instancie de nouvelles.
    /// Les flèches sont réparties uniformément le long du fil et désactivées par défaut.
    /// </summary>
    private void InitFleches()
    {
        if (fleche == null) return;

        foreach (GameObject f in fleches)
            if (f != null) Destroy(f);
        fleches.Clear();
        offsets.Clear();

        for (int i = 0; i < nbFleches; i++)
        {
            GameObject f = Instantiate(fleche, transform.position, transform.rotation);
            f.SetActive(false);
            fleches.Add(f);
            offsets.Add((float)i / nbFleches); // Répartition uniforme des offsets de départ
        }
    }

    // ─── Sens du courant ──────────────────────────────────────────────

    /// <summary>
    /// Sens du courant dans le fil :
    ///  1  = courant va de A vers B
    /// -1  = courant va de B vers A
    ///  0  = pas de courant (circuit ouvert ou courant nul)
    /// </summary>
    public int Sens { get; private set; } = 0;

    /// <summary>Définit le sens du courant. Appelé par le CircuitManager à chaque frame.</summary>
    public void SetSens(int s) => Sens = s;

    /// <summary>Accès en lecture au noeud A du fil.</summary>
    public Noeud NoeudA => noeudA;

    /// <summary>Accès en lecture au noeud B du fil.</summary>
    public Noeud NoeudB => noeudB;

    // ─── Visuel ───────────────────────────────────────────────────────

    /// <summary>
    /// Instancie le texte de potentiel au démarrage.
    /// </summary>
    void Start()
    {
        textP = Instantiate(textPrefab, transform.position, Quaternion.LookRotation(transform.forward));
    }

    /// <summary>
    /// Chaque frame :
    /// - Repositionne et oriente le fil entre ses deux noeuds.
    /// - Met à l'échelle le fil selon la distance entre les noeuds.
    /// - Lance l'animation des flèches.
    /// - Met à jour l'affichage du potentiel si le mode est actif.
    /// </summary>
    void Update()
    {
        if (noeudA == null || noeudB == null) return;

        Vector3 posA = GetPosNoeud(noeudA);
        Vector3 posB = GetPosNoeud(noeudB);
        Vector3 direction = posA - posB;

        // Centre du fil entre les deux noeuds
        transform.position = (posA + posB) / 2f;

        // Rotation pour aligner le fil le long de la direction A→B
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Étirement vertical pour couvrir exactement la distance entre les noeuds
        transform.localScale = new Vector3(transform.localScale.x, direction.magnitude, 1f);

        AnimerFleches(posA, posB);

        if (modePotentiel)
        {
            textP.text = Potentiel + " V";
            textP.transform.position = transform.position + new Vector3(-0.4f, 0, -1);
            textP.transform.rotation = Quaternion.LookRotation(transform.forward);
        }
    }

    /// <summary>
    /// Retourne la position mondiale d'un noeud.
    /// Si le noeud appartient à une composante, sa position est calculée
    /// relativement à la composante et à son offset (position sur la composante).
    /// </summary>
    /// <param name="n">Le noeud dont on veut la position.</param>
    /// <returns>Position mondiale du noeud.</returns>
    Vector3 GetPosNoeud(Noeud n)
    {
        if (n.Parent != null)
            return n.Parent.transform.position + n.Parent.transform.right * n.Offset;
        return n.transform.position;
    }

    // ─── Animation flèches ────────────────────────────────────────────

    /// <summary>
    /// Anime les flèches se déplaçant le long du fil dans le sens du courant.
    /// Si Sens == 0, toutes les flèches sont masquées.
    /// Sinon, les flèches glissent de la source vers la destination en bouclant.
    /// </summary>
    /// <param name="posA">Position mondiale du noeud A.</param>
    /// <param name="posB">Position mondiale du noeud B.</param>
    private void AnimerFleches(Vector3 posA, Vector3 posB)
    {
        if (fleches.Count == 0) return;

        if (Sens != 0)
        {
            // Sens == 1  : courant A → B, source = posA
            // Sens == -1 : courant B → A, source = posB
            Vector3 source = Sens == 1 ? posA : posB;
            Vector3 dest = Sens == 1 ? posB : posA;

            Vector3 dir = (dest - source).normalized;
            if (dir == Vector3.zero) dir = Vector3.up;

            // Rotation de la flèche pour pointer vers la destination
            float rotAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rot = Quaternion.Euler(0f, 0f, rotAngle);

            for (int i = 0; i < fleches.Count; i++)
            {
                // Avance l'offset et boucle entre 0 et 1
                offsets[i] += Time.deltaTime * vitesse;
                offsets[i] = Mathf.Repeat(offsets[i], 1f);

                // Positionne la flèche le long du fil (légèrement devant sur l'axe Z)
                fleches[i].transform.position = Vector3.Lerp(source, dest, offsets[i]) + new Vector3(0, 0, -1);
                fleches[i].transform.rotation = rot;
                fleches[i].SetActive(true);
            }
        }
        else
        {
            // Pas de courant : masquer toutes les flèches
            foreach (GameObject f in fleches)
                f.SetActive(false);
        }
    }

    // ─── Nettoyage ────────────────────────────────────────────────────

    /// <summary>
    /// Détruit toutes les flèches instanciées quand le fil est détruit.
    /// Évite les fuites d'objets dans la scène lors d'un reset du circuit.
    /// </summary>
    void OnDestroy()
    {
        foreach (GameObject f in fleches)
            if (f != null) Destroy(f);
    }

    /// <summary>
    /// Active ou désactive l'affichage du potentiel électrique sur le fil.
    /// Quand désactivé, le texte est vidé immédiatement.
    /// </summary>
    /// <param name="b">Vrai pour afficher le potentiel, faux pour le masquer.</param>
    public void ToggleModeP(bool b)
    {
        modePotentiel = b;
        if (!b)
        {
            textP.text = "";
        }
    }
}