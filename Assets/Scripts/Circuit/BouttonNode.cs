using UnityEngine;
using UnityEngine.EventSystems;

public class BoutonNode : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ItemSpawner itemSpawner;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Bouton pressé (au clic, pas au relâchement)");

        itemSpawner.spawnNode();
    }
}