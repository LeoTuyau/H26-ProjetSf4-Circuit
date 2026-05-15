using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fabrique centralisée pour l'instanciation des éléments du circuit.
/// Responsable de :
/// - Spawner les composantes (Pile, Résistance) avec leurs noeuds associés
/// - Spawner les noeuds libres (points de jonction)
/// - Spawner les fils visuels entre deux noeuds
/// - Enregistrer chaque nouvelle composante auprès du <see cref="CircuitManager"/>
/// - Passer le contrôle au <see cref="MouseManager"/> pour le glisser-déposer
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject pilePrefab;       // Prefab de la pile
    [SerializeField] GameObject resistancePrefab; // Prefab de la résistance
    [SerializeField] GameObject noeudPrefab;      // Prefab des noeuds (bornes de composante ou jonction libre)
    [SerializeField] GameObject filPrefab;        // Prefab du fil électrique

    [SerializeField] MouseManager mouseManager;   // Pour déclencher le glisser-déposer après spawn
    [SerializeField] CircuitManager circuitManager; // Pour enregistrer les composantes dans la simulation

    // ─── Spawn composantes ────────────────────────────────────────────

    /// <summary>
    /// Instancie une pile à la position de la souris.
    /// Crée ses deux noeuds (borne –, borne +), l'enregistre dans le circuit
    /// et active le glisser-déposer immédiatement.
    /// </summary>
    public void SpawnPile()
    {
        Vector3 pos = GetMouseWorldPos();
        GameObject go = Instantiate(pilePrefab, pos, Quaternion.identity);
        Pile pile = go.GetComponent<Pile>();

        SpawnNoeudsComposante(pile, 0.5f);
        circuitManager.AddComposante(pile);
        mouseManager.DragButtonStart(go);
    }

    /// <summary>
    /// Instancie une résistance à la position de la souris.
    /// Crée ses deux noeuds (borne –, borne +), l'enregistre dans le circuit
    /// et active le glisser-déposer immédiatement.
    /// </summary>
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
    /// Crée et configure les deux noeuds d'une composante.
    /// 
    /// - Noeud1 (borne –) : placé à gauche, offset négatif
    /// - Noeud2 (borne +) : placé à droite, offset positif
    /// 
    /// Les noeuds sont désactivés par défaut et ne deviennent visibles
    /// que lorsque le mode fil est actif dans le <see cref="CircuitManager"/>.
    /// </summary>
    /// <param name="c">La composante à laquelle attacher les noeuds.</param>
    /// <param name="offset">Distance en unités monde entre le centre de la composante et chaque noeud.</param>
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
    /// Instancie un noeud libre (point de jonction sans composante parente).
    /// Contrairement aux noeuds de composantes, il est toujours visible
    /// et peut servir de point de connexion intermédiaire dans le circuit.
    /// Active le glisser-déposer pour que l'utilisateur puisse le placer.
    /// </summary>
    /// <returns>Le GameObject du noeud libre instancié.</returns>
    public GameObject SpawnNoeudLibre()
    {
        Vector3 pos = GetMouseWorldPos();
        GameObject go = Instantiate(noeudPrefab, pos, Quaternion.identity);
        Noeud n = go.GetComponent<Noeud>();
        n.SetParent(null);
        n.SetBorne(0);
        n.SetOffset(0f);
        go.SetActive(true);
        mouseManager.DragButtonStart(go);
        return go;
    }

    /// <summary>
    /// Instancie un fil visuel entre deux noeuds existants.
    /// Le fil est centré entre les deux noeuds et ses références sont
    /// transmises au composant <see cref="Fil"/> pour l'animation.
    /// </summary>
    /// <param name="n1">Noeud de départ du fil.</param>
    /// <param name="n2">Noeud d'arrivée du fil.</param>
    /// <returns>Le GameObject du fil instancié.</returns>
    public GameObject SpawnFil(Noeud n1, Noeud n2)
    {
        Vector3 pos = (n1.transform.position + n2.transform.position) / 2f;
        GameObject go = Instantiate(filPrefab, pos, Quaternion.identity);
        go.GetComponent<Fil>().SetNoeuds(n1, n2);
        return go;
    }

    // ─── Helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Convertit la position actuelle de la souris en coordonnées monde (plan Z = 0).
    /// Utilisé pour placer les composantes spawnées sous le curseur.
    /// </summary>
    /// <returns>Position monde de la souris avec Z = 0.</returns>
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return new Vector3(worldPos.x, worldPos.y, 0f);
    }
}