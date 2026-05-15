using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fil : MonoBehaviour
{
    private Noeud noeudA;
    private Noeud noeudB;

    public float Potentiel;

    [SerializeField] TextMeshPro textPrefab;
    TextMeshPro textP;

    private bool modePotentiel = false;

    [SerializeField] private GameObject fleche;
    [SerializeField] private float vitesse   = 1f;
    [SerializeField] private int   nbFleches = 3;

    private List<GameObject> fleches = new List<GameObject>();
    private List<float>      offsets = new List<float>();

    // Indique si NoeudA est la borne+ (source du courant)
    // Stocké au moment de la connexion, indépendant de l'ordre de clic
    private bool bornePlusEstA = true;
    public bool BornePlusEstA => bornePlusEstA;
    public void SetBornePlusEstA(bool v) => bornePlusEstA = v;

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
            if (f != null) Destroy(f);
        fleches.Clear();
        offsets.Clear();

        for (int i = 0; i < nbFleches; i++)
        {
            GameObject f = Instantiate(fleche, transform.position, transform.rotation);
            f.SetActive(false);
            fleches.Add(f);
            offsets.Add((float)i / nbFleches);
        }
    }

    // ─── Sens du courant ──────────────────────────────────────────────
    // 1  = courant va de A vers B
    // -1 = courant va de B vers A
    //  0 = pas de courant
    public int Sens { get; private set; } = 0;
    public void SetSens(int s) => Sens = s;

    public Noeud NoeudA => noeudA;
    public Noeud NoeudB => noeudB;

    // ─── Visuel ───────────────────────────────────────────────────────

    void Start()
    {
        textP = Instantiate(textPrefab,transform.position,Quaternion.LookRotation(transform.forward));
    }
    void Update()
    {
        if (noeudA == null || noeudB == null) return;

        Vector3 posA = GetPosNoeud(noeudA);
        Vector3 posB = GetPosNoeud(noeudB);
        Vector3 direction = posA - posB;

        transform.position = (posA + posB) / 2f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        transform.localScale = new Vector3(transform.localScale.x, direction.magnitude, 1f);

        AnimerFleches(posA, posB);

        if (modePotentiel)
        {
            textP.text = Potentiel+" V";
            textP.transform.position = transform.position + new Vector3(-0.3f,0,0);
            textP.transform.rotation = Quaternion.LookRotation(transform.forward);
        }
    }

    Vector3 GetPosNoeud(Noeud n)
    {
        if (n.Parent != null)
            return n.Parent.transform.position + n.Parent.transform.right * n.Offset;
        return n.transform.position;
    }

    // ─── Animation flèches ────────────────────────────────────────────

    private void AnimerFleches(Vector3 posA, Vector3 posB)
    {
        if (fleches.Count == 0) return;

        if (Sens != 0)
        {
            // Source = borne+ (là où le courant sort)
            // bornePlusEstA indique si NoeudA est la borne+
            // Sens==1 : courant va de A vers B (sort de A)
            // Sens==-1 : courant va de B vers A (sort de B)
            Vector3 source = Sens == 1 ? posA : posB;
            Vector3 dest   = Sens == 1 ? posB : posA;

            Vector3 dir = (dest - source).normalized;
            if (dir == Vector3.zero) dir = Vector3.up;
            float rotAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rot = Quaternion.Euler(0f, 0f, rotAngle);

            for (int i = 0; i < fleches.Count; i++)
            {
                offsets[i] += Time.deltaTime * vitesse;
                offsets[i]  = Mathf.Repeat(offsets[i], 1f);

                fleches[i].transform.position = Vector3.Lerp(source, dest, offsets[i])
                                               + new Vector3(0, 0, -1);
                fleches[i].transform.rotation  = rot;
                fleches[i].SetActive(true);
            }
        }
        else
        {
            foreach (GameObject f in fleches)
                f.SetActive(false);
        }
    }

    // ─── Nettoyage ────────────────────────────────────────────────────

    void OnDestroy()
    {
        foreach (GameObject f in fleches)
            if (f != null) Destroy(f);
    }
    public void ToggleModeP(bool b)
    {
        modePotentiel = b;
        if (!b)
        {
            textP.text = "";
        }
    }
}