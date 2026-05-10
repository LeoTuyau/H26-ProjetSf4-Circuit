using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// Point de connexion entre composantes.
/// Remplace Anchor — chaque composante a deux Noeud (borne1 et borne2).
/// Un fil relie toujours deux Noeud ensemble.
/// </summary>
public class Noeud : MonoBehaviour
{
    // ─── Références ───────────────────────────────────────────────────
 
    [SerializeField] private Composante parent;
    [SerializeField] private int borne;
    [SerializeField] private float offset;
 
    void Awake()
    {
        rend = GetComponent<Renderer>();
    }
 
    // ─── Connexions ───────────────────────────────────────────────────
 
    private List<Noeud> voisins = new List<Noeud>();
 
    // ─── Simulation ───────────────────────────────────────────────────
 
    // Potentiel électrique à ce noeud (V) — calculé par CircuitManager
    public float Potentiel { get; set; }
 
    // Index dans la matrice G — assigné par CircuitManager avant chaque simulation
    // -1 = noeud de référence (GND)
    public int Index { get; set; } = -1;
 
    // ─── API ──────────────────────────────────────────────────────────
 
    public Composante Parent => parent;
    public int Borne => borne;
    public float Offset => offset;
    public List<Noeud> Voisins => voisins;
 
    public void SetParent(Composante c) => parent = c;
    public void SetBorne(int b) => borne = b;
    public void SetOffset(float o) => offset = o;
 
    public void Connecter(Noeud autre)
    {
        if (!voisins.Contains(autre))
        {
            voisins.Add(autre);
            autre.voisins.Add(this);
        }
    }
 
    public void Deconnecter(Noeud autre)
    {
        if (voisins.Remove(autre))
            autre.voisins.Remove(this);
    }
 
    // ─── Visuel ───────────────────────────────────────────────────────
 
    [SerializeField] private Renderer rend;
    [SerializeField] private Material matDefaut;
    [SerializeField] private Material matSelectionne;
 
    private bool selectionne = false;
    public bool Selectionne => selectionne;
 
    public void ToggleSelect()
    {
        selectionne = !selectionne;
        if (rend != null)
            rend.sharedMaterial = selectionne ? matSelectionne : matDefaut;
    }
 
    public void Deselectionner()
    {
        selectionne = false;
        if (rend != null)
            rend.sharedMaterial = matDefaut;
    }
 
    // ─── Position ─────────────────────────────────────────────────────
 
    void Update()
    {
        if (parent != null)
            transform.position = parent.transform.position
                + parent.transform.right * offset;
    }
}