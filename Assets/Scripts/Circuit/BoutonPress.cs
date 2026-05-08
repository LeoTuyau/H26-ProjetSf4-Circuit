using UnityEngine;
using UnityEngine.EventSystems;

public class BoutonPress : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ItemSpawner itemSpawner;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Bouton press (au clic, pas au relachement)");

        itemSpawner.SpawnPile();
    }
}