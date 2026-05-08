using UnityEngine;
using UnityEngine.EventSystems;

public class BouttonNoeud : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ItemSpawner itemSpawner;

    public void OnPointerDown(PointerEventData eventData)
    {
        itemSpawner.SpawnNoeudLibre();
    }
}