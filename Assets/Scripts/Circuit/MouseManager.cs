using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class MouseManager : MonoBehaviour
{
    [SerializeField] GameObject currentObject;
    [SerializeField] GameObject filConnexion1;
    string mode = "defaut";
    [SerializeField] CircuitManager CirMng;
    Vector3 mousePos;
    Vector3 worldPos;
    [SerializeField] TMP_Text tmp;
    int filCount = 0;
    [SerializeField] List<GameObject> selectedAnchors = new List<GameObject>();


    [SerializeField] ItemSpawner itemSpawner;

    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //Switch
        switch (mode)
        {
            case "defaut":
                tmp.text = "Mode : defaut";
                selectedAnchors.Clear();
                UpdateModeDefault();
                break;
            case "buttonPress":
                tmp.text = "Mode : buttonPress";
                selectedAnchors.Clear();
                UpdateRelease();
                break;
            case "fil":
                tmp.text = "Mode : fil";
                UpdateModeFil();
                break;
        }
    }
    private void UpdateModeDefault()
    {
        // Quand on clique
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentObject = hit.collider.gameObject;
                Debug.Log("Clicked on: " + currentObject.name);
            }
        }
        // Tant que le bouton est maintenu
        if (Mouse.current.leftButton.isPressed && currentObject != null && !currentObject.CompareTag("Anchor") && !currentObject.CompareTag("Background"))
        {
            currentObject.transform.position = new Vector3(worldPos.x, worldPos.y, currentObject.transform.position.z);
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentObject.transform.Rotate(0, 0, 90);
            }
        }
        // Quand on rel�che
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            currentObject = null;
        }
    }
    private void UpdateRelease()
    {
        // Tant que le bouton est maintenu
        if (Mouse.current.leftButton.isPressed && currentObject != null && !currentObject.CompareTag("Anchor") && !currentObject.CompareTag("Background"))
        {
            currentObject.transform.position = new Vector3(worldPos.x, worldPos.y, currentObject.transform.position.z);
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentObject.transform.Rotate(0, 0, 90);
            }
        }
        // Quand on relache
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            currentObject = null;
            mode = "defaut";
        }
    }
    private void UpdateModeFil()
    {
        if (filCount < 2) //On n'est pas encore a deux anchors
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePos);

                if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Anchor"))
                {
                    currentObject = hit.collider.gameObject;
                    Anchor anchor = currentObject.GetComponent<Anchor>();
                    if (anchor != null)
                    {
                        if (!anchor.GetSelect()) // Anchor pas selecte
                        {
                            if (filCount == 1)
                            {
                                if (selectedAnchors[0].GetComponent<Anchor>().GetAttache() != anchor.GetAttache())
                                {
                                    selectedAnchors.Add(currentObject);
                                    anchor.ToggleSelect();
                                }
                            }
                            else // selectedAnchors = 0
                            {
                                selectedAnchors.Add(currentObject);
                                anchor.ToggleSelect();
                            }
                        }
                        else // Anchor deja selecte
                        {
                            selectedAnchors.Remove(currentObject);
                            anchor.ToggleSelect();
                        }
                    }
                }
            }
        }
        else
        {
            mode = "defaut";
            CirMng.AddFil(selectedAnchors);
            selectedAnchors.Clear();
            CirMng.ToggleFil(false);
        }
        filCount = selectedAnchors.Count;
    }
    public void DragButtonStart(GameObject obj)
    {
        mode = "buttonPress";
        currentObject = obj;
        CirMng.ToggleFil(false);
    }
    public void SetMode(string mode)
    {
        this.mode = mode;
    }
}