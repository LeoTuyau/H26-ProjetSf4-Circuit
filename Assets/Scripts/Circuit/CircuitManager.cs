using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire principal du circuit électrique.
/// Responsable de :
/// - La gestion des composantes, piles et fils
/// - La détection de circuit fermé (DFS)
/// - La simulation électrique par la méthode des noeuds (MNA)
/// - L'animation des fils selon le sens du courant
/// </summary>
public class CircuitManager : MonoBehaviour
{
    [SerializeField] List<Composante> composantes = new List<Composante>(); // Toutes les composantes du circuit (piles incluses)
    [SerializeField] List<Pile> piles = new List<Pile>();                   // Piles présentes dans le circuit
    [SerializeField] List<Fil> fils = new List<Fil>();                      // Fils reliant les noeuds
    [SerializeField] List<Noeud> noeudsAffiches = new List<Noeud>();        // Noeuds visibles en mode fil
    [SerializeField] ItemSpawner itemSpawner;
    [SerializeField] BouttonFil BtnFil;
    [SerializeField] MouseManager mouseManager;

    bool modeFil = false;        // Vrai si le mode connexion par fil est actif
    bool modePotentiel = false;  // Vrai si l'affichage du potentiel sur les fils est actif
    bool modeSlider = false;     // Vrai si le mode slider (ajustement des composantes) est actif

    void Start()
    {
        itemSpawner = GetComponent<ItemSpawner>();
    }

    /// <summary>
    /// Chaque frame : si le circuit est fermé, simule et met à jour les sens des fils.
    /// Sinon, arrête toutes les animations de courant.
    /// </summary>
    void Update()
    {
        if (circuitFerme())
        {
            SimulerCircuit();
            MettreAJourSensFils();
        }
        else
        {
            foreach (Fil fil in fils)
                fil.SetSens(0);
        }
    }

    // ─── Mode slider ──────────────────────────────────────────────────

    /// <summary>
    /// Active ou désactive le mode slider sur toutes les composantes.
    /// Permet à l'utilisateur d'ajuster les valeurs des composantes via un slider.
    /// </summary>
    public void ToggleModeSlider()
    {
        modeSlider = !modeSlider;
        foreach (Composante composante in composantes)
            composante.ToggleModeSlider(modeSlider);
    }

    // ─── Mode affichage potentiel ─────────────────────────────────────

    /// <summary>
    /// Active ou désactive l'affichage du potentiel électrique sur chaque fil.
    /// </summary>
    public void ToggleModePotentielFil()
    {
        modePotentiel = !modePotentiel;
        foreach (Fil fil in fils)
            fil.ToggleModeP(modePotentiel);
    }

    // ─── Mode fil ─────────────────────────────────────────────────────

    /// <summary>
    /// Active ou désactive le mode de connexion par fil.
    /// En mode actif, tous les noeuds des composantes deviennent visibles et cliquables.
    /// En mode inactif, les noeuds sont masqués et la souris revient en mode défaut.
    /// </summary>
    public void ToggleFil()
    {
        if (!modeFil)
        {
            noeudsAffiches.Clear();
            foreach (Composante c in composantes)
            {
                c.Noeud1.gameObject.SetActive(true);
                c.Noeud2.gameObject.SetActive(true);
                noeudsAffiches.Add(c.Noeud1);
                noeudsAffiches.Add(c.Noeud2);
            }

            BtnFil.SetSelected(true);
            modeFil = true;
            mouseManager.SetMode("fil");
        }
        else
        {
            foreach (Noeud n in noeudsAffiches)
                n.gameObject.SetActive(false);
            noeudsAffiches.Clear();

            BtnFil.SetSelected(false);
            modeFil = false;
            mouseManager.SetMode("defaut");
        }
    }

    /// <summary>
    /// Surcharge permettant de forcer l'état du mode fil.
    /// N'agit que si l'état demandé diffère de l'état actuel.
    /// </summary>
    /// <param name="modeFil">État désiré du mode fil.</param>
    public void ToggleFil(bool modeFil)
    {
        if (this.modeFil != modeFil) ToggleFil();
    }

