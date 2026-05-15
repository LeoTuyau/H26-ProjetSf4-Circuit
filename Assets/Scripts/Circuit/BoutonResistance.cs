using UnityEngine;
using UnityEngine.EventSystems;

public class BoutonResistance : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ItemSpawner itemSpawner;
    public void OnPointerDown(PointerEventData eventData)
    {

        itemSpawner.SpawnResistance();
    }
}