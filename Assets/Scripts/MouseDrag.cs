using UnityEngine;
using UnityEngine.InputSystem;

public class MouseDrag : MonoBehaviour
{
    GameObject currentObject;
    string mode;

    void Update()
    {
        Vector3 mousePos;
        Vector3 worldPos;
        switch (mode)
        {
            case "buttonPress":
                // Tant que le bouton est maintenu
                
                mousePos = Mouse.current.position.ReadValue();
                worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                currentObject.transform.position = new Vector3(worldPos.x, worldPos.y, currentObject.transform.position.z);
                

                // Quand on relâche
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    currentObject = null;

                }
                mode = "defaut";
                break;
            case "fil":
                if (Mouse.current == null) return;

                // Quand on clique
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    mousePos = Mouse.current.position.ReadValue();
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        currentObject = hit.collider.gameObject;
                        Debug.Log("Clicked on: " + currentObject.name);
                    }
                }
                mode = "defaut";
                break;
            default:
                if (Mouse.current == null) return;

                // Quand on clique
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    mousePos = Mouse.current.position.ReadValue();
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        currentObject = hit.collider.gameObject;
                        Debug.Log("Clicked on: " + currentObject.name);
                    }
                }
                // Tant que le bouton est maintenu
                if (Mouse.current.leftButton.isPressed && currentObject != null && !currentObject.name.Equals("Background") && !currentObject.name.Equals("Anchor"))
                {
                    mousePos = Mouse.current.position.ReadValue();
                    worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                    currentObject.transform.position = new Vector3(worldPos.x, worldPos.y, currentObject.transform.position.z);
                }

                // Quand on relâche
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    currentObject = null;
                    
                }
                break;
        }
        
            
        

        // Tant que le bouton est maintenu
        if (Mouse.current.leftButton.isPressed && currentObject != null && !currentObject.name.Equals("Background"))
        {
            mousePos = Mouse.current.position.ReadValue();
            worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            currentObject.transform.position = new Vector3(worldPos.x, worldPos.y, currentObject.transform.position.z);
        }

        // Quand on relâche
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            currentObject = null;
        }
    }
    public void ButtonPressStart(GameObject obj)
    {
        currentObject = obj;
    }
    public void ModeFil(bool fil)
    {
        if (fil)
        {
            mode = "fil";
        }
        else
        {
            mode = "defaut";
        }
    }
}