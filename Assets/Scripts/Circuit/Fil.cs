using UnityEngine;
 
public class Fil : MonoBehaviour
{
    private Noeud noeudA;
    private Noeud noeudB;
 
    public void SetNoeuds(Noeud a, Noeud b)
    {
        noeudA = a;
        noeudB = b;
    }
 
    // ─── Sens du courant ──────────────────────────────────────────────
 
    // 1  = courant va de A vers B (potentiel A > potentiel B)
    // -1 = courant va de B vers A (potentiel B > potentiel A)
    //  0 = pas de courant
    public int Sens
    {
        get
        {
            if (noeudA == null || noeudB == null) return 0;
            float diff = noeudA.Potentiel - noeudB.Potentiel;
            if (Mathf.Abs(diff) < 0.0001f) return 0;
            return diff > 0 ? 1 : -1;
        }
    }
 
    // Intensité du courant dans ce fil (A)
    public float Courant
    {
        get
        {
            if (noeudA == null || noeudB == null) return 0f;
            return noeudA.Potentiel - noeudB.Potentiel; // sera divisé par R dans la simulation
        }
    }
 
    public Noeud NoeudA => noeudA;
    public Noeud NoeudB => noeudB;
 
    // ─── Visuel ───────────────────────────────────────────────────────
 
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
    }
 
    Vector3 GetPosNoeud(Noeud n)
    {
        if (n.Parent != null)
            return n.Parent.transform.position + n.Parent.transform.right * n.Offset;
        return n.transform.position;
    }
}