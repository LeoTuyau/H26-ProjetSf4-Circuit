using System.Collections.Generic;
using TMPro;
using UnityEngine;
 
public class CircuitManager : MonoBehaviour
{
    [SerializeField] List<Composante> composantes  = new List<Composante>();
    [SerializeField] List<Pile>       piles        = new List<Pile>();
    [SerializeField] List<Noeud>      noeudsLibres = new List<Noeud>();
    [SerializeField] List<Noeud>      noeudsAffiches = new List<Noeud>();
    [SerializeField] TMP_Text         tmp;
    [SerializeField] ItemSpawner      itemSpawner;
    [SerializeField] BouttonFil       BtnFil;
    [SerializeField] MouseManager     mouseManager;
 
    bool modeFil = false;
 
    void Start()
    {
        itemSpawner = GetComponent<ItemSpawner>();
    }
 
    void Update()
    {
        if (circuitFerme())
        {
            tmp.text = "Circuit fermé";
            SimulerCircuit();
        }
        else
        {
            tmp.text = "Circuit ouvert";
        }
    }
 
    // ─── Mode fil ─────────────────────────────────────────────────────
 
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
            foreach (Noeud n in noeudsLibres)
                noeudsAffiches.Add(n);
 
            BtnFil.SetSelected(true);
            modeFil = true;
            mouseManager.SetMode("fil");
        }
        else
        {
            foreach (Noeud n in noeudsAffiches)
                if (n.Parent != null) // ne cacher que les noeuds de composantes
                    n.gameObject.SetActive(false);
            noeudsAffiches.Clear();
 
            BtnFil.SetSelected(false);
            modeFil = false;
            mouseManager.SetMode("defaut");
        }
    }
 
    public void ToggleFil(bool modeFil)
    {
        if (this.modeFil != modeFil) ToggleFil();
    }
 
    public void AddFil(Noeud n1, Noeud n2)
    {
        n1.Connecter(n2);
        itemSpawner.SpawnFil(n1, n2);
    }
 
    public void AddComposante(Composante c)
    {
        composantes.Add(c);
        if (c is Pile p) piles.Add(p);
    }
 
    public void AddNoeudLibre(Noeud n) => noeudsLibres.Add(n);
 
    // ─── Circuit fermé (DFS) ──────────────────────────────────────────
 
    bool circuitFerme()
    {
        if (piles.Count == 0) return false;
 
        foreach (Pile pile in piles)
        {
            Noeud depart      = pile.Noeud2; // borne +
            Noeud destination = pile.Noeud1; // borne –
 
            if (depart.Voisins.Count == 0 || destination.Voisins.Count == 0)
                continue;
 
            HashSet<Noeud> visites = new HashSet<Noeud>();
 
            if (DFS(depart, destination, visites, null))
                return true;
        }
 
        return false;
    }
 
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
 
    Noeud GetAutreNoeud(Noeud n)
    {
        Composante c = n.Parent;
        if (c == null) return n;
        if (n == c.Noeud1) return c.Noeud2;
        if (n == c.Noeud2) return c.Noeud1;
        return null;
    }
 
    // ─── Simulation — Méthode des noeuds (MNA) ───────────────────────
 
    void SimulerCircuit()
    {
        // 1. Collecter tous les noeuds du circuit
        List<Noeud> tousLesNoeuds = CollecterNoeuds();
        Debug.Log($"Noeuds collectés : {tousLesNoeuds.Count}");
        if (tousLesNoeuds.Count < 2) return;
 
        // 2. Le noeud1 de la première pile = GND (référence, index -1)
        Noeud gnd = piles[0].Noeud1;
        Debug.Log($"GND = {gnd.name}");
        gnd.Potentiel = 0f;
        gnd.Index = -1;
 
        // 3. Numéroter les autres noeuds
        int n = 0;
        foreach (Noeud noeud in tousLesNoeuds)
        {
            Debug.Log($"Noeud {noeud.name} index={noeud.Index}");
            if (noeud == gnd) continue;
            noeud.Index = n++;
        }
 
        // n = nombre de noeuds inconnus
        if (n == 0) return;
 
        // 4. Construire matrice G (n×n) et vecteur I (n)
        float[,] G = new float[n, n];
        float[]  I = new float[n];
 
        // Remplir G avec les résistances
        foreach (Composante c in composantes)
        {
            if (c is Pile) continue; // les piles sont gérées séparément
            if (c.ValeurOhms <= 0f) continue;
 
            float g = 1f / c.ValeurOhms; // conductance
            int i = c.Noeud1.Index;
            int j = c.Noeud2.Index;
 
            if (i >= 0) G[i, i] += g;
            if (j >= 0) G[j, j] += g;
            if (i >= 0 && j >= 0)
            {
                G[i, j] -= g;
                G[j, i] -= g;
            }
            // Si un côté est GND, sa contribution à G est déjà nulle (index -1)
        }
 
        // Remplir I avec les sources de tension (piles)
        // On modélise chaque pile comme une source de tension idéale
        // en utilisant la méthode de la source de tension modifiée :
        // le noeud + reçoit une tension fixe → on fixe son potentiel directement
        foreach (Pile pile in piles)
        {
            int iPlus  = pile.Noeud2.Index; // borne +
            int iMoins = pile.Noeud1.Index; // borne – (souvent GND = -1)
 
            if (iPlus >= 0)
            {
                // Forcer V[iPlus] = V[iMoins] + Tension
                // On utilise une grande conductance (stamp de tension)
                float bigG = 1e6f;
                G[iPlus, iPlus] += bigG;
                I[iPlus] += bigG * (pile.Tension + (iMoins >= 0 ? 0f : 0f));
 
                // Si borne – n'est pas GND
                if (iMoins >= 0)
                {
                    G[iMoins, iMoins] += bigG;
                    G[iPlus,  iMoins] -= bigG;
                    G[iMoins, iPlus]  -= bigG;
                    I[iMoins] -= bigG * pile.Tension;
                }
            }
        }
 
        // 5. Résoudre G × V = I par élimination de Gauss
        float[] V = GaussElimination(G, I, n);
        if (V == null)
        {
            Debug.LogWarning("Système singulier — circuit mal formé ?");
            return;
        }
 
        // 6. Injecter les potentiels dans les noeuds
        foreach (Noeud noeud in tousLesNoeuds)
        {
            Debug.Log($"Noeud {noeud.name} potentiel={noeud.Potentiel}");
            if (noeud.Index >= 0)
                noeud.Potentiel = V[noeud.Index];
        }
 
        // 7. Calculer le courant dans chaque composante : I = (V1 - V2) / R
        foreach (Composante c in composantes)
        {
            Debug.Log($"{c.name} courant={c.Courant}");
            float delta = c.Noeud2.Potentiel - c.Noeud1.Potentiel;
            if (c.ValeurOhms > 0f)
                c.SetCourant(delta / c.ValeurOhms);
            else
                c.SetCourant(0f);
        }
    }
 
    /// <summary>
    /// Collecte tous les noeuds uniques du circuit
    /// (noeuds des composantes + noeuds libres).
    /// </summary>
    List<Noeud> CollecterNoeuds()
    {
        HashSet<Noeud> vus = new HashSet<Noeud>();
        List<Noeud> liste  = new List<Noeud>();
 
        foreach (Composante c in composantes)
        {
            if (c.Noeud1 != null && vus.Add(c.Noeud1)) liste.Add(c.Noeud1);
            if (c.Noeud2 != null && vus.Add(c.Noeud2)) liste.Add(c.Noeud2);
        }
        foreach (Noeud n in noeudsLibres)
            if (vus.Add(n)) liste.Add(n);
 
        return liste;
    }
 
    /// <summary>
    /// Résolution du système linéaire A × x = b
    /// par élimination de Gauss avec pivot partiel.
    /// Retourne x, ou null si le système est singulier.
    /// </summary>
    float[] GaussElimination(float[,] A, float[] b, int size)
    {
        // Copie pour ne pas modifier les originaux
        float[,] M = new float[size, size];
        float[]  r = new float[size];
        for (int i = 0; i < size; i++)
        {
            r[i] = b[i];
            for (int j = 0; j < size; j++)
                M[i, j] = A[i, j];
        }
 
        // Élimination avec pivot partiel
        for (int col = 0; col < size; col++)
        {
            // Trouver le pivot maximal
            int pivotRow = col;
            float maxVal = Mathf.Abs(M[col, col]);
            for (int row = col + 1; row < size; row++)
            {
                if (Mathf.Abs(M[row, col]) > maxVal)
                {
                    maxVal   = Mathf.Abs(M[row, col]);
                    pivotRow = row;
                }
            }
 
            // Système singulier
            if (maxVal < 1e-10f) return null;
 
            // Échanger les lignes
            if (pivotRow != col)
            {
                for (int j = 0; j < size; j++)
                    (M[col, j], M[pivotRow, j]) = (M[pivotRow, j], M[col, j]);
                (r[col], r[pivotRow]) = (r[pivotRow], r[col]);
            }
 
            // Éliminer la colonne
            for (int row = 0; row < size; row++)
            {
                if (row == col) continue;
                float factor = M[row, col] / M[col, col];
                for (int j = col; j < size; j++)
                    M[row, j] -= factor * M[col, j];
                r[row] -= factor * r[col];
            }
        }
 
        // Extraire la solution
        float[] x = new float[size];
        for (int i = 0; i < size; i++)
            x[i] = r[i] / M[i, i];
 
        return x;
    }
}