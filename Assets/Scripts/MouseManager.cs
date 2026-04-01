using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
    
    [SerializeField] ItemSpawner itemSpawner;

    void Update()
    {

        mousePos = Mouse.current.position.ReadValue();
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //Switch
        switch (mode)
        {
            case "defaut":
                UpdateModeDefault();
                tmp.text = "Mode : defaut";
                break;
            case "buttonPress":
                UpdateRelease();
                tmp.text = "Mode : buttonPress";
                break;
            case "fil":
                UpdateModeFil();
                tmp.text = "Mode : fil";
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
        }
        // Quand on relâche
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
        }
        // Quand on relâche
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            currentObject = null;
            mode = "defaut";
        }
    }
    private void UpdateModeFil()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit)&& hit.collider.CompareTag("Anchor"))
            {
                currentObject = hit.collider.gameObject;
                Anchor anchor = currentObject.GetComponent<Anchor>();
                if (anchor != null)
                {
                    if (!anchor.GetSelect())
                    {
                        filCount++;
                        if (filCount == 1)
                        {
                            filConnexion1 = currentObject;
                        }
                        if (filCount == 2)
                        {
                            itemSpawner.SpawnFil(filConnexion1, currentObject);
                            filConnexion1.GetComponent<Anchor>().ToggleSelect();
                            currentObject.GetComponent<Anchor>().ToggleSelect();
                            mode = "defaut";
                            filCount = 0;
                        }
                    }
                    else
                    {
                        filCount--;
                    }
                    anchor.ToggleSelect();
                }
            }
        }
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