using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{
    [SerializeField] List<Composante> composantes    = new List<Composante>();
    [SerializeField] List<Pile>       piles          = new List<Pile>();
    [SerializeField] List<Fil>        fils           = new List<Fil>();
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
            MettreAJourSensFils();
        }
        else
        {
            tmp.text = "Circuit ouvert";
            foreach (Fil fil in fils)
                fil.SetSens(0);
        }
    }

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

    public void ToggleFil(bool modeFil)
    {
        if (this.modeFil != modeFil) ToggleFil();
    }

    public void AddFil(Noeud n1, Noeud n2)
    {
        n1.Connecter(n2);
        GameObject filGO = itemSpawner.SpawnFil(n1, n2);
        Fil fil = filGO.GetComponent<Fil>();
        if (fil != null) fils.Add(fil);
    }

    public void AddComposante(Composante c)
    {
        composantes.Add(c);
        if (c is Pile p) piles.Add(p);
    }

    bool circuitFerme()
    {
        if (piles.Count == 0) return false;

        foreach (Pile pile in piles)
        {
            Noeud depart      = pile.Noeud2;
            Noeud destination = pile.Noeud1;

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

    void SimulerCircuit()
    {
        List<Noeud> tousLesNoeuds = CollecterNoeuds();
        if (tousLesNoeuds.Count < 2) return;

        Noeud gnd = piles[0].Noeud1;
        gnd.Potentiel = 0f;
        gnd.Index = -1;

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

        float[,] G = new float[n, n];
        float[]  I = new float[n];

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

        foreach (Pile pile in piles)
        {
            int iPlus  = pile.Noeud2.Index;
            int iMoins = pile.Noeud1.Index;

            if (iPlus < 0) continue;

            float bigG = 1e6f;
            G[iPlus, iPlus] += bigG;
            I[iPlus] += bigG * pile.Tension;

            if (iMoins >= 0)
            {
                G[iMoins, iMoins] += bigG;
                G[iPlus,  iMoins] -= bigG;
                G[iMoins, iPlus]  -= bigG;
                I[iMoins] -= bigG * pile.Tension;
            }
        }

        float[] V = GaussElimination(G, I, n);
        if (V == null)
        {
            Debug.LogWarning("Système singulier — circuit mal formé ?");
            return;
        }

        foreach (Noeud noeud in tousLesNoeuds)
            noeud.Potentiel = noeud.Index >= 0 ? V[noeud.Index] : 0f;

        foreach (Composante c in composantes)
        {
            if (c is Pile)
            {
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
                float delta = c.Noeud2.Potentiel - c.Noeud1.Potentiel;
                c.SetCourant(c.ValeurOhms > 0f ? Mathf.Abs(delta / c.ValeurOhms) : 0f);
            }
        }
    }

    void MettreAJourSensFils()
    {
        foreach (Fil fil in fils)
        {
            Noeud a = fil.NoeudA;
            Noeud b = fil.NoeudB;

            Composante compA = a?.Parent;
            Composante compB = b?.Parent;

            bool courantA = compA != null && compA.Courant > 0.0001f;
            bool courantB = compB != null && compB.Courant > 0.0001f;

            if (!courantA || !courantB) { fil.SetSens(0); continue; }

            bool aEstSortie = (a == compA.Noeud2);
            bool bEstSortie = (b == compB.Noeud2);

            int sens = 0;
            if (aEstSortie && !bEstSortie) sens = 1;
            else if (!aEstSortie && bEstSortie) sens = -1;
            else if (aEstSortie && bEstSortie)
            {
                if (compA is Pile) sens = 1;
                else if (compB is Pile) sens = -1;
            }
            else
            {
                if (compA is Pile) sens = -1;
                else if (compB is Pile) sens = 1;
            }

            fil.SetSens(sens);
        }
    }

    List<Noeud> TrouverGroupe(Noeud depart)
    {
        List<Noeud>    groupe = new List<Noeud>();
        HashSet<Noeud> vus    = new HashSet<Noeud>();
        Queue<Noeud>   queue  = new Queue<Noeud>();
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

    List<Noeud> CollecterNoeuds()
{
    HashSet<Noeud> vus = new HashSet<Noeud>();
    List<Noeud> liste  = new List<Noeud>();

    if (piles.Count == 0) return liste;

    // Partir de la pile et ne collecter que les noeuds connectés
    Queue<Noeud> queue = new Queue<Noeud>();
    queue.Enqueue(piles[0].Noeud1);
    queue.Enqueue(piles[0].Noeud2);

    while (queue.Count > 0)
    {
        Noeud courant = queue.Dequeue();
        if (!vus.Add(courant)) continue;
        liste.Add(courant);

        // Traverser les fils
        foreach (Noeud voisin in courant.Voisins)
            if (!vus.Contains(voisin))
                queue.Enqueue(voisin);

        // Traverser la composante parente
        Noeud autre = GetAutreNoeud(courant);
        if (autre != null && !vus.Contains(autre))
            queue.Enqueue(autre);
    }

    return liste;
}

    float[] GaussElimination(float[,] A, float[] b, int size)
    {
        float[,] M = new float[size, size];
        float[]  r = new float[size];
        for (int i = 0; i < size; i++)
        {
            r[i] = b[i];
            for (int j = 0; j < size; j++)
                M[i, j] = A[i, j];
        }

        for (int col = 0; col < size; col++)
        {
            int   pivotRow = col;
            float maxVal   = Mathf.Abs(M[col, col]);
            for (int row = col + 1; row < size; row++)
            {
                if (Mathf.Abs(M[row, col]) > maxVal)
                {
                    maxVal   = Mathf.Abs(M[row, col]);
                    pivotRow = row;
                }
            }

            if (maxVal < 1e-10f) return null;

            if (pivotRow != col)
            {
                for (int j = 0; j < size; j++)
                    (M[col, j], M[pivotRow, j]) = (M[pivotRow, j], M[col, j]);
                (r[col], r[pivotRow]) = (r[pivotRow], r[col]);
            }

            for (int row = 0; row < size; row++)
            {
                if (row == col) continue;
                float factor = M[row, col] / M[col, col];
                for (int j = col; j < size; j++)
                    M[row, j] -= factor * M[col, j];
                r[row] -= factor * r[col];
            }
        }

        float[] x = new float[size];
        for (int i = 0; i < size; i++)
            x[i] = r[i] / M[i, i];

        return x;
    }
    public void ResetCircuit()
{
    // Désactiver le mode fil si actif
    ToggleFil(false);

    // Supprimer les fils
    foreach (Fil fil in fils)
    {
        if (fil != null)
            Destroy(fil.gameObject);
    }

    // Supprimer les composantes
    foreach (Composante c in composantes)
    {
        if (c != null)
            Destroy(c.gameObject);
    }

    // Vider les listes
    fils.Clear();
    composantes.Clear();
    piles.Clear();
    noeudsAffiches.Clear();

    // Réinitialiser le texte
    tmp.text = "Circuit ouvert";

    // Réinitialiser le mode souris
    modeFil = false;
    mouseManager.SetMode("defaut");
}
}