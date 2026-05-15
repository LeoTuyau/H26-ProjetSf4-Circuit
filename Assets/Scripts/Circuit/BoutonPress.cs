using UnityEngine;
using UnityEngine.EventSystems;

public class BoutonPress : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ItemSpawner itemSpawner;
    public void OnPointerDown(PointerEventData eventData)
    {

        itemSpawner.SpawnPile();
    }
}