    /// <summary>
    /// Crée un fil entre deux noeuds, les connecte logiquement et l'ajoute à la liste.
    /// </summary>
    /// <param name="n1">Noeud de départ du fil.</param>
    /// <param name="n2">Noeud d'arrivée du fil.</param>
    public void AddFil(Noeud n1, Noeud n2)
    {
        n1.Connecter(n2);
        GameObject filGO = itemSpawner.SpawnFil(n1, n2);
        Fil fil = filGO.GetComponent<Fil>();
        if (fil != null) fils.Add(fil);
    }

    /// <summary>
    /// Ajoute une composante au circuit.
    /// Si c'est une pile et qu'il en existe déjà une, la nouvelle est rejetée et détruite
    /// (une seule pile autorisée par circuit).
    /// </summary>
    /// <param name="c">La composante à ajouter.</param>
    public void AddComposante(Composante c)
    {
        composantes.Add(c);
        if (c is Pile p)
        {
            if (piles.Count > 0)
            {
                composantes.Remove(c);
                Destroy(c.gameObject);
                return;
            }
            piles.Add(p);
        }
    }

    // ─── Détection de circuit fermé (DFS) ────────────────────────────

    /// <summary>
    /// Vérifie si le circuit est fermé, c'est-à-dire s'il existe un chemin
    /// entre la borne positive et la borne négative de chaque pile.
    /// Utilise une recherche en profondeur (DFS).
    /// </summary>
    /// <returns>Vrai si au moins un circuit fermé est détecté.</returns>
    bool circuitFerme()
    {
        if (piles.Count == 0) return false;

        foreach (Pile pile in piles)
        {
            Noeud depart = pile.Noeud2;       // Borne positive (+)
            Noeud destination = pile.Noeud1;  // Borne négative (-)

            if (depart.Voisins.Count == 0 || destination.Voisins.Count == 0)
                continue;

            HashSet<Noeud> visites = new HashSet<Noeud>();
            if (DFS(depart, destination, visites, null))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Recherche en profondeur (DFS) pour trouver un chemin entre deux noeuds.
    /// Traverse les composantes en passant automatiquement à leur autre noeud.
    /// </summary>
    /// <param name="courant">Noeud actuellement visité.</param>
    /// <param name="destination">Noeud cible à atteindre.</param>
    /// <param name="visites">Ensemble des noeuds déjà visités.</param>
    /// <param name="precedent">Noeud précédent (pour éviter de rebrousser chemin).</param>
    /// <returns>Vrai si la destination est atteignable depuis le noeud courant.</returns>
    bool DFS(Noeud courant, Noeud destination, HashSet<Noeud> visites, Noeud precedent)
    {
        foreach (Noeud voisin in courant.Voisins)
        {
            if (voisin == destination) return true;
            if (voisin == precedent) continue;
            if (visites.Contains(voisin)) continue;

            Noeud autreNoeud = GetAutreNoeud(voisin);
            if (autreNoeud == null) continue;

            visites.Add(voisin);
            visites.Add(autreNoeud);

            if (DFS(autreNoeud, destination, visites, voisin))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Retourne le noeud opposé d'une composante par rapport au noeud donné.
    /// Si le noeud n'a pas de parent composante, retourne le noeud lui-même.
    /// </summary>
    /// <param name="n">Le noeud dont on cherche l'opposé.</param>
    /// <returns>Le noeud opposé sur la même composante, ou le noeud lui-même si pas de parent.</returns>
    Noeud GetAutreNoeud(Noeud n)
    {
        Composante c = n.Parent;
        if (c == null) return n;
        if (n == c.Noeud1) return c.Noeud2;
        if (n == c.Noeud2) return c.Noeud1;
        return null;
    }

    // ─── Simulation — Méthode des noeuds (MNA) ───────────────────────

    /// <summary>
    /// Simule le circuit électrique en utilisant la Méthode des Noeuds Modifiée (MNA).
    /// 
    /// Étapes :
    /// 1. Collecte tous les noeuds accessibles depuis la pile.
    /// 2. Assigne un index matriciel à chaque noeud (les noeuds connectés par fil partagent le même index).
    /// 3. Construit la matrice de conductance G et le vecteur de courant I.
    /// 4. Les piles sont modélisées par une grande conductance (stamp de tension).
    /// 5. Résout le système G·V = I par élimination de Gauss.
    /// 6. Met à jour le potentiel de chaque noeud.
    /// 7. Calcule le courant traversant chaque composante.
    /// 8. Met à jour le potentiel moyen de chaque fil.
    /// </summary>
    void SimulerCircuit()
    {
        List<Noeud> tousLesNoeuds = CollecterNoeuds();
        if (tousLesNoeuds.Count < 2) return;

        // La borne négative de la première pile est la référence (masse, 0V)
        Noeud gnd = piles[0].Noeud1;
        gnd.Potentiel = 0f;
        gnd.Index = -1;

        // Assigne un index matriciel à chaque groupe de noeuds connectés par fil
        Dictionary<Noeud, int> indexMap = new Dictionary<Noeud, int>();
        int n = 0;

        foreach (Noeud noeud in tousLesNoeuds)
        {
            if (noeud == gnd) continue;
            if (indexMap.ContainsKey(noeud)) continue;

            List<Noeud> groupe = TrouverGroupe(noeud);
            bool contientGnd = groupe.Contains(gnd);

            foreach (Noeud membre in groupe)
                indexMap[membre] = contientGnd ? -1 : n;

            if (!contientGnd) n++;
        }

        foreach (Noeud noeud in tousLesNoeuds)
        {
            if (noeud == gnd) { noeud.Index = -1; continue; }
            noeud.Index = indexMap.ContainsKey(noeud) ? indexMap[noeud] : -1;
        }

        if (n == 0) return;

        // Construction de la matrice de conductance G et du vecteur I
        float[,] G = new float[n, n];
        float[] I = new float[n];

        // Stamp des résistances : g = 1/R ajouté en diagonale et soustrait aux positions croisées
        foreach (Composante c in composantes)
        {
            if (c is Pile) continue;
            if (c.ValeurOhms <= 0f) continue;

            float g = 1f / c.ValeurOhms;
            int i = c.Noeud1.Index;
            int j = c.Noeud2.Index;

            if (i >= 0) G[i, i] += g;
            if (j >= 0) G[j, j] += g;
            if (i >= 0 && j >= 0)
            {
                G[i, j] -= g;
                G[j, i] -= g;
            }
        }

        // Stamp des piles : modélisation par grande conductance (méthode de la source de tension)
        // Un bigG très élevé force le potentiel du noeud+ à la tension de la pile
        foreach (Pile pile in piles)
        {
            int iPlus = pile.Noeud2.Index;   // Borne positive
            int iMoins = pile.Noeud1.Index;  // Borne négative

            if (iPlus < 0) continue;

            float bigG = 1e6f;
            G[iPlus, iPlus] += bigG;
            I[iPlus] += bigG * pile.Tension;

            if (iMoins >= 0)
            {
                G[iMoins, iMoins] += bigG;
                G[iPlus, iMoins] -= bigG;
                G[iMoins, iPlus] -= bigG;
                I[iMoins] -= bigG * pile.Tension;
            }
        }

        // Résolution du système linéaire G·V = I
        float[] V = GaussElimination(G, I, n);
        if (V == null)
        {
            Debug.LogWarning("Système singulier — circuit mal formé ?");
            return;
        }

        // Mise à jour des potentiels de chaque noeud
        foreach (Noeud noeud in tousLesNoeuds)
            noeud.Potentiel = noeud.Index >= 0 ? V[noeud.Index] : 0f;

        // Calcul du courant dans chaque composante
        foreach (Composante c in composantes)
        {
            if (c is Pile)
            {
                // Pour la pile : somme des courants sortant vers les composantes voisines
                float courant = 0f;
                foreach (Noeud voisin in c.Noeud2.Voisins)
                {
                    Composante comp = voisin.Parent;
                    if (comp == null || comp is Pile) continue;
                    if (comp.ValeurOhms <= 0f) continue;

                    Noeud autreNoeud = GetAutreNoeud(voisin);
                    if (autreNoeud == null) continue;

                    float deltaV = c.Noeud2.Potentiel - autreNoeud.Potentiel;
                    courant += Mathf.Abs(deltaV / comp.ValeurOhms);
                }
                c.SetCourant(courant);
            }
            else
            {
                // Pour une résistance : I = |ΔV / R|
                float delta = c.Noeud2.Potentiel - c.Noeud1.Potentiel;
                c.SetCourant(c.ValeurOhms > 0f ? Mathf.Abs(delta / c.ValeurOhms) : 0f);
            }
        }

        // Potentiel affiché sur le fil = moyenne des deux noeuds
        foreach (Fil fil in fils)
            fil.Potentiel = (fil.NoeudA.Potentiel + fil.NoeudB.Potentiel) / 2f;
    }

    // ─── Sens des fils ────────────────────────────────────────────────

    /// <summary>
    /// Met à jour le sens du courant sur tous les fils du circuit.
    /// </summary>
    void MettreAJourSensFils()
    {
        foreach (Fil fil in fils)
            fil.SetSens(CalculerSensFil(fil));
    }

    /// <summary>
    /// Détermine le sens du courant dans un fil (1, -1 ou 0) selon la topologie du circuit.
    /// 
    /// Priorité de décision :
    /// 1. Si un côté est une sortie et l'autre une entrée → sens évident.
    /// 2. Si l'une des composantes est une pile → le sens suit la convention pile.
    /// 3. Si les deux sont des résistances avec même orientation → compare les potentiels
    ///    de Noeud2 de chaque composante pour trancher.
    /// </summary>
    /// <param name="fil">Le fil dont on veut calculer le sens.</param>
    /// <returns>1 (A→B), -1 (B→A) ou 0 (pas de courant).</returns>
    int CalculerSensFil(Fil fil)
    {
        Noeud a = fil.NoeudA;
        Noeud b = fil.NoeudB;

        Composante compA = a?.Parent;
        Composante compB = b?.Parent;

        if (compA == null || compB == null) return 0;
        if (compA.Courant <= 0.0001f || compB.Courant <= 0.0001f) return 0;

        // Noeud2 est la borne de sortie de la composante
        bool aEstSortie = (a == compA.Noeud2);
        bool bEstSortie = (b == compB.Noeud2);

        // Cas 1 : sortie d'un côté, entrée de l'autre → sens non ambigu
        if (aEstSortie && !bEstSortie) return 1;
        if (!aEstSortie && bEstSortie) return -1;

        // Cas 2 : une pile impliquée → le sens suit la convention de la pile
        if (compA is Pile) return aEstSortie ? 1 : -1;
        if (compB is Pile) return bEstSortie ? -1 : 1;

        // Cas 3 : deux résistances avec même orientation (entrée-entrée ou sortie-sortie)
        // Compare les potentiels de Noeud2 de chaque composante pour déterminer le sens
        float potSortieA = compA.Noeud2.Potentiel;
        float potSortieB = compB.Noeud2.Potentiel;
        float diff = potSortieA - potSortieB;

        if (Mathf.Abs(diff) < 0.0001f) return 0;
        return diff > 0 ? 1 : -1;
    }

    // ─── TrouverGroupe ────────────────────────────────────────────────

    /// <summary>
    /// Trouve tous les noeuds connectés au noeud de départ par des fils (voisins directs).
    /// Utilisé pour regrouper les noeuds au même potentiel (court-circuit par fil idéal).
    /// </summary>
    /// <param name="depart">Noeud de départ de la recherche.</param>
    /// <returns>Liste de tous les noeuds du même groupe de potentiel.</returns>
    List<Noeud> TrouverGroupe(Noeud depart)
    {
        List<Noeud> groupe = new List<Noeud>();
        HashSet<Noeud> vus = new HashSet<Noeud>();
        Queue<Noeud> queue = new Queue<Noeud>();
        queue.Enqueue(depart);

        while (queue.Count > 0)
        {
            Noeud courant = queue.Dequeue();
            if (vus.Contains(courant)) continue;
            vus.Add(courant);
            groupe.Add(courant);

            foreach (Noeud voisin in courant.Voisins)
                if (!vus.Contains(voisin))
                    queue.Enqueue(voisin);
        }

        return groupe;
    }

    // ─── CollecterNoeuds ──────────────────────────────────────────────

    /// <summary>
    /// Collecte tous les noeuds accessibles depuis la première pile du circuit.
    /// Parcourt à la fois les connexions par fil (Voisins) et les composantes (GetAutreNoeud).
    /// Seuls les noeuds atteignables sont simulés.
    /// </summary>
    /// <returns>Liste de tous les noeuds accessibles dans le circuit.</returns>
    List<Noeud> CollecterNoeuds()
    {
        HashSet<Noeud> vus = new HashSet<Noeud>();
        List<Noeud> liste = new List<Noeud>();

        if (piles.Count == 0) return liste;

        Queue<Noeud> queue = new Queue<Noeud>();
        queue.Enqueue(piles[0].Noeud1);
        queue.Enqueue(piles[0].Noeud2);

        while (queue.Count > 0)
        {
            Noeud courant = queue.Dequeue();
            if (!vus.Add(courant)) continue;
            liste.Add(courant);

            foreach (Noeud voisin in courant.Voisins)
                if (!vus.Contains(voisin))
                    queue.Enqueue(voisin);

            Noeud autre = GetAutreNoeud(courant);
            if (autre != null && !vus.Contains(autre))
                queue.Enqueue(autre);
        }

        return liste;
    }

    // ─── GaussElimination ─────────────────────────────────────────────

    /// <summary>
    /// Résout le système linéaire A·x = b par élimination de Gauss-Jordan avec pivot partiel.
    /// Utilisé pour calculer les potentiels nodaux du circuit.
    /// </summary>
    /// <param name="A">Matrice de conductance G (n×n).</param>
    /// <param name="b">Vecteur de courant I (taille n).</param>
    /// <param name="size">Dimension du système.</param>
    /// <returns>Vecteur solution x (potentiels), ou null si le système est singulier.</returns>
    float[] GaussElimination(float[,] A, float[] b, int size)
    {
        // Copie locale pour ne pas modifier les originaux
        float[,] M = new float[size, size];
        float[] r = new float[size];
        for (int i = 0; i < size; i++)
        {
            r[i] = b[i];
            for (int j = 0; j < size; j++)
                M[i, j] = A[i, j];
        }

        for (int col = 0; col < size; col++)
        {
            // Recherche du pivot maximal (stabilité numérique)
            int pivotRow = col;
            float maxVal = Mathf.Abs(M[col, col]);
            for (int row = col + 1; row < size; row++)
            {
                if (Mathf.Abs(M[row, col]) > maxVal)
                {
                    maxVal = Mathf.Abs(M[row, col]);
                    pivotRow = row;
                }
            }

            // Système singulier : circuit mal formé
            if (maxVal < 1e-10f) return null;

            // Échange de lignes
            if (pivotRow != col)
            {
                for (int j = 0; j < size; j++)
                    (M[col, j], M[pivotRow, j]) = (M[pivotRow, j], M[col, j]);
                (r[col], r[pivotRow]) = (r[pivotRow], r[col]);
            }

            // Élimination de toutes les autres lignes (Gauss-Jordan complet)
            for (int row = 0; row < size; row++)
            {
                if (row == col) continue;
                float factor = M[row, col] / M[col, col];
                for (int j = col; j < size; j++)
                    M[row, j] -= factor * M[col, j];
                r[row] -= factor * r[col];
            }
        }

        // Extraction de la solution
        float[] x = new float[size];
        for (int i = 0; i < size; i++)
            x[i] = r[i] / M[i, i];

        return x;
    }

    // ─── Reset ────────────────────────────────────────────────────────

    /// <summary>
    /// Réinitialise complètement le circuit.
    /// Détruit tous les fils et composantes, vide toutes les listes,
    /// et remet la souris en mode défaut.
    /// </summary>
    public void ResetCircuit()
    {
        ToggleFil(false);

        foreach (Fil fil in fils)
            if (fil != null) Destroy(fil.gameObject);

        foreach (Composante c in composantes)
            if (c != null) Destroy(c.gameObject);

        fils.Clear();
        composantes.Clear();
        piles.Clear();
        noeudsAffiches.Clear();
        modeFil = false;
        mouseManager.SetMode("defaut");
    }
}