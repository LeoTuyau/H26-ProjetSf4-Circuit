using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
 
public class MouseManager : MonoBehaviour
{
    [SerializeField] CircuitManager circuitManager;
    [SerializeField] ItemSpawner    itemSpawner;
    [SerializeField] TMP_Text       tmp;
    [SerializeField] HelpMenu menu;
 
    private string mode = "defaut";
    private GameObject objetCourant;
    private List<Noeud> noeudsSelectionnes = new List<Noeud>();
 
    Vector3 mousePos;
    Vector3 worldPos;
 
    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);
 
        switch (mode)
        {
            case "defaut":
                tmp.text = "Mode : défaut";
                UpdateModeDefaut();
                break;
            case "buttonPress":
                tmp.text = "Mode : déplacement";
                UpdateModeRelease();
                break;
            case "fil":
                tmp.text = "Mode : fil";
                UpdateModeFil();
                break;
        }
    }
 
    // ─── Mode défaut ──────────────────────────────────────────────────
 
    private void UpdateModeDefaut()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
                objetCourant = hit.collider.gameObject;
        }
 
        if (Mouse.current.leftButton.isPressed && objetCourant != null
            && !objetCourant.CompareTag("Noeud")
            && !objetCourant.CompareTag("Background"))
        {
            objetCourant.transform.position = new Vector3(worldPos.x, worldPos.y,
                objetCourant.transform.position.z);
        }
 
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            objetCourant = null;
        if (Mouse.current.leftButton.isPressed && objetCourant != null
            && !objetCourant.CompareTag("Noeud")
            && !objetCourant.CompareTag("Background"))
        {
            objetCourant.transform.position = new Vector3(worldPos.x, worldPos.y,
            objetCourant.transform.position.z);
            if (Input.GetKeyDown(KeyCode.R))
            {
                objetCourant.transform.Rotate(0f, 0f, 90f);
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            menu.Toggle();
        }
    }
 
    // ─── Mode release (après spawn bouton) ───────────────────────────
 
    private void UpdateModeRelease()
    {
        if (Mouse.current.leftButton.isPressed && objetCourant != null
            && !objetCourant.CompareTag("Background"))
        {
            objetCourant.transform.position = new Vector3(worldPos.x, worldPos.y,
                objetCourant.transform.position.z);
        }
 
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            objetCourant = null;
            mode = "defaut";
        }
        if (Mouse.current.leftButton.isPressed && objetCourant != null
            && !objetCourant.CompareTag("Noeud")
            && !objetCourant.CompareTag("Background"))
        {
            objetCourant.transform.position = new Vector3(worldPos.x, worldPos.y,
            objetCourant.transform.position.z);
            if (Input.GetKeyDown(KeyCode.R))
            {
                objetCourant.transform.Rotate(0f, 0f, 90f);
            }
        } 
    }
 
    // ─── Mode fil ─────────────────────────────────────────────────────
 
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
                    // Désélectionner
                    noeud.ToggleSelect();
                    noeudsSelectionnes.Remove(noeud);
                }
                else
                {
                    // Ne pas connecter deux noeuds du même parent
                    if (noeudsSelectionnes.Count == 1
                        && noeudsSelectionnes[0].Parent == noeud.Parent)
                        return;
 
                    noeud.ToggleSelect();
                    noeudsSelectionnes.Add(noeud);
                }
 
                // Deux noeuds sélectionnés → créer le fil
                if (noeudsSelectionnes.Count == 2)
                {
                    circuitManager.AddFil(noeudsSelectionnes[0], noeudsSelectionnes[1]);
 
                    // Déselectionner
                    foreach (Noeud n in noeudsSelectionnes)
                        n.Deselectionner();
                    noeudsSelectionnes.Clear();
 
                    circuitManager.ToggleFil(false);
                }
            }
        }
    }
 
    // ─── API publique ─────────────────────────────────────────────────
 
    public void DragButtonStart(GameObject obj)
    {
        mode = "buttonPress";
        objetCourant = obj;
        circuitManager.ToggleFil(false);
    }
 
    public void SetMode(string m) => mode = m;
}