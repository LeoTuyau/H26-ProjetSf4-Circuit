using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Gestionnaire de la souris et des interactions utilisateur.
/// Responsable de :
/// - Déplacer les composantes par glisser-déposer
/// - Gérer la sélection et la connexion de noeuds en mode fil
/// - Gérer les raccourcis clavier (H, P, R, S)
/// - Coordonner les changements de mode avec le CircuitManager
/// 
/// Modes disponibles :
/// - "defaut"      : déplacement des composantes et raccourcis clavier
/// - "buttonPress" : glisser-déposer après spawn d'une composante via un bouton
/// - "fil"         : sélection de noeuds pour créer des fils
/// </summary>
public class MouseManager : MonoBehaviour
{
    [SerializeField] CircuitManager circuitManager;
    [SerializeField] ItemSpawner itemSpawner;
    [SerializeField] HelpMenu menu;

    /// <summary>Mode d'interaction actuel ("defaut", "buttonPress" ou "fil").</summary>
    private string mode = "defaut";

    /// <summary>GameObject actuellement maintenu / déplacé par la souris.</summary>
    private GameObject objetCourant;

    /// <summary>Liste des noeuds sélectionnés en mode fil (max 2 avant création du fil).</summary>
    private List<Noeud> noeudsSelectionnes = new List<Noeud>();

    Vector3 mousePos; // Position de la souris en pixels écran
    Vector3 worldPos; // Position de la souris convertie en coordonnées monde

    /// <summary>
    /// Chaque frame : lit la position de la souris et délègue au sous-mode actif.
    /// </summary>
    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        switch (mode)
        {
            case "defaut":
                UpdateModeDefaut();
                break;
            case "buttonPress":
                UpdateModeRelease();
                break;
            case "fil":
                UpdateModeFil();
                break;
        }
    }

    // ─── Mode défaut ──────────────────────────────────────────────────

    /// <summary>
    /// Mode principal. Gère :
    /// - Le glisser-déposer des composantes (clic gauche maintenu)
    /// - La rotation de la composante tenue (touche R)
    /// - Les raccourcis clavier globaux (H, P, S)
    /// 
    /// Les tags "Noeud", "Background" et "Fil" sont exclus du déplacement.
    /// </summary>
    private void UpdateModeDefaut()
    {
        // Détecte l'objet cliqué au premier frame du clic
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
                objetCourant = hit.collider.gameObject;
        }

        // Déplace l'objet tenu à la position de la souris
        if (Mouse.current.leftButton.isPressed && objetCourant != null
            && !objetCourant.CompareTag("Noeud")
            && !objetCourant.CompareTag("Background")
            && !objetCourant.CompareTag("Fil"))
        {
            objetCourant.transform.position = new Vector3(worldPos.x, worldPos.y, objetCourant.transform.position.z);

            // Rotation de 90° sur l'axe Z avec la touche R
            if (Input.GetKeyDown(KeyCode.R))
            {
                objetCourant.transform.Rotate(0f, 0f, 90f);
            }
        }

        // Relâchement du clic : on libère l'objet
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            objetCourant = null;

        // Raccourci H : affiche/masque le menu d'aide
        if (Input.GetKeyDown(KeyCode.H))
            menu.Toggle();

        // Raccourci P : active/désactive l'affichage des potentiels sur les fils
        if (Input.GetKeyDown(KeyCode.P))
            circuitManager.ToggleModePotentielFil();

        // Raccourci S : active/désactive le mode slider des composantes
        if (Input.GetKeyDown(KeyCode.S))
            circuitManager.ToggleModeSlider();
    }

    // ─── Mode release (après spawn bouton) ───────────────────────────

    /// <summary>
    /// Mode actif immédiatement après qu'un bouton de l'UI ait spawné une composante.
    /// L'utilisateur maintient le clic et place la composante où il veut.
    /// Dès qu'il relâche, le mode repasse en "defaut".
    /// 
    /// Le tag "Background" est exclu du déplacement.
    /// La touche R permet de faire pivoter la composante de 90°.
    /// </summary>
    private void UpdateModeRelease()
    {
        // Déplace la composante spawnée à la position de la souris
        if (Mouse.current.leftButton.isPressed && objetCourant != null
            && !objetCourant.CompareTag("Background"))
        {
            objetCourant.transform.position = new Vector3(worldPos.x, worldPos.y,
                objetCourant.transform.position.z);

            // Rotation de 90° sur l'axe Z avec la touche R
            if (!objetCourant.CompareTag("Noeud")
                && Input.GetKeyDown(KeyCode.R))
            {
                objetCourant.transform.Rotate(0f, 0f, 90f);
            }
        }

        // Relâchement : fin du placement, retour au mode défaut
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            objetCourant = null;
            mode = "defaut";
        }
    }

    // ─── Mode fil ─────────────────────────────────────────────────────

    /// <summary>
    /// Mode de connexion par fil.
    /// L'utilisateur clique sur deux noeuds pour les relier par un fil.
    /// 
    /// Règles :
    /// - Cliquer sur un noeud déjà sélectionné le désélectionne.
    /// - On ne peut pas connecter deux noeuds appartenant à la même composante.
    /// - Quand deux noeuds sont sélectionnés, le fil est créé et le mode fil se ferme.
    /// </summary>
    private void UpdateModeFil()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit)
                && hit.collider.CompareTag("Noeud"))
            {
                Noeud noeud = hit.collider.GetComponent<Noeud>();
                if (noeud == null) return;

                if (noeud.Selectionne)
                {
                    // Désélectionne le noeud si déjà sélectionné
                    noeud.ToggleSelect();
                    noeudsSelectionnes.Remove(noeud);
                }
                else
                {
                    // Empêche la connexion de deux noeuds de la même composante
                    if (noeudsSelectionnes.Count == 1
                        && noeudsSelectionnes[0].Parent == noeud.Parent)
                        return;

                    noeud.ToggleSelect();
                    noeudsSelectionnes.Add(noeud);
                }

                // Deux noeuds sélectionnés : créer le fil et réinitialiser la sélection
                if (noeudsSelectionnes.Count == 2)
                {
                    circuitManager.AddFil(noeudsSelectionnes[0], noeudsSelectionnes[1]);

                    foreach (Noeud n in noeudsSelectionnes)
                        n.Deselectionner();
                    noeudsSelectionnes.Clear();

                    // Ferme automatiquement le mode fil après la connexion
                    circuitManager.ToggleFil(false);
                }
            }
        }
    }

    // ─── API publique ─────────────────────────────────────────────────

    /// <summary>
    /// Appelé par un bouton de l'UI quand l'utilisateur commence à faire glisser
    /// une composante fraîchement spawnée. Passe en mode "buttonPress" et
    /// désactive le mode fil si actif.
    /// </summary>
    /// <param name="obj">Le GameObject spawné à faire glisser.</param>
    public void DragButtonStart(GameObject obj)
    {
        mode = "buttonPress";
        objetCourant = obj;
        circuitManager.ToggleFil(false);
    }

    /// <summary>
    /// Change le mode d'interaction courant.
    /// Appelé par le CircuitManager ou d'autres systèmes externes.
    /// </summary>
    /// <param name="m">Nom du mode : "defaut", "buttonPress" ou "fil".</param>
    public void SetMode(string m) => mode = m;
}