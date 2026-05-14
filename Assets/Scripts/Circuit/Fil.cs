using System.Collections.Generic;
using UnityEngine;

public class Fil : MonoBehaviour
{
    private Noeud noeudA;
    private Noeud noeudB;

    [SerializeField] private GameObject fleche;
    [SerializeField] private float vitesse = 1f;
    [SerializeField] private int nbFleches = 3;

    private List<GameObject> fleches = new List<GameObject>();
    private List<float> offsets = new List<float>();

    // ─── Initialisation ───────────────────────────────────────────────

    public void SetNoeuds(Noeud a, Noeud b)
    {
        noeudA = a;
        noeudB = b;
        InitFleches();
    }

    private void InitFleches()
    {
        if (fleche == null) return;

        foreach (GameObject f in fleches)
        {
            if (f != null)
                Destroy(f);
        }

        fleches.Clear();
        offsets.Clear();

        for (int i = 0; i < nbFleches; i++)
        {
            GameObject f = Instantiate(fleche, transform.position, Quaternion.identity);

            f.SetActive(false);

            fleches.Add(f);
            offsets.Add((float)i / nbFleches);
        }
    }

    // ─── Sens du courant ──────────────────────────────────────────────

    // 1  = A → B
    // -1 = B → A
    // 0  = aucun courant

    public int Sens { get; private set; } = 0;

    public void SetSens(int s)
    {
        Sens = s;
    }

    public float Courant
    {
        get
        {
            if (noeudA == null || noeudB == null)
                return 0f;

            return noeudA.Potentiel - noeudB.Potentiel;
        }
    }

    public Noeud NoeudA => noeudA;
    public Noeud NoeudB => noeudB;

    // ─── Visuel ───────────────────────────────────────────────────────

    void Update()
    {
        if (noeudA == null || noeudB == null)
            return;

        Vector3 posA = GetPosNoeud(noeudA);
        Vector3 posB = GetPosNoeud(noeudB);

        // IMPORTANT : direction correcte
        Vector3 direction = posB - posA;

        // Position du fil
        transform.position = (posA + posB) / 2f;

        // Rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Ajuste selon ton sprite
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Longueur du fil
        transform.localScale = new Vector3(
            transform.localScale.x,
            direction.magnitude,
            1f
        );

        AnimerFleches(posA, posB);
    }

    Vector3 GetPosNoeud(Noeud n)
    {
        if (n.Parent != null)
        {
            return n.Parent.transform.position
                   + n.Parent.transform.right * n.Offset;
        }

        return n.transform.position;
    }

    // ─── Animation flèches ────────────────────────────────────────────

    private void AnimerFleches(Vector3 posA, Vector3 posB)
    {
        if (fleches.Count == 0)
            return;

        if (Sens != 0)
        {
            Quaternion rotNormale = transform.rotation;

            Quaternion rotInversee = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z + 180f
            );

            for (int i = 0; i < fleches.Count; i++)
            {
                offsets[i] += Time.deltaTime * vitesse * Sens;

                offsets[i] = Mathf.Repeat(offsets[i], 1f);

                fleches[i].transform.position =
                    Vector3.Lerp(posA, posB, offsets[i])
                    + new Vector3(0, 0, -1);

                fleches[i].transform.rotation =
                    Sens == 1 ? rotNormale : rotInversee;

                fleches[i].SetActive(true);
            }
        }
        else
        {
            foreach (GameObject f in fleches)
            {
                f.SetActive(false);
            }
        }
    }

    // ─── Nettoyage ────────────────────────────────────────────────────

    void OnDestroy()
    {
        foreach (GameObject f in fleches)
        {
            if (f != null)
                Destroy(f);
        }
    }
}