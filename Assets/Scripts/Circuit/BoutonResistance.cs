using UnityEngine;
using UnityEngine.EventSystems;

public class BoutonResistance : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ItemSpawner itemSpawner;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Bouton press� (au clic, pas au rel�chement)");

        itemSpawner.SpawnResistance();
    }
}