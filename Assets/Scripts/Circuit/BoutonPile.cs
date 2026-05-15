using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Bouton UI qui spawne une pile au clic.
/// Implémente <see cref="IPointerDownHandler"/> pour réagir dès l'appui
/// (sans attendre le relâchement, contrairement à un Button classique Unity).
/// 
/// À placer sur un GameObject UI avec un composant Image (raycast target activé).
/// </summary>
public class BoutonPress : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ItemSpawner itemSpawner; // Référence au spawner chargé d'instancier les composantes

    /// <summary>
    /// Appelé dès que l'utilisateur appuie sur ce bouton.
    /// Demande à l'<see cref="ItemSpawner"/> de créer une nouvelle pile dans la scène.
    /// </summary>
    /// <param name="eventData">Données de l'événement pointeur (position, bouton, etc.).</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        itemSpawner.SpawnPile();
    }
}