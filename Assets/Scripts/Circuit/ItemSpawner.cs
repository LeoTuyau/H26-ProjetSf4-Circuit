using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject pilePrefab;
    [SerializeField] GameObject resistancePrefab;
    [SerializeField] GameObject noeudPrefab;
    [SerializeField] GameObject filPrefab;
 
    [SerializeField] MouseManager  mouseManager;
    [SerializeField] CircuitManager circuitManager;
 
    // ─── Spawn composantes ────────────────────────────────────────────
 
    public void SpawnPile()
    {
        Vector3 pos = GetMouseWorldPos();
        GameObject go = Instantiate(pilePrefab, pos, Quaternion.identity);
        Pile pile = go.GetComponent<Pile>();
 
        SpawnNoeudsComposante(pile, 0.5f);
        circuitManager.AddComposante(pile);
        mouseManager.DragButtonStart(go);
    }
 
    public void SpawnResistance()
    {
        Vector3 pos = GetMouseWorldPos();
        GameObject go = Instantiate(resistancePrefab, pos, Quaternion.identity);
        Resistance r = go.GetComponent<Resistance>();
 
        SpawnNoeudsComposante(r, 1f);
        circuitManager.AddComposante(r);
        mouseManager.DragButtonStart(go);
    }
 
    /// <summary>
    /// Crée noeud1 (borne –) et noeud2 (borne +) pour une composante.
    /// Les noeuds sont cachés par défaut, visibles seulement en mode fil.
    /// </summary>
    private void SpawnNoeudsComposante(Composante c, float offset)
    {
        // Noeud 1 — borne – (gauche)
        GameObject go1 = Instantiate(noeudPrefab, c.transform.position, Quaternion.identity);
        Noeud n1 = go1.GetComponent<Noeud>();
        n1.SetParent(c);
        n1.SetBorne(1);
        n1.SetOffset(-offset);
        go1.SetActive(false);
        c.SetNoeud1(n1);
 
        // Noeud 2 — borne + (droite)
        GameObject go2 = Instantiate(noeudPrefab, c.transform.position, Quaternion.identity);
        Noeud n2 = go2.GetComponent<Noeud>();
        n2.SetParent(c);
        n2.SetBorne(2);
        n2.SetOffset(offset);
        go2.SetActive(false);
        c.SetNoeud2(n2);
    }
 
    /// <summary>
    /// Spawn un noeud libre (point de jonction sans composante).
    /// </summary>
    public GameObject SpawnNoeudLibre()
    {
        Vector3 pos = GetMouseWorldPos();
        GameObject go = Instantiate(noeudPrefab, pos, Quaternion.identity);
        Noeud n = go.GetComponent<Noeud>();
        n.SetParent(null);
        n.SetBorne(0);
        n.SetOffset(0f);
        go.SetActive(true); // toujours visible contrairement aux noeuds de composantes

        circuitManager.AddNoeudLibre(n); // pour le mode fil
        mouseManager.DragButtonStart(go);
        return go;
    }
 
    /// <summary>
    /// Crée un fil visuel entre deux noeuds.
    /// </summary>
    public GameObject SpawnFil(Noeud n1, Noeud n2)
    {
        Vector3 pos = (n1.transform.position + n2.transform.position) / 2f;
        GameObject go = Instantiate(filPrefab, pos, Quaternion.identity);
        go.GetComponent<Fil>().SetNoeuds(n1, n2);
        return go;
    }
 
    // ─── Helpers ──────────────────────────────────────────────────────
 
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return new Vector3(worldPos.x, worldPos.y, 0f);
    }
}