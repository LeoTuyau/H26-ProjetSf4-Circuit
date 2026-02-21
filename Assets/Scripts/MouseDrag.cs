using UnityEngine;
using UnityEngine.InputSystem;

public class MouseDrag : MonoBehaviour
{
    GameObject currentObject;

    void Update()
    {
        if (Mouse.current == null) return;

        // Quand on clique
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentObject = hit.collider.gameObject;
                Debug.Log("Clicked on: " + currentObject.name);
            }
        }

        // Tant que le bouton est maintenu
        if (Mouse.current.leftButton.isPressed && currentObject != null && !currentObject.name.Equals("Background"))
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            currentObject.transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        }

        // Quand on relâche
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            currentObject = null;
        }
    }
}